using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Project Item Events
	/// </summary>
	public interface IItemEvents : IDisposable
	{
		/// <summary>
		/// Occurs when an item is removed from the solution.
		/// </summary>
		event EventHandler<FileEventArgs> ItemRemoved;
	}
}