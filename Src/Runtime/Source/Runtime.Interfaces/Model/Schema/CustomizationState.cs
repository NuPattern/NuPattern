
namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines the allowable values for customization.
	/// </summary>
	public enum CustomizationState
	{
		/// <summary>
		/// Customization is determined by the value of the item which this item is related to.
		/// </summary>
		Inherited,

		/// <summary>
		/// Cannot be customized.
		/// </summary>
		False,

		/// <summary>
		/// Can be customized.
		/// </summary>
		True
	}
}
