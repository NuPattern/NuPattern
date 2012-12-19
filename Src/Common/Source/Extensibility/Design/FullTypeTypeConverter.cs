using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
	/// <summary>
	/// Defines a <see cref="TypeConverter"/> that provides standard values for types accessible from 
	/// the current project that are assignable to the given type parameter <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// Does not show non-public, abstract, generic, non-browsable or nested types.
	/// </remarks>
	/// <typeparam name="T">The types to add as <see cref="TypeConverter.StandardValuesCollection"/>.</typeparam>
	public class FullTypeTypeConverter<T> : StringConverter
	{
		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list, using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="ITypeDescriptorContext"/> that provides a format context.</param>
		/// <returns>
		/// true if <see cref="GetStandardValues"/> should be called to find a common set of values the object supports; otherwise, false.
		/// </returns>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
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
			using (new MouseCursor(Cursors.Wait))
			{
				var solution = context.GetService<ISolution>();
				var selection = solution.GetSelectedItems<IItemContainer>().FirstOrDefault();
				var project = selection as IProject;
				if (project == null)
				{
					project = (IProject)selection.Traverse(s => s.Parent, s => s.Kind == ItemKind.Project);
				}

				if (project == null)
				{
					return new StandardValuesCollection(new List<StandardValue>());
				}

				var values = project.GetAvailableTypes(typeof(T))
					.Where(t => !t.IsAbstract && t.IsPublic && !t.IsGenericTypeDefinition && t.IsBrowsable() && !t.IsNested)
					.Select(t => CreateStandardValue(t))
					.OrderBy(t => t.DisplayText)
					.ToArray();

				return new StandardValuesCollection(values);
			}
		}

		private static StandardValue CreateStandardValue(Type type)
		{
            var displayNameAttribute = ReflectionExtensions.GetCustomAttribute<DisplayNameAttribute>(type, false);
            var descriptionAttribute = ReflectionExtensions.GetCustomAttribute<DescriptionAttribute>(type, false);
            var categoryAttribute = ReflectionExtensions.GetCustomAttribute<CategoryAttribute>(type, false);

			return new StandardValue(
				displayNameAttribute != null ? displayNameAttribute.DisplayName : type.FullName,
				type.FullName + ", " + type.Assembly.FullName.Split(',')[0],
				descriptionAttribute != null ? descriptionAttribute.Description : string.Empty,
				categoryAttribute != null ? categoryAttribute.Category : string.Empty);
		}
	}
}