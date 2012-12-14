using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Integration;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	[System.CodeDom.Compiler.GeneratedCode("T4", "1.0")]
	public partial class AdapterManager
	{
		/// <summary>
		/// Gets the file extension.
		/// </summary>
		public string FileExtension
		{
			get { return ".slnbldr"; }
		}

		/// <summary>
		/// Gets the exposed element types.
		/// </summary>
		public override IEnumerable<SupportedType> GetExposedElementTypes(string adapterId)
		{
			if (!GetSupportedLogicalAdapterIds().Contains(adapterId))
				yield break;

			yield return new SupportedType(typeof(IProductState), "ProductState");
			yield return new SupportedType(typeof(IProperty), "Property");
			yield return new SupportedType(typeof(ICollection), "Collection");
			yield return new SupportedType(typeof(IElement), "Element");
			yield return new SupportedType(typeof(IProduct), "Product");
			yield return new SupportedType(typeof(IView), "View");
			yield return new SupportedType(typeof(IReference), "Reference");
		}
	}
}