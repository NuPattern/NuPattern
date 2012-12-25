using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NuPattern.Runtime
{
	internal class ContainerProxy<TInterface> : IContainerProxy<TInterface>
	{
		private IElementContainer container;

		public ContainerProxy(IElementContainer container)
		{
			this.container = container;
		}

		public TElement GetElement<TElement>(Expression<Func<TElement>> propertyExpresion, Func<IAbstractElement, TElement> elementProxyFactory)
		{
			return InterfaceLayer.GetElement(this.container, propertyExpresion, elementProxyFactory);
		}

		public IEnumerable<TElement> GetElements<TElement>(Expression<Func<IEnumerable<TElement>>> propertyExpresion, Func<IAbstractElement, TElement> elementProxyFactory)
		{
			return InterfaceLayer.GetElements(this.container, propertyExpresion, elementProxyFactory);
		}

		public TExtension GetExtension<TExtension>(Expression<Func<TExtension>> propertyExpresion, Func<IProduct, TExtension> productProxyFactory)
		{
			return InterfaceLayer.GetExtension(this.container, propertyExpresion, productProxyFactory);
		}

		public IEnumerable<TExtension> GetExtensions<TExtension>(Expression<Func<IEnumerable<TExtension>>> propertyExpresion, Func<IProduct, TExtension> productProxyFactory)
		{
			return InterfaceLayer.GetExtensions(this.container, propertyExpresion, productProxyFactory);
		}
	}
}
