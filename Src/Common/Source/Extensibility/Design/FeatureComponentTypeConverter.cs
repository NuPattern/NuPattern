using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Design
{
	/// <summary>
	/// Defines a generic type converter.
	/// </summary>
	[CLSCompliant(false)]
	public class FeatureComponentTypeConverter<T> : TypeConverter where T : class
	{
		private static IEnumerable<Lazy<T, IFeatureComponentMetadata>> components;
		private static INuPatternProjectTypeProvider projectTypes;

		/// <summary>
		/// Components for the <see cref="FeatureComponentTypeConverter{T}"/>
		/// </summary>
		[ImportMany]
		protected IEnumerable<Lazy<T, IFeatureComponentMetadata>> Components
		{
			get
			{
				if (components == null)
					FeatureCompositionService.Instance.SatisfyImportsOnce(this);

				return components;
			}
			set
			{
				components = value;
			}
		}

		/// <summary>
		/// Reference to an <see cref="INuPatternProjectTypeProvider"/>
		/// </summary>
		[Import]
		protected INuPatternProjectTypeProvider ProjectTypeProvider
		{
			get
			{
				if (projectTypes == null)
					FeatureCompositionService.Instance.SatisfyImportsOnce(this);

				return projectTypes;
			}
			set
			{
				projectTypes = value;
			}
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
		/// Returns whether the collection of standard values returned from <see cref="TypeConverter.GetStandardValues()"/> is an exclusive list of possible values, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
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
			var metadataComparer = new MetadataEqualityComparer();

			var types = this.ProjectTypeProvider
				.GetTypes<T>()
				.Where(type => !type.IsAbstract)
				.Select(type => type.AsProjectFeatureComponent());

			var components = this.Components
				.FromFeaturesCatalog()
				.Where(condition => !condition.Metadata.ExportingType.IsAbstract && condition.Metadata.ExportingType.IsBrowsable())
				.Select(component => component.Metadata);

			var values = components
				.Concat(types)
				.Where(metadata => metadata != null)
				.Distinct(metadataComparer)
				.Select(metadata => metadata.AsStandardValue())
				.ToArray();

			return new StandardValuesCollection(values);
		}
	}
}