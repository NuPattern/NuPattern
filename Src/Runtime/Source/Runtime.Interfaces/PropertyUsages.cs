using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// The primary usage of this property.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "We like General better than None?")]
	[Flags]
	public enum PropertyUsages
	{
		/// <summary>
		/// The property is a regular element property.
		/// </summary>
		General = 0,

		/// <summary>
		/// The property is imported from an extension point contract.
		/// </summary>
		ExtensionContract = 1
	}
}
