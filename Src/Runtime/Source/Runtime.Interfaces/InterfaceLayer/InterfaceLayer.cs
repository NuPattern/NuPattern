using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualStudio.Patterning.Runtime.Interfaces;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Provides extension methods to <see cref="IProduct"/>, <see cref="IView"/> and 
	/// <see cref="IAbstractElement"/> to create strong typed proxies.
	/// </summary>
	[CLSCompliant(false)]
	public static class InterfaceLayer
	{
		/// <summary>
		/// Gets a strong typed interface layer for a product.
		/// </summary>
		public static TInterface As<TInterface>(this IProduct product)
		{
			return CreateProxy<TInterface, IProduct>(product);
		}

		/// <summary>
		/// Gets a strong typed interface layer for a view.
		/// </summary>
		public static TInterface As<TInterface>(this IView view)
		{
			return CreateProxy<TInterface, IView>(view);
		}

		/// <summary>
		/// Gets a strong typed interface layer for a collection or element.
		/// </summary>
		public static TInterface As<TInterface>(this IAbstractElement element)
		{
			return CreateProxy<TInterface, IAbstractElement>(element);
		}

		/// <summary>
		/// Gets a proxy accessor for a product targetting the given <typeparamref name="TInterface"/>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IProductProxy<TInterface> ProxyFor<TInterface>(this IProduct product)
		{
			return new ProductProxy<TInterface>(product);
		}

		/// <summary>
		/// Gets a proxy accessor for a view targetting the given <typeparamref name="TInterface"/>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IContainerProxy<TInterface> ProxyFor<TInterface>(this IView view)
		{
			return new ContainerProxy<TInterface>(view);
		}

		/// <summary>
		/// Gets a proxy accessor for an abstract element targetting the given <typeparamref name="TInterface"/>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IAbstractElementProxy<TInterface> ProxyFor<TInterface>(this IAbstractElement element)
		{
			return new AbstractElementProxy<TInterface>(element);
		}

		internal static TProperty GetValue<TInterface, TProperty>(IProductElement target, Expression<Func<TProperty>> propertyExpresion)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);

			var member = GetExpressionPropertyOrThrow<TProperty>(target, propertyExpresion);
			var property = GetInterfacePropertyOrThrow<TInterface>(member);

			var variableProperty = target.Properties.FirstOrDefault(prop => prop.DefinitionName == property.Name);
			if (variableProperty != null)
				return (TProperty)property.Converter.ConvertFromString(variableProperty.Value);

			var elementProperty = TypeDescriptor.GetProperties(target).Find(property.Name, false);
			if (elementProperty == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_InstancePropertyNotFound,
					target.GetType().Name,
					property.Name));

			// We don't do type conversion from element (reflection) properties as they will already be typed accesses.
			return (TProperty)elementProperty.GetValue(target);
		}

		internal static void SetValue<TInterface, TProperty>(IProductElement target, Expression<Func<TProperty>> propertyExpresion, TProperty value)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);

			var member = GetExpressionPropertyOrThrow<TProperty>(target, propertyExpresion);
			var property = GetInterfacePropertyOrThrow<TInterface>(member);

			var variableProperty = target.Properties.FirstOrDefault(prop => prop.DefinitionName == property.Name);
			if (variableProperty != null)
			{
				variableProperty.Value = property.Converter.ConvertToString(value);
			}
			else
			{
				var elementProperty = TypeDescriptor.GetProperties(target).Find(property.Name, false);
				if (elementProperty == null)
					throw new NotSupportedException(string.Format(
						CultureInfo.CurrentCulture,
						Resources.InterfaceLayer_InstancePropertyNotFound,
						target.GetType().Name,
						property.Name));

				// We don't do type conversion from element (reflection) properties as they will already be typed accesses.
				elementProperty.SetValue(target, value);
			}
		}

		internal static TView GetView<TInterface, TView>(IProduct target, Expression<Func<TView>> propertyExpresion, Func<IView, TView> viewProxyFactory)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);
			Guard.NotNull(() => viewProxyFactory, viewProxyFactory);

			var member = GetExpressionPropertyOrThrow(target, propertyExpresion);
			var property = GetInterfacePropertyOrThrow<TInterface>(member);

			var view = target.Views.FirstOrDefault(v => v.DefinitionName == property.Name);
			if (view == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_ViewNotFound,
					property.Name));

			return viewProxyFactory(view);
		}

		internal static TInterface GetElement<TInterface>(IElementContainer target, Expression<Func<TInterface>> propertyExpresion, Func<IAbstractElement, TInterface> elementProxyFactory)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);
			Guard.NotNull(() => elementProxyFactory, elementProxyFactory);

			var property = GetExpressionPropertyOrThrow(target, propertyExpresion);
			var definitionName = GetDesignerOrThrow(property.PropertyType);

			return target.Elements
				.Where(element => element.DefinitionName == definitionName)
				.Select(element => elementProxyFactory(element))
				.FirstOrDefault();
		}

		internal static IEnumerable<TInterface> GetElements<TInterface>(IElementContainer target, Expression<Func<IEnumerable<TInterface>>> propertyExpresion, Func<IAbstractElement, TInterface> elementProxyFactory)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);
			Guard.NotNull(() => elementProxyFactory, elementProxyFactory);

			var property = GetExpressionPropertyOrThrow(target, propertyExpresion);
			var definitionName = GetDesignerOrThrow(property.PropertyType.GetGenericArguments()[0]);

			return target.Elements
				.Where(element => element.DefinitionName == definitionName)
				.Select(element => elementProxyFactory(element));
		}

		internal static TInterface GetExtension<TInterface>(IElementContainer target, Expression<Func<TInterface>> propertyExpresion, Func<IProduct, TInterface> productProxyFactory)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);
			Guard.NotNull(() => productProxyFactory, productProxyFactory);

			var property = GetExpressionPropertyOrThrow(target, propertyExpresion);
			var definitionName = GetDesignerOrThrow(property.PropertyType);

			return target.Extensions
				.Where(element => element.DefinitionName == definitionName)
				.Select(element => productProxyFactory(element))
				.FirstOrDefault();
		}

		internal static IEnumerable<TInterface> GetExtensions<TInterface>(IElementContainer target, Expression<Func<IEnumerable<TInterface>>> propertyExpresion, Func<IProduct, TInterface> productProxyFactory)
		{
			Guard.NotNull(() => propertyExpresion, propertyExpresion);
			Guard.NotNull(() => productProxyFactory, productProxyFactory);

			var property = GetExpressionPropertyOrThrow(target, propertyExpresion);
			var definitionName = GetDesignerOrThrow(property.PropertyType.GetGenericArguments()[0]);

			return target.Extensions
				.Where(element => element.DefinitionName == definitionName)
				.Select(element => productProxyFactory(element));
		}

		private static PropertyInfo GetExpressionPropertyOrThrow<TProperty>(object target, Expression<Func<TProperty>> propertyExpresion)
		{
			var member = propertyExpresion.Body as MemberExpression;
			if (member == null || !(member.Member is PropertyInfo))
			{
				throw new ArgumentException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_InvalidPropertyExpression,
					propertyExpresion), "propertyExpression");
			}

			return (PropertyInfo)member.Member;
		}

		private static PropertyDescriptor GetInterfacePropertyOrThrow<TInterface>(PropertyInfo expressionProperty)
		{
			var property = TypeDescriptor.GetProperties(typeof(TInterface)).Find(expressionProperty.Name, false);
			if (property == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_PropertyNotFound,
					typeof(TInterface).FullName, expressionProperty.Name));
			return property;
		}

		private static string GetDesignerOrThrow(Type type)
		{
			var designer = type.GetCustomAttribute<DesignerAttribute>();
			if (designer == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_DesignerNotFound,
					type.FullName));

			return designer.DesignerTypeName;
		}

		private static TInterface CreateProxy<TInterface, TProxied>(object target)
		{
			var interfaceType = typeof(TInterface);
			var proxyType = GetProxyTypeOrThrow(interfaceType);

			ThrowIfProxyDoesNotImplementInterface(proxyType, interfaceType);

			var constructor = GetProxyConstructorOrThrow(proxyType, typeof(TProxied));

			return (TInterface)constructor.Invoke(new object[] { target });
		}

		private static Type GetProxyTypeOrThrow(Type type)
		{
			var designer = type.GetCustomAttribute<DesignerAttribute>();
			if (designer == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_DesignerNotFound,
					type.FullName));

			try
			{
				return Type.GetType(designer.DesignerBaseTypeName, true);
			}
			catch (TypeLoadException inner)
			{
				throw new TypeLoadException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_DesignerTypeLoadFailed,
					designer.DesignerBaseTypeName,
					type.FullName), inner);
			}
		}

		private static void ThrowIfProxyDoesNotImplementInterface(Type proxyType, Type expectedInterface)
		{
			if (!expectedInterface.IsAssignableFrom(proxyType))
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_ProxyMissingInterface,
					proxyType.FullName,
					expectedInterface.FullName));
		}

		private static ConstructorInfo GetProxyConstructorOrThrow(Type proxyType, Type argumentType)
		{
			var constructor = proxyType.GetConstructor(new[] { argumentType });
			if (constructor == null)
				throw new NotSupportedException(string.Format(
					CultureInfo.CurrentCulture,
					Resources.InterfaceLayer_ProxyImplementationConstructorMissing,
					proxyType.FullName,
					argumentType.FullName));

			return constructor;
		}
	}
}