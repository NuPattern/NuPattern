using System;

namespace NuPattern.Runtime
{
	public partial interface IInstanceBase
	{
		/// <summary>
		/// Occurs when the runtime element is being deleted from the state.
		/// </summary>
		event EventHandler Deleting;

		/// <summary>
		/// Occurs when the runtime element has been deleted from the state.
		/// </summary>
		event EventHandler Deleted;

		/// <summary>
		/// Gets the parent of the current element, or <see langword="null"/> if 
		/// it's the root.
		/// </summary>
		[Hidden]
		IInstanceBase Parent { get; }

		/// <summary>
		/// Gets the root pattern ancestor for this instance. Note that for a pattern, 
		/// this may be an ancestor pattern if it has been instantiated as an 
		/// extension point.
		/// </summary>
		[Hidden]
		IProduct Root { get; }
	}
}
