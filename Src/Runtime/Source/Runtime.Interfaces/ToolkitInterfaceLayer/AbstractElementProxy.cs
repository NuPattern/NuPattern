using System;
using System.Linq.Expressions;

namespace NuPattern.Runtime
{
	internal class AbstractElementProxy<TInterface> : ContainerProxy<TInterface>, IAbstractElementProxy<TInterface>
	{
		private IAbstractElement element;

		public AbstractElementProxy(IAbstractElement element)
			: base(element)
		{
			this.element = element;
		}

		public TProperty GetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion)
		{
			return ToolkitInterfaceLayer.GetValue<TInterface, TProperty>(this.element, propertyExpresion);
		}

		public void SetValue<TProperty>(Expression<Func<TProperty>> propertyExpresion, TProperty value)
		{
			ToolkitInterfaceLayer.SetValue<TInterface, TProperty>(this.element, propertyExpresion, value);
		}
	}
}
