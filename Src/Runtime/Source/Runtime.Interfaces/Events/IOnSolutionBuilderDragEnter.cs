using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Exposes the event raised when any runtime component in the pattern state is dragged.
	/// </summary>
	public interface IOnSolutionBuilderDragEnter : IObservable<IEvent<DragEventArgs>>, IObservableEvent
	{
	}
}
