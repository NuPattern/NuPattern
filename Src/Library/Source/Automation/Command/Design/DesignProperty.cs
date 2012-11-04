using System;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Defines a wrapper over <see cref="PropertySettings"/> to be used in the property grid.
	/// </summary>
	[TypeDescriptionProvider(typeof(DesignPropertyTypeDescriptionProvider))]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DesignProperty
	{
		private Lazy<TypeConverter> valueConverter;
		private Type type;

		/// <summary>
		/// Initializes a new instance of the <see cref="DesignProperty"/> class.
		/// </summary>
		/// <param name="modelProperty">The model property.</param>
		public DesignProperty(PropertySettings modelProperty)
		{
			this.ModelProperty = modelProperty;
			this.valueConverter = new Lazy<TypeConverter>(() =>
			{
				var converter = this.Attributes.FindCustomTypeConverter();
				if (converter == null ||
					// The custom converter must be capable of converting to AND from string 
					// for proper interaction with the property grid.
					!converter.CanConvertFrom(typeof(string)) ||
					!converter.CanConvertTo(typeof(string)))
				{
					converter = TypeDescriptor.GetConverter(this.Type);
				}
				return converter;
			});
		}

		/// <summary>
		/// Gets or sets the design value provider.
		/// </summary>
		/// <value>The design value provider.</value>
		[TypeConverter(typeof(DesignValueProviderTypeConverter))]
		[Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
		[DisplayNameResource("DesignValueProvider_ValueProviderDisplayName", typeof(Resources))]
		[DescriptionResource("DesignValueProvider_ValueProviderDescription", typeof(Resources))]
		public DesignValueProvider DesignValueProvider
		{
			get
			{
				return this.ModelProperty.ValueProvider != null ? new DesignValueProvider(this, this.ModelProperty.ValueProvider) : null;
			}

			set
			{
				using (var transaction = this.ModelProperty.Store.TransactionManager.BeginTransaction("Setting Value Provider"))
				{
					if (value != null)
					{
						if (this.ModelProperty.ValueProvider == null)
						{
							this.ModelProperty.Create<ValueProviderSettings>();
						}

						this.ModelProperty.ValueProvider.TypeId = value.Name;
					}
					else if (this.ModelProperty.ValueProvider != null)
					{
						this.ModelProperty.ValueProvider.Delete();
					}

					transaction.Commit();
				}
			}
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		[Browsable(false)]
		public object Value
		{
			get
			{
				var context = new SimpleTypeDescriptorContext { Instance = this.ModelProperty, ServiceProvider = this.ModelProperty.CommandSettings.Store };
				return this.valueConverter.Value.ConvertFromString(context, this.ModelProperty.Value);
			}
			set
			{
				var context = new SimpleTypeDescriptorContext { Instance = this.ModelProperty, ServiceProvider = this.ModelProperty.CommandSettings.Store };
				this.ModelProperty.Value = this.valueConverter.Value.ConvertToString(context, value);
			}
		}

		/// <summary>
		/// Gets the model property.
		/// </summary>
		/// <value>The model property.</value>
		internal PropertySettings ModelProperty { get; private set; }

		/// <summary>
		/// Gets or sets the attributes.
		/// </summary>
		/// <value>The attributes.</value>
		internal Attribute[] Attributes { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		internal Type Type
		{
			get { return this.type; }
			set { this.type = Type.GetType(value.AssemblyQualifiedName) ?? TypeDescriptor.GetProvider(value).GetRuntimeType(value); }
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return this.ModelProperty.Value;
		}
	}
}