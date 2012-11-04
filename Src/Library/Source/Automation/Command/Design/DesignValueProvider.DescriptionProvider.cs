using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Defines a type description provider over <see cref="DesignValueProvider"/>.
	/// </summary>
	internal class DesignValueProviderTypeDescriptionProvider : TypeDescriptionProvider
	{
		private static TypeDescriptionProvider parent =
				TypeDescriptor.GetProvider(typeof(DesignValueProvider));

		/// <summary>
		/// Initializes a new instance of the <see cref="DesignValueProviderTypeDescriptionProvider"/> class.
		/// </summary>
		public DesignValueProviderTypeDescriptionProvider()
			: base(parent)
		{
		}

		/// <summary>
		/// Gets a custom type descriptor for the given type and object.
		/// </summary>
		/// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
		/// <param name="instance">An instance of the type. Can be null if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor"/>.</param>
		/// <returns>
		/// An <see cref="T:System.ComponentModel.ICustomTypeDescriptor"/> that can provide metadata for the type.
		/// </returns>
		public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			return new DesignValueProviderTypeDescriptor(parent.GetTypeDescriptor(objectType, instance), instance as DesignValueProvider);
		}

		/// <summary>
		/// Defines a type descriptor over <see cref="DesignValueProvider"/>.
		/// </summary>
		private class DesignValueProviderTypeDescriptor : CustomTypeDescriptor
		{
			private static IEnumerable<Lazy<IValueProvider, IFeatureComponentMetadata>> ValueProviders { get; set; }
			private static IPlatuProjectTypeProvider ProjectTypeProvider { get; set; }

			private DesignValueProvider designValueProvider;
			private ICustomTypeDescriptor parent;

			/// <summary>
			/// Initializes a new instance of the <see cref="DesignValueProviderTypeDescriptor"/> class.
			/// </summary>
			/// <param name="parent">The parent.</param>
			/// <param name="valueProvider">The value provider.</param>
			public DesignValueProviderTypeDescriptor(ICustomTypeDescriptor parent, DesignValueProvider valueProvider)
				: base(parent)
			{
				this.parent = parent;
				this.designValueProvider = valueProvider;
			}

			/// <summary>
			/// Returns a collection of property descriptors for the object represented by this type descriptor.
			/// </summary>
			/// <returns>
			/// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
			/// </returns>
			public override PropertyDescriptorCollection GetProperties()
			{
				return this.GetProperties(new Attribute[0]);
			}

			/// <summary>
			/// Returns a filtered collection of property descriptors for the object represented by this type descriptor.
			/// </summary>
			/// <param name="attributes">An array of attributes to use as a filter. This can be null.</param>
			/// <returns>
			/// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> containing the property descriptions for the object represented by this type descriptor. The default is <see cref="F:System.ComponentModel.PropertyDescriptorCollection.Empty"/>.
			/// </returns>
			public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
			{
				var properties = this.parent.GetProperties(attributes).Cast<PropertyDescriptor>().ToList();

				if (attributes != null)
				{
					properties.AddRange(base.GetProperties(attributes).Cast<PropertyDescriptor>());
				}
				else
				{
					properties.AddRange(base.GetProperties().Cast<PropertyDescriptor>());
				}

				if (ProjectTypeProvider == null || ValueProviders == null)
				{
					ProjectTypeProvider = FeatureCompositionService.Instance.GetExportedValue<IPlatuProjectTypeProvider>();
					ValueProviders = FeatureCompositionService.Instance
						.GetExports<IValueProvider, IFeatureComponentMetadata>()
						.FromFeaturesCatalog()
						.Where(value => value.Metadata.ExportingType.IsBrowsable());
				}

				var valueProviderType = ValueProviders
					.FromFeaturesCatalog()
					.Where(binding => binding.Metadata.Id == this.designValueProvider.Name)
					.Select(binding => binding.Metadata.ExportingType)
					.FirstOrDefault();

				if (valueProviderType == null && !string.IsNullOrEmpty(this.designValueProvider.Name))
				{
					valueProviderType = (from type in ProjectTypeProvider.GetTypes<IValueProvider>()
										 let meta = type.AsProjectFeatureComponent()
										 where meta != null && meta.Id == this.designValueProvider.Name
										 select type)
										.FirstOrDefault();
				}

				if (valueProviderType != null)
				{
					foreach (var descriptor in TypeDescriptor.GetProperties(valueProviderType).Cast<PropertyDescriptor>().Where(d => d.IsBindableProperty()))
					{
						properties.Add(
							new DesignPropertyDescriptor(
								this.GetPropertyName(this.designValueProvider, descriptor.Name),
								descriptor.Name,
								string.Empty,
								string.Empty,
								descriptor.PropertyType,
								typeof(DesignValueProvider),
								descriptor.Attributes.Cast<Attribute>().ToArray()));
					}
				}

				return new PropertyDescriptorCollection(properties.ToArray());
			}

			private string GetPropertyName(DesignValueProvider valueProvider, string propertyName)
			{
				if (valueProvider == null)
				{
					return propertyName;
				}

				DesignValueProvider parentProvider = null;

				if (valueProvider.DesignProperty.ModelProperty.ParentProvider != null)
				{
					parentProvider = new DesignValueProvider(
						new DesignProperty(valueProvider.DesignProperty.ModelProperty.ParentProvider.OwnerProperty),
						valueProvider.DesignProperty.ModelProperty.ParentProvider);
				}

				return valueProvider.DesignProperty.ModelProperty.Name + "." + valueProvider.Name + "." + this.GetPropertyName(parentProvider, propertyName);
			}
		}
	}
}