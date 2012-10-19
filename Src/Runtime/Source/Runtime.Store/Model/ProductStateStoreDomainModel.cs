using System;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
	/// <summary>
	/// Product state state domain model.
	/// </summary>
	public partial class ProductStateStoreDomainModel
	{
		/// <summary>
		/// Gets the list of non-generated domain model types.
		/// </summary>
		/// <returns>List of types.</returns>
		protected override Type[] GetCustomDomainModelTypes()
		{
			return new[]
			{ 
				typeof(ProductAddRule),
				typeof(ViewAddRule),
				typeof(AbstractElementAddRule),
				typeof(ProductElementAddRule), 
				typeof(PropertyAddRule)
			};
		}
	}
}