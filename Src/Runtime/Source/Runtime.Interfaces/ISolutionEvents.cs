using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Interface that allows consumers to listen to solution events.
	/// </summary>
	[CLSCompliant(false)]
	public interface ISolutionEvents
	{
		/// <summary>
		/// Occurs before the solution is closed.
		/// </summary>
		event EventHandler<SolutionEventArgs> SolutionClosing;

		/// <summary>
		/// Occurs after the solution is closed.
		/// </summary>
		event EventHandler<SolutionEventArgs> SolutionClosed;

		/// <summary>
		/// Occurs after the solution is opened.
		/// </summary>
		event EventHandler<SolutionEventArgs> SolutionOpened;

		/// <summary>
		/// Gets a value indicating whether this instance is solution opened.
		/// </summary>
		/// <value>
		/// Returns <c>true</c> if this instance is solution opened; otherwise, <c>false</c>.
		/// </value>
		bool IsSolutionOpened { get; }
	}
}