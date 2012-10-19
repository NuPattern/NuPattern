using System.Collections.Generic;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	public partial interface IEventSettings
	{
		/// <summary>
		/// Gets the condition settings.
		/// </summary>
		IEnumerable<IBindingSettings> ConditionSettings { get; }
	}
}