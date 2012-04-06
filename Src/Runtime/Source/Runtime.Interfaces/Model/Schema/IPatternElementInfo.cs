using System.Collections.Generic;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	public partial interface IPatternElementInfo
	{
		/// <summary>
		/// Gets the validation settings.
		/// </summary>
		IEnumerable<IBindingSettings> ValidationSettings { get; }
	}
}