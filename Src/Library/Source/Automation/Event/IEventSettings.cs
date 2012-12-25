using System.Collections.Generic;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
	public partial interface IEventSettings
	{
		/// <summary>
		/// Gets the condition settings.
		/// </summary>
		IEnumerable<IBindingSettings> ConditionSettings { get; }
	}
}