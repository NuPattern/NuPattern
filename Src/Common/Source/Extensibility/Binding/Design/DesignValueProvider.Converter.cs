using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;

namespace NuPattern.Extensibility.Binding
{
	/// <summary>
	/// Defines a type converter over DesignValueProvider.
	/// </summary>
	internal class DesignValueProviderTypeConverter : ExpandableObjectConverter
	{
		private static IEnumerable<Lazy<IValueProvider, IFeatureComponentMetadata>> ValueProviders { get; set; }
		private static IPlatuProjectTypeProvider ProjectTypeProvider { get; set; }

		public DesignValueProviderTypeConverter()
		{

		}

		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		/// <summary>
		/// Returns whether the collection of standard values returned from <see cref="M:System.ComponentModel.TypeConverter.GetStandardValues"/> is an exclusive list of possible values, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">A <see cref="CultureInfo"/>. If null is passed, the current culture is assumed.</param>
		/// <param name="value">The <see cref="object"/> to convert.</param>
		/// <param name="destinationType">The <see cref="Type"/> to convert the <paramref name="value"/> parameter to.</param>
		/// <returns>
		/// An <see cref="object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
		/// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var componentMetadata = value as IFeatureComponentMetadata;
				if (componentMetadata != null)
				{
					return componentMetadata.Id;
				}

				var valueProvider = value as DesignValueProvider;
				if (valueProvider != null)
				{
					return valueProvider.Name;
				}

				return string.Empty;
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}

		/// <summary>
		/// Returns whether this converter can convert the object to the specified type, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) ? true : base.CanConvertTo(context, destinationType);
		}

		/// <summary>
		/// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		/// <summary>
		/// Converts the given object to the type of this converter, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
		/// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
		/// <returns>
		/// An <see cref="object"/> that represents the converted value.
		/// </returns>
		/// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			var typeId = value as string;
			if (context == null || string.IsNullOrEmpty(typeId))
				return null;

			var designProperty = (DesignProperty)context.Instance;
			if (designProperty.ValueProvider == null ||
				designProperty.ValueProvider.ValueProvider.TypeId != typeId)
			{
				designProperty.ValueProvider = new DesignValueProvider(context.Instance as DesignProperty, new ValueProviderBindingSettings { TypeId = typeId });
			}

			return designProperty.ValueProvider;
		}

		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for when provided with a format context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context that can be used to extract additional information about the environment from which this converter is invoked. This parameter or properties of this parameter can be null.</param>
		/// <returns>
		/// A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of valid values, or null if the data type does not support a standard set of values.
		/// </returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (ProjectTypeProvider == null || ValueProviders == null)
			{
				ProjectTypeProvider = FeatureCompositionService.Instance.GetExportedValue<IPlatuProjectTypeProvider>();
				ValueProviders = FeatureCompositionService.Instance
					.GetExports<IValueProvider, IFeatureComponentMetadata>()
					.FromFeaturesCatalog()
					.Where(value => value.Metadata.ExportingType.IsBrowsable());
			}

			return this.StandardValues;
		}

		private StandardValuesCollection standardValues;

		private StandardValuesCollection StandardValues
		{
			get
			{
				if (standardValues == null)
				{
					var metadataComparer = new MetadataEqualityComparer();
					var types = ProjectTypeProvider
						.GetTypes<IValueProvider>()
						.Where(type => !type.IsAbstract)
						.Select(type => type.AsProjectFeatureComponent());

					var values = ValueProviders
						.Select(value => value.Metadata)
						.Concat(types)
						.Where(metadata => metadata != null)
						.Distinct(metadataComparer)
						.Select(metadata => metadata.AsStandardValue())
						.ToArray();

					this.standardValues = new StandardValuesCollection(values);
				}

				return standardValues;
			}
		}
	}
}