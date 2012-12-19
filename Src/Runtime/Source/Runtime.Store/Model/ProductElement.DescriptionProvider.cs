using System.ComponentModel;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Design;

namespace NuPattern.Runtime.Store
{
	internal class ProductElementTypeDescriptionProvider : ElementTypeDescriptionProvider
	{
		/// <summary>
		/// Overridables for the derived class to provide a custom type descriptor.
		/// </summary>
		/// <param name="parent">Parent custom type descriptor.</param>
		/// <param name="element">Element to be described.</param>
		protected override ElementTypeDescriptor CreateTypeDescriptor(ICustomTypeDescriptor parent, ModelElement element)
		{
			return new ProductElementTypeDescriptor(parent, element);
		}
	}
}