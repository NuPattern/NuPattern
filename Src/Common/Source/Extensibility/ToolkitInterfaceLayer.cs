using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Bindings;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.VisualStudio;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Provides extension methods to <see cref="IProduct"/>, <see cref="IView"/> and 
    /// <see cref="IAbstractElement"/> to create strong typed proxies.
    /// </summary>
    [CLSCompliant(false)]
    public static class ToolkitInterfaceLayer
    {
        private static ITraceSource tracer = Tracer.GetSourceFor(typeof(ToolkitInterfaceLayer));
        private static ThreadLocal<bool?> resolvingInstanceBaseAs = new ThreadLocal<bool?>();

        /// <summary>
        /// Adds the interface layer for the given instance to the dynamic binding context.
        /// </summary>
        public static void AddInterfaceLayer(this IDynamicBindingContext context, IProductElement element)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => element, element);

            //TODO: If we ever allow automation to be defined on an extension point, 
            // then for every pattern that implments and extension point, we must add a key to the cache for that extensionPoint.DefinitionId
            var layer = default(object);
            var key = new ToolkitInterfaceLayerCacheKey(element, element.DefinitionId);
            if (element.Root != null &&
                element.Root.ProductState != null &&
                element.Root.ProductState.PropertyBag != null &&
                !element.Root.ProductState.PropertyBag.TryGetValue(key, out layer))
            {
                layer = GetInterfaceLayer(element);
                if (layer != null)
                    element.Root.ProductState.PropertyBag[key] = layer;
            }

            if (layer != null)
            {
                context.AddExportsFromInterfaces(layer);
            }
        }

        /// <summary>
        /// Gets a strong typed interface layer for a pattern.
        /// </summary>
        public static TInterface As<TInterface>(this IInstanceBase instance)
        {
            Guard.NotNull(() => instance, instance);

            if (resolvingInstanceBaseAs.Value.GetValueOrDefault())
                throw new InvalidOperationException(String.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_InstanceTypeDoesNotHaveInterfaceLayer,
                    instance.GetType()));

            dynamic dynInstance = instance;

            resolvingInstanceBaseAs.Value = true;
            try
            {
                return (TInterface)ToolkitInterfaceLayer.As<TInterface>(dynInstance);
            }
            finally
            {
                resolvingInstanceBaseAs.Value = null;
            }
        }

        /// <summary>
        /// Gets a strong typed interface layer for a pattern.
        /// </summary>
        public static TInterface As<TInterface>(this IProduct product)
            where TInterface : class, IToolkitInterface
        {
            Guard.NotNull(() => product, product);

            return GetInterfaceLayer<TInterface, IProduct>(product);
        }

        /// <summary>
        /// Gets a strong typed interface layer for a view.
        /// </summary>
        public static TInterface As<TInterface>(this IView view)
            where TInterface : class, IToolkitInterface
        {
            Guard.NotNull(() => view, view);

            return GetInterfaceLayer<TInterface, IView>(view);
        }

        /// <summary>
        /// Gets a strong typed interface layer for a collection or element.
        /// </summary>
        public static TInterface As<TInterface>(this IAbstractElement element)
            where TInterface : class, IToolkitInterface
        {
            Guard.NotNull(() => element, element);

            return GetInterfaceLayer<TInterface, IAbstractElement>(element);
        }

        /// <summary>
        /// Gets a strong typed interface layer for a collection or element.
        /// </summary>
        public static TInterface As<TInterface>(this IProductElement element)
            where TInterface : class, IToolkitInterface
        {
            Guard.NotNull(() => element, element);

            var typed = element as TInterface;
            if (typed != null)
                return typed;

            var product = element as IProduct;
            if (product != null)
            {
                return GetInterfaceLayer<TInterface, IProduct>(product);
            }

            var abstractElement = element as IAbstractElement;
            if (abstractElement != null)
            {
                return GetInterfaceLayer<TInterface, IAbstractElement>(abstractElement);
            }

            return default(TInterface);
        }

        /// <summary>
        /// Gets a proxy accessor for a pattern targetting the given <typeparamref name="TInterface"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IProductProxy<TInterface> ProxyFor<TInterface>(this IProduct product)
            where TInterface : class
        {
            Guard.NotNull(() => product, product);

            return (product as IProductProxy<TInterface>) ?? new ProductProxy<TInterface>(product);
        }

        /// <summary>
        /// Gets a proxy accessor for a view targetting the given <typeparamref name="TInterface"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IContainerProxy<TInterface> ProxyFor<TInterface>(this IView view)
            where TInterface : class
        {
            Guard.NotNull(() => view, view);

            return (view as IContainerProxy<TInterface>) ?? new ContainerProxy<TInterface>(view);
        }

        /// <summary>
        /// Gets a proxy accessor for an abstract element targetting the given <typeparamref name="TInterface"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAbstractElementProxy<TInterface> ProxyFor<TInterface>(this IAbstractElement element)
            where TInterface : class
        {
            Guard.NotNull(() => element, element);

            return (element as IAbstractElementProxy<TInterface>) ?? new AbstractElementProxy<TInterface>(element);
        }

        internal static TProperty GetValue<TInterface, TProperty>(IProductElement target, Expression<Func<TProperty>> propertyExpresion)
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);

            var member = GetExpressionPropertyOrThrow<TProperty>(propertyExpresion);
            var property = GetInterfacePropertyOrThrow<TInterface>(member);

            var variableProperty = target.Properties.FirstOrDefault(prop => prop.DefinitionName == property.Name);
            if (variableProperty != null)
            {
                var value = variableProperty.Value;
                if (value != null && !typeof(TProperty).IsAssignableFrom(value.GetType()))
                    return (TProperty)property.Converter.ConvertFrom(value);

                return (TProperty)value;
            }

            var elementProperty = TypeDescriptor.GetProperties(target).Find(property.Name, false);
            if (elementProperty == null)
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_InstancePropertyNotFound,
                    target.GetType().Name,
                    property.Name));

            // We don't do type conversion from element (reflection) properties as they will already be typed accesses.
            return (TProperty)elementProperty.GetValue(target);
        }

        internal static void SetValue<TInterface, TProperty>(IProductElement target, Expression<Func<TProperty>> propertyExpresion, TProperty value)
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);

            var member = GetExpressionPropertyOrThrow<TProperty>(propertyExpresion);
            var property = GetInterfacePropertyOrThrow<TInterface>(member);

            var variableProperty = target.Properties.FirstOrDefault(prop => prop.DefinitionName == property.Name);
            if (variableProperty != null)
            {
                // The descriptor associated with the property will do the conversion already if supported.
                variableProperty.Value = value;
            }
            else
            {
                var elementProperty = TypeDescriptor.GetProperties(target).Find(property.Name, false);
                if (elementProperty == null)
                    throw new NotSupportedException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.ToolkitInterfaceLayer_InstancePropertyNotFound,
                        target.GetType().Name,
                        property.Name));

                // We don't do type conversion from element (reflection) properties as they will already be typed accesses.
                elementProperty.SetValue(target, value);
            }
        }

        internal static TInterface CreateElement<TInterface>(IElementContainer target, string name, Action<TInterface> initializer = null, bool raiseInstantiateEvents = true)
            where TInterface : class
        {
            return CreateAbstractElement<IElement, TInterface>(
                target,
                name,
                target.CreateElement,
                initializer,
                raiseInstantiateEvents);
        }

        internal static TInterface CreateCollection<TInterface>(IElementContainer target, string name, Action<TInterface> initializer = null, bool raiseInstantiateEvents = true)
            where TInterface : class
        {
            return CreateAbstractElement<ICollection, TInterface>(
                target,
                name,
                target.CreateCollection,
                initializer,
                raiseInstantiateEvents);
        }

        internal static TInterface CreateExtension<TInterface>(IElementContainer target, string name, Guid productId, string toolkitId, Action<TInterface> initializer = null, bool raiseInstantiateEvents = true)
            where TInterface : class
        {
            return CreateExtension<TInterface>(
                target,
                name, productId, toolkitId,
                target.CreateExtension,
                initializer,
                raiseInstantiateEvents);
        }

        private static TInterface CreateAbstractElement<TAbstractElement, TInterface>(IElementContainer target, string name,
            Func<Action<TAbstractElement>, bool, TAbstractElement> elementFactory,
            Action<TInterface> initializer = null, bool raiseInstantiateEvents = true)
            where TInterface : class
            where TAbstractElement : IAbstractElement
        {
            Guard.NotNull(() => target, target);
            Guard.NotNullOrEmpty(() => name, name);

            if (initializer == null)
                initializer = e => { };

            var toolkitInterface = GetToolkitInterfaceAttributeOrThrow(typeof(TInterface));
            using (var transaction = target.BeginTransaction())
            {
                var element = elementFactory(e =>
                {
                    e.DefinitionId = new Guid(toolkitInterface.DefinitionId);
                    e.InstanceName = name;
                }, raiseInstantiateEvents);

                var interfaceLayer = GetInterfaceLayer<TInterface, TAbstractElement>(element);
                initializer(interfaceLayer);

                transaction.Commit();

                return interfaceLayer;
            }
        }

        private static TInterface CreateExtension<TInterface>(IElementContainer target, string name, Guid productId, string toolkitId,
            Func<Action<IProduct>, bool, IProduct> elementFactory,
            Action<TInterface> initializer = null, bool raiseInstantiateEvents = true)
            where TInterface : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNullOrEmpty(() => name, name);
            Guard.NotNullOrEmpty(() => toolkitId, toolkitId);

            if (initializer == null)
                initializer = e => { };

            var toolkitInterface = GetToolkitInterfaceAttributeOrThrow(typeof(TInterface));
            using (var transaction = target.BeginTransaction())
            {
                var element = elementFactory(e =>
                {
                    e.DefinitionId = productId;
                    e.ExtensionId = toolkitId;
                    e.InstanceName = name;
                }, raiseInstantiateEvents);

                var interfaceLayer = GetInterfaceLayer<TInterface, IProduct>(element);
                initializer(interfaceLayer);

                transaction.Commit();

                return interfaceLayer;
            }
        }

        internal static TView GetView<TInterface, TView>(IProduct target, Expression<Func<TView>> propertyExpresion, Func<IView, TView> viewProxyFactory)
            where TInterface : class
            where TView : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);
            Guard.NotNull(() => viewProxyFactory, viewProxyFactory);

            var member = GetExpressionPropertyOrThrow(propertyExpresion);
            var property = GetInterfacePropertyOrThrow<TInterface>(member);

            var view = target.Views.FirstOrDefault(v => v.DefinitionName == property.Name);
            if (view == null)
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_ViewNotFound,
                    property.Name));

            return GetInterfaceLayer(view, () => viewProxyFactory(view));
        }

        internal static TInterface GetElement<TInterface>(IElementContainer target, Expression<Func<TInterface>> propertyExpresion, Func<IAbstractElement, TInterface> elementProxyFactory)
            where TInterface : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);
            Guard.NotNull(() => elementProxyFactory, elementProxyFactory);

            var property = GetExpressionPropertyOrThrow(propertyExpresion);
            var definitionId = new Guid(GetToolkitInterfaceAttributeOrThrow(property.PropertyType).DefinitionId);

            return target.Elements
                .Where(element => element.DefinitionId == definitionId)
                .Select(element => GetInterfaceLayer(element, () => elementProxyFactory(element)))
                .FirstOrDefault();
        }

        internal static IEnumerable<TInterface> GetElements<TInterface>(IElementContainer target, Expression<Func<IEnumerable<TInterface>>> propertyExpresion, Func<IAbstractElement, TInterface> elementProxyFactory)
            where TInterface : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);
            Guard.NotNull(() => elementProxyFactory, elementProxyFactory);

            var property = GetExpressionPropertyOrThrow(propertyExpresion);
            var definitionId = new Guid(GetToolkitInterfaceAttributeOrThrow(property.PropertyType.GetGenericArguments()[0]).DefinitionId);

            return target.Elements
                .Where(element => element.DefinitionId == definitionId)
                .Select(element => GetInterfaceLayer(element, () => elementProxyFactory(element)));
        }

        internal static TInterface GetExtension<TInterface>(IElementContainer target, Expression<Func<TInterface>> propertyExpresion, Func<IProduct, TInterface> productProxyFactory)
            where TInterface : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);
            Guard.NotNull(() => productProxyFactory, productProxyFactory);

            var property = GetExpressionPropertyOrThrow(propertyExpresion);
            var definitionId = new Guid(GetToolkitInterfaceAttributeOrThrow(property.PropertyType).DefinitionId);

            return target.Extensions
                .Where(extension => extension.DefinitionId == definitionId)
                .Select(extension => GetInterfaceLayer(extension, () => productProxyFactory(extension)))
                .FirstOrDefault();
        }

        internal static IEnumerable<TInterface> GetExtensions<TInterface>(IElementContainer target, Expression<Func<IEnumerable<TInterface>>> propertyExpresion, Func<IProduct, TInterface> productProxyFactory)
            where TInterface : class
        {
            Guard.NotNull(() => target, target);
            Guard.NotNull(() => propertyExpresion, propertyExpresion);
            Guard.NotNull(() => productProxyFactory, productProxyFactory);

            var property = GetExpressionPropertyOrThrow(propertyExpresion);
            var definitionId = new Guid(GetToolkitInterfaceAttributeOrThrow(property.PropertyType.GetGenericArguments()[0]).DefinitionId);

            return target.Extensions
                .Where(extension => extension.DefinitionId == definitionId)
                .Select(extension => GetInterfaceLayer(extension, () => productProxyFactory(extension)));
        }

        private static System.Reflection.PropertyInfo GetExpressionPropertyOrThrow<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var member = propertyExpression.Body as MemberExpression;
            if (member == null || !(member.Member is System.Reflection.PropertyInfo))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_InvalidPropertyExpression,
                    propertyExpression), "propertyExpression");
            }

            return (System.Reflection.PropertyInfo)member.Member;
        }

        private static PropertyDescriptor GetInterfacePropertyOrThrow<TInterface>(System.Reflection.PropertyInfo expressionProperty)
        {
            var property = TypeDescriptor.GetProperties(typeof(TInterface)).Find(expressionProperty.Name, false);
            if (property == null)
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_PropertyNotFound,
                    typeof(TInterface).FullName, expressionProperty.Name));

            return property;
        }

        private static ToolkitInterfaceAttribute GetToolkitInterfaceAttributeOrThrow(Type type)
        {
            var toolkitInterface = ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(type);
            if (toolkitInterface == null)
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_InterfaceLayerNotFound,
                    type.FullName));

            return toolkitInterface;
        }

        /// <summary>
        /// Attempts to retrieve (from cache or by creating and caching it the first time) 
        /// the typed interface layer proxy defined by the runtime instance, without knowing 
        /// its type. This is used in the add rules to initialize the interface layers.
        /// </summary>
        public static IToolkitInterface GetInterfaceLayer(this IInstanceBase instance)
        {
            Guard.NotNull(() => instance, instance);

            if (instance.Info == null)
                return null;

            var key = new ToolkitInterfaceLayerCacheKey(instance, instance.DefinitionId);
            var layer = default(object);
            var canCache = instance.Root != null &&
                instance.Root.ProductState != null &&
                instance.Root.ProductState.PropertyBag != null;

            if (canCache && instance.Root.ProductState.PropertyBag.TryGetValue(key, out layer))
                return (IToolkitInterface)layer;

            var interfaceService = default(IToolkitInterfaceService);
            if (!canCache || (interfaceService = instance.Root.ProductState.TryGetService<IToolkitInterfaceService>()) == null)
            {
                tracer.TraceWarning(Resources.ToolkitInterfaceLayer_ServiceUnavailable);
                return null;
            }

            var definitionId = Guid.Empty;
            var proxyType = interfaceService.AllInterfaces
                .Where(export => !string.IsNullOrEmpty(export.Metadata.DefinitionId) && Guid.TryParse(export.Metadata.DefinitionId, out definitionId))
                .Where(export =>
                    export.Metadata.ExtensionId == instance.Info.GetRoot().ExtensionId &&
                    definitionId == instance.DefinitionId)
                .Select(export => export.Metadata.ProxyType)
                .FirstOrDefault();

            if (proxyType != null)
            {
                try
                {
                    layer = Activator.CreateInstance(proxyType, instance);
                    if (layer != null && canCache)
                        instance.Root.ProductState.PropertyBag[key] = layer;

                    return layer as IToolkitInterface;
                }
                catch (System.Reflection.TargetInvocationException tie)
                {
                    tracer.TraceError(tie.InnerException, "Failed to instantiate the interface layer proxy type '{0}'.", proxyType);
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves a cached interface layer proxy or creates a new one and caches it 
        /// the first time. Infers the toolkit from the toolkit interface attribute 
        /// on the <typeparamref name="TInterface"/>.
        /// </summary>
        private static TInterface GetInterfaceLayer<TInterface, TProxied>(IInstanceBase target)
            where TInterface : class
        {
            Func<TInterface> toolkit = () =>
                {
                    var interfaceType = typeof(TInterface);
                    var interfaceAttr = GetToolkitInterfaceAttributeOrThrow(interfaceType);

                    if (!IsTargetInterfaceDefinition(target, new Guid(interfaceAttr.DefinitionId)))
                        return default(TInterface);

                    var proxyType = interfaceAttr.ProxyType;

                    ThrowIfProxyDoesNotImplementInterface(proxyType, interfaceType);

                    var constructor = GetProxyConstructorOrThrow(proxyType, typeof(TProxied));
                    return (TInterface)constructor.Invoke(new object[] { target });
                };

            return GetInterfaceLayer(target, toolkit);
        }

        private static bool IsTargetInterfaceDefinition(IInstanceBase target, Guid definitionId)
        {
            // Are we creating an extension pattern?
            var guestProduct = target as IProduct;
            if (guestProduct != null && guestProduct.Info != null)
            {
                // See if the guest pattern implements an extension point within the host pattern
                var hostProduct = guestProduct.Product;
                if (hostProduct != null
                    && hostProduct != guestProduct
                    && hostProduct.Info != null)
                {
                    var extensions = hostProduct.Info.FindAllDescendants<IExtensionPointInfo>();
                    if (extensions.Any())
                    {
                        var extensionDefinitionId = extensions.FirstOrDefault(rex => guestProduct.Info.ProvidedExtensionPoints.Any(pex => pex.ExtensionPointId == rex.RequiredExtensionPointId));
                        if (extensionDefinitionId != null)
                        {
                            if (extensionDefinitionId.Id == definitionId)
                                return true;
                        }
                    }
                }
            }

            // Did we ask for correct interface?
            return (definitionId == target.DefinitionId);
        }

        /// <summary>
        /// Retrieves a cached interface layer proxy or creates a new one using 
        /// the toolkit specified and caches it the first time.
        /// </summary>
        private static TInterface GetInterfaceLayer<TInterface>(IInstanceBase element, Func<TInterface> toolkit)
            where TInterface : class
        {
            var typed = element as TInterface;
            if (typed != null)
                return typed;

            // We'll never be able to cache in this case.
            if (element.Root == null ||
                element.Root.ProductState == null ||
                element.Root.ProductState.PropertyBag == null)
                return toolkit();

            var layer = default(object);
            var key = new ToolkitInterfaceLayerCacheKey(element, element.DefinitionId);

            if (element.Root.ProductState.PropertyBag.TryGetValue(key, out layer))
            {
                return layer as TInterface;
            }
            else
            {
                var result = toolkit();
                if (result != default(TInterface))
                    element.Root.ProductState.PropertyBag[key] = result;

                return result;
            }
        }

        private static void ThrowIfProxyDoesNotImplementInterface(Type proxyType, Type expectedInterface)
        {
            if (!expectedInterface.IsAssignableFrom(proxyType))
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_ProxyMissingInterface,
                    proxyType.FullName,
                    expectedInterface.FullName));
        }

        private static System.Reflection.ConstructorInfo GetProxyConstructorOrThrow(Type proxyType, Type argumentType)
        {
            var constructor = proxyType.GetConstructor(new[] { argumentType });
            if (constructor == null)
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ToolkitInterfaceLayer_ProxyImplementationConstructorMissing,
                    proxyType.FullName,
                    argumentType.FullName));

            return constructor;
        }
    }
}