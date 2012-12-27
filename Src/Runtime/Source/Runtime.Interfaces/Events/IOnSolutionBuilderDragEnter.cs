using System;
using System.Windows;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Exposes the event raised when any runtime component in the pattern state is dragged.
	/// </summary>
	public interface IOnSolutionBuilderDragEnter : IObservable<IEvent<DragEventArgs>>, IObservableEvent
	{
	}
}
