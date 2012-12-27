using System;
using System.Windows;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Exposes the event raised when any runtime component in the pattern state 
	/// is instantiated.
	/// </summary>
	public interface IOnSolutionBuilderDrop : IObservable<IEvent<DragEventArgs>>, IObservableEvent
	{
	}

}
