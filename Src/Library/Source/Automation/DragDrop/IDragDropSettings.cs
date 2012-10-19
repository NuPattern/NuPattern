using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	public partial interface IDragDropSettings
	{
		/// <summary>
		/// Gets the condition settings.
		/// </summary>
		IEnumerable<IBindingSettings> ConditionSettings { get; }
	}
}
