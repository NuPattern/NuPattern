using System;
using System.ComponentModel.Composition;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource("OnSolutionBuilderDrop_DisplayName", typeof(Resources))]
    [DescriptionResource("OnSolutionBuilderDrop_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_DragDrop", typeof(Resources))]
    [Event(typeof(IOnSolutionBuilderDrop))]
    [Export(typeof(IOnSolutionBuilderDrop))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnSolutionBuilderDrop : IOnSolutionBuilderDrop, IDisposable
    {
        private SolutionBuilderView solutionBuilder;
        private IObservable<IEvent<DragEventArgs>> sourceEvent;

        [ImportingConstructor]
        public OnSolutionBuilderDrop([Import]SolutionBuilderView solutionBuilder)
        {
            Guard.NotNull(() => solutionBuilder, solutionBuilder);

            this.solutionBuilder = solutionBuilder;

            this.sourceEvent = WeakObservable.FromEvent<DragEventHandler, DragEventArgs>(
                handler => new DragEventHandler(handler),
                handler => this.solutionBuilder.ElementDrop += handler,
                handler => this.solutionBuilder.ElementDrop -= handler);
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
