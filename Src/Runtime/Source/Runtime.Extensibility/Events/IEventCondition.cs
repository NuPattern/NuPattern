using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Events
{
	/// <summary>
	/// Represents a condition that can filter an event.
	/// </summary>
	[CLSCompliant(false)]
	public interface IEventCondition : ICondition
	{
		/// <summary>
		/// Gets or sets the current event.
		/// </summary>
		IEvent<EventArgs> Event { get; set; }
	}
}
