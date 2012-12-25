using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NuPattern.Extensibility;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// The <see cref="PropertyDescriptor"/> for the <see cref="DesignProperty"/>.
	/// </summary>
	public class DesignPropertyDescriptor : PropertyDescriptor
	{
		private const string DefaultCategory = "Misc";

		private Type componentType;
		private Type propertyType;
		private string category;
		private string displayName;
		private string description;
		private Attribute[] valuePropertyAttributes;

		/// <summary>
		/// Initializes a new instance of the <see cref="DesignPropertyDescriptor"/> class.
		/// </summary>
		/// <param name="name">The name of the property.</param>
		/// <param name="displayName">The display name.</param>
		/// <param name="description">The description</param>
		/// <param name="category">The category.</param>
		/// <param name="valueType">Type of the value.</param>
		/// <param name="componentType">Type of the component.</param>
		/// <param name="valuePropertyAttributes">The value property attributes.</param>
		public DesignPropertyDescriptor(
			string name,
			string displayName,
			string description,
			string category,
			Type valueType,
			Type componentType,
			Attribute[] valuePropertyAttributes)
			: base(name, null)
		{
			this.displayName = displayName;
			this.description = description;
			this.componentType = componentType;
			this.propertyType = Type.GetType(valueType.AssemblyQualifiedName) ?? TypeDescriptor.GetProvider(valueType).GetRuntimeType(valueType);
			this.valuePropertyAttributes = valuePropertyAttributes == null ? new Attribute[0] : valuePropertyAttributes;
			this.category = string.IsNullOrEmpty(category) ? DefaultCategory : category;
		}

		/// <summary>
		/// Gets the name that can be displayed in a window, such as a Properties window.
		/// </summary>
		/// <value></value>
		/// <returns>The name to display for the member.</returns>
		public override string DisplayName
		{
			get
			{
				var displayNameAttr = this.valuePropertyAttributes.OfType<DisplayNameAttribute>().FirstOrDefault() as DisplayNameAttribute;

				if (displayNameAttr != null)
				{
					return displayNameAttr.DisplayName;
				}

				if (!string.IsNullOrEmpty(this.displayName))
				{
					return this.displayName;
				}

				return base.DisplayName;
			}
		}

		/// <summary>
		/// Gets the description of the member, as specified in the <see cref="T:System.ComponentModel.DescriptionAttribute"/>.
		/// </summary>
		/// <value></value>
		/// <returns>The description of the member. If there is no <see cref="T:System.ComponentModel.DescriptionAttribute"/>, the property value is set to the default, which is an empty string ("").</returns>
		public override string Description
		{
			get
			{
				var descriptionAttr = this.valuePropertyAttributes.OfType<DescriptionAttribute>().FirstOrDefault() as DescriptionAttribute;

				if (descriptionAttr != null)
				{
					return descriptionAttr.Description;
				}

				if (!string.IsNullOrEmpty(this.description))
				{
					return this.description;
				}

				return base.Description;
			}
		}

		/// <summary>
		/// Gets the name of the category to which the member belongs, as specified in the <see cref="T:System.ComponentModel.CategoryAttribute"/>.
		/// </summary>
		/// <value></value>
		/// <returns>The name of the category to which the member belongs. If there is no <see cref="T:System.ComponentModel.CategoryAttribute"/>, the category name is set to the default category, Misc.</returns>
		/// <PermissionSet>
		/// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
		/// </PermissionSet>
		public override string Category
		{
			get
			{
				if (base.Category == DefaultCategory || string.IsNullOrEmpty(base.Category))
				{
					return this.category;
				}

				return base.Category;
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets the type of the component this property is bound to.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
		public override Type ComponentType
		{
			get { return this.componentType; }
		}

		/// <summary>
		/// Gets a value indicating whether value change notifications for this property may originate from outside the property descriptor.
		/// </summary>
		/// <value></value>
		/// <returns>true if value change notifications may originate from outside the property descriptor; otherwise, false.</returns>
		public override bool SupportsChangeEvents
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the member is browsable, as specified in the <see cref="T:System.ComponentModel.BrowsableAttribute"/>.
		/// </summary>
		public override bool IsBrowsable
		{
			get
			{
				var browsable = this.valuePropertyAttributes.OfType<BrowsableAttribute>().Select(x => x.Browsable);
				return !browsable.Any() || browsable.First() == true;
			}
		}

		/// <summary>
		/// When overridden in a derived class, gets a value indicating whether this property is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the property is read-only; otherwise, false.</returns>
		public override bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// When overridden in a derived class, gets the type of the property.
		/// </summary>
		/// <value></value>
		/// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
		public override Type PropertyType
		{
			get { return typeof(DesignProperty); }
		}

		/// <summary>
		/// When overridden in a derived class, returns whether resetting an object changes its value.
		/// </summary>
		/// <param name="component">The component to test for reset capability.</param>
		/// <returns>
		/// Returns true if resetting the component changes its value; otherwise, false.
		/// </returns>
		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// When overridden in a derived class, gets the current value of the property on a component.
		/// </summary>
		/// <param name="component">The component with the property for which to retrieve the value.</param>
		/// <returns>
		/// The value of a property for a given component.
		/// </returns>
		public override object GetValue(object component)
		{
			var modelElement = GetModelElement(component);

			var element = modelElement as CommandSettings;
			var provider = component as DesignValueProvider;
			if (modelElement == null && provider != null)
			{
				element = provider.DesignProperty.ModelProperty.CommandSettings;
			}

			var modelProperty = element.Properties.FirstOrDefault(prop => prop.Name == this.Name);
			if (modelProperty == null)
			{
				using (var tx = element.Store.TransactionManager.BeginTransaction("Creating Property"))
				{
					modelProperty = element.Create<PropertySettings>();
					modelProperty.Name = this.Name;

					if (provider != null)
					{
						provider.ValueProvider.Properties.Add(modelProperty);
					}

					if (this.propertyType.IsValueType)
					{
						// We need to provide a default value
						modelProperty.Value = Activator.CreateInstance(this.propertyType).ToString();
					}

					tx.Commit();
				}
			}

			var designProperty = new DesignProperty(modelProperty)
			{
				Type = this.propertyType,
				Attributes = this.valuePropertyAttributes,
			};

			return designProperty;
		}

		/// <summary>
		/// When overridden in a derived class, sets the value of the component to a different value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be set.</param>
		/// <param name="value">The new value.</param>
		public override void SetValue(object component, object value)
		{
		}

		/// <summary>
		/// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
		/// </summary>
		/// <param name="component">The component with the property to be examined for persistence.</param>
		/// <returns>
		/// Returns true if the property should be persisted; otherwise, false.
		/// </returns>
		public override bool ShouldSerializeValue(object component)
		{
			return true;
		}

		/// <summary>
		/// When overridden in a derived class, resets the value for this property of the component to the default value.
		/// </summary>
		/// <param name="component">The component with the property value that is to be reset to the default value.</param>
		public override void ResetValue(object component)
		{
		}

		/// <summary>
		/// Gets or sets the attributes of this property as an array.
		/// </summary>
		protected override Attribute[] AttributeArray
		{
			get
			{
				return this.valuePropertyAttributes;
			}
			set
			{
				this.valuePropertyAttributes = value;
			}
		}

		/// <summary>
		/// Gets the collection of attributes for this member.
		/// </summary>
		public override AttributeCollection Attributes
		{
			get
			{
				return new AttributeCollection(this.valuePropertyAttributes);
			}
		}

		private static ModelElement GetModelElement(object component)
		{
			var shape = component as ShapeElement;

			return shape != null ? shape.ModelElement as ModelElement : component as ModelElement;
		}
	}
}