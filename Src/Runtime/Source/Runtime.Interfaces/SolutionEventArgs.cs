using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Provides data to the events triggered related to solution actions.
	/// </summary>
	[CLSCompliant(false)]
	public class SolutionEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SolutionEventArgs"/> class.
		/// </summary>
		/// <param name="solution">The solution.</param>
		public SolutionEventArgs(ISolution solution)
		{
			this.Solution = solution;
		}

		/// <summary>
		/// Gets the solution.
		/// </summary>
		/// <value>The solution.</value>
		public ISolution Solution { get; private set; }
	}
}