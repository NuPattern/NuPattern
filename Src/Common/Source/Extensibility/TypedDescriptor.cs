using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Provides a strong-typed alternative to <see cref="TypeDescriptor"/>.
	/// </summary>
	public static class TypedDescriptor
	{
		/// <summary>
		/// Retrieves the named descriptor of the given target instance if available.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "NotApplicable")]
		public static PropertyDescriptor GetProperty<T, TResult>(this T target, Expression<Func<T, TResult>> property)
		{
			var propertyName = Reflector<T>.GetPropertyName(property);

			return TypeDescriptor.GetProperties(target)[propertyName];
		}

		/// <summary>
		/// Creates a descriptor of the property of the given target instance if available.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "NotApplicable")]
		public static PropertyDescriptor CreateProperty<T, TResult>(this T target, Expression<Func<T, TResult>> property, params Attribute[] attributes)
		{
			var descriptor = Reflector<T>.GetProperty(property);

			return TypeDescriptor.CreateProperty(target.GetType(), descriptor.Name, descriptor.PropertyType, attributes);
		}

		/// <summary>
		/// Replaces the found descriptor with the given descriptor.
		/// </summary>
		/// <param name="properties">The properties collection.</param>
		/// <param name="property">The named property to replace.</param>
		/// <param name="replacementDescriptor">The descriptor to replace the found one.</param>
		public static void ReplaceDescriptor<TTarget, TProperty>(this ICollection<PropertyDescriptor> properties,
			Expression<Func<TTarget, TProperty>> property, Func<PropertyDescriptor, PropertyDescriptor> replacementDescriptor)
		{
			Guard.NotNull(() => properties, properties);
			Guard.NotNull(() => property, property);
			Guard.NotNull(() => replacementDescriptor, replacementDescriptor);

			//Find existing descriptor
			var propertyName = Reflector<TTarget>.GetPropertyName(property);
			var descriptor = properties.FirstOrDefault(d => d.Name.Equals(propertyName, StringComparison.Ordinal));

			if (descriptor != null)
			{
				//Replace the descriptor
				properties.Remove(descriptor);
				properties.Add(replacementDescriptor(descriptor));
			}
		}
	}
}