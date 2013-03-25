using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    internal class ContainerProxy<TInterface> : IContainerProxy<TInterface>
    {
        private IElementContainer container;

        public ContainerProxy(IElementContainer container)
        {
            this.container = container;
        }

        public TElement CreateElement<TElement>(string name, Action<TElement> initializer = null, bool raiseInstantiateEvents = true)
            where TElement : class
        {
            return ToolkitInterfaceLayer.CreateElement(this.container, name, initializer, raiseInstantiateEvents);
        }

        public TCollection CreateCollection<TCollection>(string name, Action<TCollection> initializer = null, bool raiseInstantiateEvents = true)
            where TCollection : class
        {
            return ToolkitInterfaceLayer.CreateCollection(this.container, name, initializer, raiseInstantiateEvents);
        }

        public TExtension CreateExtension<TExtension>(string name, Guid productId, string toolkitId, Action<TExtension> initializer = null, bool raiseInstantiateEvents = true)
            where TExtension : class
        {
            return ToolkitInterfaceLayer.CreateExtension(this.container, name, productId, toolkitId, initializer, raiseInstantiateEvents);
        }

        public TElement GetElement<TElement>(Expression<Func<TElement>> propertyExpresion, Func<IAbstractElement, TElement> elementProxyFactory)
            where TElement : class
        {
            return ToolkitInterfaceLayer.GetElement(this.container, propertyExpresion, elementProxyFactory);
        }

        public IEnumerable<TElement> GetElements<TElement>(Expression<Func<IEnumerable<TElement>>> propertyExpresion, Func<IAbstractElement, TElement> elementProxyFactory)
            where TElement : class
        {
            return ToolkitInterfaceLayer.GetElements(this.container, propertyExpresion, elementProxyFactory);
        }

        public TExtension GetExtension<TExtension>(Expression<Func<TExtension>> propertyExpresion, Func<IProduct, TExtension> productProxyFactory)
            where TExtension : class
        {
            return ToolkitInterfaceLayer.GetExtension(this.container, propertyExpresion, productProxyFactory);
        }

        public IEnumerable<TExtension> GetExtensions<TExtension>(Expression<Func<IEnumerable<TExtension>>> propertyExpresion, Func<IProduct, TExtension> productProxyFactory)
            where TExtension : class
        {
            return ToolkitInterfaceLayer.GetExtensions(this.container, propertyExpresion, productProxyFactory);
        }
    }
}
