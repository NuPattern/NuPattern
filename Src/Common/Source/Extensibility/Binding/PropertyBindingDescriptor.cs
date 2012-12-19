using System;
using System.ComponentModel;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
	/// <summary>
	/// A custom descriptor that automatically serializes a property binding to Json and exposes 
	/// the Value and ValueProvider properties for it.
	/// </summary>
	public class PropertyBindingDescriptor : DelegatingPropertyDescriptor
	{
		private IPropertyBindingSettings settings;

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyBindingDescriptor"/> class.
		/// </summary>
		/// <param name="innerDescriptor">The inner descriptor.</param>
		/// <param name="overriddenAttributes">The optional custom attributes.</param>
		public PropertyBindingDescriptor(PropertyDescriptor innerDescriptor, params Attribute[] overriddenAttributes)
			: base(innerDescriptor, overriddenAttributes)
		{
		}

		/// <summary>
		/// When overridden in a class, determines if the vaue of this property can be reset.
		/// </summary>
		public override bool CanResetValue(object component)
		{
			return true;
		}

		/// <summary>
		/// Changes the property type to expose the design Value and ValueProvider type.
		/// </summary>
		public override Type PropertyType
		{
			get { return typeof(DesignProperty); }
		}

		/// <summary>
		/// Specifies that the value should be serialized always.
		/// </summary>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		/// <summary>
		/// Always returns <see langword="true"/>.
		/// </summary>
		public override bool SupportsChangeEvents
		{
			get { return true; }
		}

		/// <summary>
		/// When overridden in a derived class, gets the current value of the property on a component.
		/// </summary>
		public override object GetValue(object component)
		{
			// We cache the settings to avoid paying the serialization cost too often.
			// Also, this allows us to track changes more consistently.
			if (this.settings == null)
			{
				var json = base.GetValue(component) as string;

				if (string.IsNullOrWhiteSpace(json))
				{
					this.settings = new PropertyBindingSettings { Name = base.Name };
					// Save the value right after instantiation, so that 
					// subsequent GetValue gets the same instance and 
					// does not re-create from scratch.
					SetValue(component, settings);
				}
				else
				{
					// Deserialize always to the concrete type, as we the serializer needs to 
					// know the type of thing to create.
					try
					{
						this.settings = BindingSerializer.Deserialize<PropertyBindingSettings>(json);
						// Make sure the settings property name matches the actual descriptor property name.
						if (this.settings.Name != base.Name)
							this.settings.Name = base.Name;
					}
					catch (BindingSerializationException)
					{
						// This would happen if the value was a raw value from before we had a binding.
						// Consider it the property value.
						settings = new PropertyBindingSettings
						{
							Name = base.Name,
							Value = json,
						};

						// Persist updated value.
						SetValue(component, settings);
					}
				}

				// Hookup property changed event, which supports nested changes as well. This 
				// allows us to automatically save the serialized json on every property change.
				this.settings.PropertyChanged += OnSettingsChanged;
			}

			// We know that we're always dealing with a concrete type implementation as it's not 
			// externally set-able and we always instantiate a PropertyBindingSettings.
			return new DesignProperty(this.settings)
			{
				Type = base.PropertyType,
				Attributes = this.AttributeArray
			};
		}

		/// <summary>
		/// When overridden in a derived class, sets the value of the component to a different value.
		/// </summary>
		public override void SetValue(object component, object value)
		{
			var stringValue = value as string;
			var settingsValue = value as IPropertyBindingSettings;

			// The setter will be called with a plain string value whenever the standard values editor is 
			// used to pick the TypeId. So we need to actually set that value on the binding instead.
			if (stringValue != null)
			{
				// Note that the setting of the property Value automatically triggers a property changed 
				// that will cause the value to be serialized and saved :). Indeed, 
				// it will be calling the SetValue again, but this time with the entire binding settings
				// so it will call the next code branch.
				((IPropertyBindingSettings)this.GetValue(component)).Value = stringValue;
			}
			else if (settingsValue != null)
			{
				// Someone is explicitly setting the entire binding value, so we have to serialize it straight.
				base.SetValue(component, BindingSerializer.Serialize(settingsValue));

				// If the previous value was non-null, then we'd be monitoring its property changes 
				// already, and we'd need to unsubscribe.
				if (this.settings != null)
				{
					this.settings.PropertyChanged -= OnSettingsChanged;
				}

				// Store for reuse on GetValue, and attach to property changes for auto-save.
				this.settings = settingsValue;
				this.settings.PropertyChanged += OnSettingsChanged;
			}
		}

		/// <summary>
		/// When overridden in a class, resets teh property value.
		/// </summary>
		public override void ResetValue(object component)
		{
			this.settings.Value = string.Empty;
			this.settings.ValueProvider = null;
		}

		private void OnSettingsChanged(object sender, EventArgs args)
		{
			// Automatically save the settings on change.
			SetValue(sender, this.settings);
		}
	}
}
