using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.Interfaces;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// A base class for Reference Type Providers, that provides easy means to manage references of the given type.
	/// </summary>
	public abstract class ReferenceKindProvider<TKind, TValue> : IReferenceKindProvider<TValue>
	{
		private static readonly string typeIdFromKind = typeof(TKind).FullName;

		/// <summary>
		/// Returns the first <see cref="IReference"/> for the current element.
		/// </summary>
		public static TValue GetReference(IProductElement element)
		{
			Guard.NotNull(() => element, element);

			var value = element.TryGetReference(typeIdFromKind);
			if (value == null)
			{
				return default(TValue);
			}

			//Convert value to type of this reference
			return GetValueOfKindType(new ElementDescriptorContext(element), value);
		}

		/// <summary>
		/// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
		/// </summary>
		public static IEnumerable<TValue> GetReferences(IProductElement element)
		{
			return GetReferences(element, r => true);
		}

		/// <summary>
		/// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
		/// </summary>
		public static IEnumerable<TValue> GetReferences(IProductElement element, Func<IReference, bool> whereFilter)
		{
			Guard.NotNull(() => element, element);
			Guard.NotNull(() => whereFilter, whereFilter);

			return element.References
				.Where(r => r.Kind == typeIdFromKind && !string.IsNullOrEmpty(r.Value))
				.Where(whereFilter)
				.Select(r => GetValueOfKindType(new ElementDescriptorContext(element), r.Value));
		}

		/// <summary>
		/// Adds a new <see cref="IReference"/> to the References of this element.
		/// </summary>
		public static IReference AddReference(IProductElement element, TValue value)
		{
			Guard.NotNull(() => element, element);
			Guard.NotNull(() => value, value);

			return element.AddReference(typeIdFromKind, GetStringValue(new ElementDescriptorContext(element), value));
		}

		/// <summary>
		/// Sets an existing <see cref="IReference"/> in the References of this element, or creates if not exist.
		/// </summary>
		public static IReference SetReference(IProductElement element, TValue value)
		{
			Guard.NotNull(() => element, element);
			Guard.NotNull(() => value, value);

			return element.AddReference(typeIdFromKind, GetStringValue(new ElementDescriptorContext(element), value), true);
		}

		/// <summary>
		/// Sets the given <paramref name="reference"/> value to the given typed value.
		/// </summary>
		public static void SetReference(IReference reference, TValue value)
		{
			Guard.NotNull(() => reference, reference);
			Guard.NotNull(() => value, value);

			reference.Value = GetStringValue(new ElementDescriptorContext(reference.Owner), value);
		}

		/// <summary>
		/// Converts the reference value to the strong typed value supported by this provider.
		/// </summary>
		public static TValue FromReference(IReference reference)
		{
			Guard.NotNull(() => reference, reference);

			if (reference.Kind != typeIdFromKind)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.ReferenceKindProvider_UnsupportedReferenceKind,
					reference.Kind,
					typeIdFromKind));

			return GetValueOfKindType(new ElementDescriptorContext(reference.Owner), reference.Value);
		}

		/// <summary>
		/// Returns the provided kind value from the provider's type converter.
		/// </summary>
		private static TValue GetValueOfKindType(ITypeDescriptorContext context, string value)
		{
			var kindConverter = TypeDescriptor.GetConverter(typeof(TKind));
			var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

			if (!kindConverter.CanConvertFrom(context, typeof(string)) && !valueConverter.CanConvertFrom(context, typeof(string)))
			{
				throw new NotSupportedException(Resources.ReferenceKindProvider_CanNotConvertFromString);
			}

			var converter = CanConvertString(context, kindConverter) ? kindConverter : valueConverter;

			var convertedValue = converter.ConvertFromString(context, value);

			if (convertedValue != null &&
				!typeof(TValue).IsAssignableFrom(convertedValue.GetType()))
			{
				if (converter.CanConvertTo(context, typeof(TValue)))
				{
					return (TValue)converter.ConvertTo(context, CultureInfo.CurrentCulture, convertedValue, typeof(TValue));
				}
				else
				{
					throw new NotSupportedException(Resources.ReferenceKindProvider_CanNotConvertFromString);
				}
			}

			return (TValue)convertedValue;
		}

		/// <summary>
		/// Returns the string value from the provider's type converter of its Kind.
		/// </summary>
		private static string GetStringValue(ITypeDescriptorContext context, TValue value)
		{
			var kindConverter = TypeDescriptor.GetConverter(typeof(TKind));
			var valueConverter = TypeDescriptor.GetConverter(typeof(TValue));

			if (!CanConvertString(context, kindConverter) && !CanConvertString(context, valueConverter))
			{
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.ReferenceKindProvider_CanNotConvertString,
					typeof(TKind).Name,
					typeof(TValue).Name));
			}

			var converter = CanConvertString(context, kindConverter) ? kindConverter : valueConverter;

			return converter.ConvertToString(context, value);
		}

		private static bool CanConvertString(ITypeDescriptorContext context, TypeConverter kindConverter)
		{
			return kindConverter.CanConvertTo(context, typeof(string)) && kindConverter.CanConvertFrom(context, typeof(string));
		}

		private class ElementDescriptorContext : ITypeDescriptorContext
		{
			Func<Type, object> getService;

			public ElementDescriptorContext(IProductElement element)
			{
				this.Element = element;
				if (this.Element.Root != null && this.Element.Root.ProductState != null)
					this.getService = serviceType => this.Element.Root.ProductState.GetService(serviceType);
				else
					this.getService = serviceType => null;
			}

			public IProductElement Element { get; private set; }

			public IContainer Container
			{
				get { throw new NotImplementedException(); }
			}

			public object Instance
			{
				get { return this.Element; }
			}

			public void OnComponentChanged()
			{
			}

			public bool OnComponentChanging()
			{
				return true;
			}

			public PropertyDescriptor PropertyDescriptor
			{
				get { throw new NotImplementedException(); }
			}

			public object GetService(Type serviceType)
			{
				return this.getService(serviceType);
			}
		}
	}
}