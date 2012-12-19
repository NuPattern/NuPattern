using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NuPattern.Runtime
{
	internal class ProductProxy<TInterface> : IProductProxy<TInterface>
	{
		private IProduct product;

		public ProductProxy(IProduct product)
		{
			this.product = product;
		}

		public TView GetView<TView>(Expression<Func<TView>> propertyExpresion, Func<IView, TView> viewProxyFactory)
		{
			return InterfaceLayer.GetView<TInterface, TView>(this.product, propertyExpresion, viewProxyFactory);
		}

		public TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion)
		{
			return InterfaceLayer.GetValue<TInterface, TProperty>(this.product, propertyExpresion);
		}

		public void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion, TProperty value)
		{
			InterfaceLayer.SetValue<TInterface, TProperty>(this.product, propertyExpresion, value);
		}
	}
}
