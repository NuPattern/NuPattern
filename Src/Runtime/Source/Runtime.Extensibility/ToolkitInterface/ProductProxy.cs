using System;
using System.Linq.Expressions;

namespace NuPattern.Runtime.ToolkitInterface
{
    internal class ProductProxy<TInterface> : IProductProxy<TInterface>
        where TInterface : class
    {
        private IProduct product;

        public ProductProxy(IProduct product)
        {
            this.product = product;
        }

        public TView GetView<TView>(Expression<Func<TView>> propertyExpresion, Func<IView, TView> viewProxyFactory)
            where TView : class
        {
            return ToolkitInterfaceLayer.GetView<TInterface, TView>(this.product, propertyExpresion, viewProxyFactory);
        }

        public TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion)
        {
            return ToolkitInterfaceLayer.GetValue<TInterface, TProperty>(this.product, propertyExpresion);
        }

        public void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion, TProperty value)
        {
            ToolkitInterfaceLayer.SetValue<TInterface, TProperty>(this.product, propertyExpresion, value);
        }
    }
}
