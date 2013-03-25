using System;
using System.ComponentModel.Composition;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.UI.Events
{
    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource("OnSolutionBuilderDragLeave_DisplayName", typeof(Resources))]
    [DescriptionResource("OnSolutionBuilderDragLeave_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_DragDrop", typeof(Resources))]
    [Event(typeof(IOnSolutionBuilderDragLeave))]
    [Export(typeof(IOnSolutionBuilderDragLeave))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnSolutionBuilderDragLeave : IOnSolutionBuilderDragLeave, IDisposable
    {
        private SolutionBuilderView solutionBuilder;
        private IObservable<IEvent<DragEventArgs>> sourceEvent;

        [ImportingConstructor]
        public OnSolutionBuilderDragLeave([Import]SolutionBuilderView solutionBuilder)
        {
            Guard.NotNull(() => solutionBuilder, solutionBuilder);

            this.solutionBuilder = solutionBuilder;

            this.sourceEvent = WeakObservable.FromEvent<DragEventHandler, DragEventArgs>(
                handler => new DragEventHandler(handler),
                handler => this.solutionBuilder.ElementDragLeave += handler,
                handler => this.solutionBuilder.ElementDragLeave -= handler);
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
