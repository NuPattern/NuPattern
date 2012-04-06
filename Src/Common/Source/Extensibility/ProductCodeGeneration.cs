using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Defines utility methods for use in templates that generate code from a pattern or pattern schema.
	/// </summary>
	[CLSCompliant(false)]
	public class ProductCodeGeneration<TInfo, TRuntime> : CodeGeneration
		where TInfo : IPatternElementInfo
		where TRuntime : IInstanceBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProductCodeGeneration{TInfo, TRuntime}"/> class.
		/// </summary>
		/// <param name="element">The element.</param>
		public ProductCodeGeneration(TInfo element)
		{
			Guard.NotNull(() => element, element);

			this.BuildTypeMap(element);
		}

		/// <summary>
		/// Builds a map of all the full type names that are used by the given 
		/// schema element and the final type name to use in templates.
		/// </summary>
		private void BuildTypeMap(TInfo element)
		{
			this.AddTypes(GetUsedTypesFromVariableProperties(element));
			this.AddUsedTypes(typeof(TInfo));
			this.AddUsedTypes(typeof(TRuntime));
			// Force ComponentModel and Drawing.Design to be there always.
			base.TypeNameMap[typeof(ArrayConverter).FullName] = typeof(ArrayConverter).FullName;
			base.TypeNameMap[typeof(UITypeEditor).FullName] = typeof(UITypeEditor).FullName;
		}

		/// <devdoc>
		/// Gets all the unique type names used by the element variable properties, 
		/// its reflection properties which are not hidden, as well as all the 
		/// used types in any of those property attributes.
		/// </devdoc>
		private static IEnumerable<string> GetUsedTypesFromVariableProperties(TInfo element)
		{
			Guard.NotNull(() => element, element);

			// The namespace of all used property types.
			var usedTypes = TypeNameFromProperty(element, prop => prop.Type);

			// Plus all if their type converter types
			usedTypes = usedTypes.Concat(TypeNameFromProperty(element, prop => prop.TypeConverterTypeName));

			// And all their type editor types
			usedTypes = usedTypes.Concat(TypeNameFromProperty(element, prop => prop.EditorTypeName));

			return usedTypes = usedTypes.Distinct();
		}

		private static IEnumerable<string> TypeNameFromProperty(TInfo element, Func<IPropertyInfo, string> getProperty)
		{
			return from property in element.Properties
				   let value = getProperty(property)
				   where !string.IsNullOrEmpty(value)
				   let trimmed = value.Trim()
				   where trimmed.Length > 0
				   select trimmed;
		}

		/// <summary>
		/// Determines if the customizable setting for the DefaultValue property of a customizable element is enabled.
		/// </summary>
		public bool IsPropertyDefaultValueCustomizationEnabled(IPatternElementInfo property)
		{
			Guard.NotNull(() => property, property);

			var defaultValueProperty = Reflector<IPropertySchema>.GetProperty(prop => prop.RawDefaultValue).Name;

			return property.Policy.Settings
				.Any(setting => setting.PropertyId == defaultValueProperty && setting.IsEnabled);
		}
	}
}
