using System;
using System.ComponentModel.Composition;
using System.Windows;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource("OnSolutionBuilderDragEnter_DisplayName", typeof(Resources))]
    [DescriptionResource("OnSolutionBuilderDragEnter_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_DragDrop", typeof(Resources))]
    [Event(typeof(IOnSolutionBuilderDragEnter))]
    [Export(typeof(IOnSolutionBuilderDragEnter))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnSolutionBuilderDragEnter : IOnSolutionBuilderDragEnter, IDisposable
    {
        private SolutionBuilderView solutionBuilder;
        private IObservable<IEvent<DragEventArgs>> sourceEvent;

        [ImportingConstructor]
        public OnSolutionBuilderDragEnter([Import]SolutionBuilderView solutionBuilder)
        {
            Guard.NotNull(() => solutionBuilder, solutionBuilder);

            this.solutionBuilder = solutionBuilder;

            this.sourceEvent = WeakObservable.FromEvent<DragEventHandler, DragEventArgs>(
                handler => new DragEventHandler(handler),
                handler => this.solutionBuilder.ElementDragEnter += handler,
                handler => this.solutionBuilder.ElementDragEnter -= handler);
        }

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<IEvent<DragEventArgs>> observer)
        {
            Guard.NotNull(() => observer, observer);

            return this.sourceEvent.Subscribe(observer);
        }

        /// <summary>
        /// Cleans up subscriptions.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
