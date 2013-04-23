using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime.Events;

namespace NuPattern.Library.Events
{
    /// <summary>
    /// Visual Studio event that is raised whenever a build starts.
    /// </summary>
    public interface IOnBuildStartedEvent : IObservable<IEvent<EventArgs>>, IObservableEvent
    {
    }

    /// <summary>
    /// Implements the <see cref="IOnBuildFinishedEvent"/>.
    /// </summary>
    [DisplayNameResource(@"OnBuildStartedEvent_DisplayName", typeof(Resources))]
    [DescriptionResource(@"OnBuildStartedEvent_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [Event(typeof(IOnBuildStartedEvent))]
    [Export(typeof(IOnBuildStartedEvent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnBuildStartedEvent : IOnBuildStartedEvent
    {
        private IObservable<IEvent<EventArgs>> sourceEvent;
        private EnvDTE.DTE dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnBuildStartedEvent"/> class.
        /// </summary>
        [ImportingConstructor]
        public OnBuildStartedEvent([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.dte = serviceProvider.GetService<EnvDTE.DTE>();
            this.dte.Events.BuildEvents.OnBuildBegin += this.OnDteBuildBegin;

            this.sourceEvent = WeakObservable.FromEvent<EventArgs>(
                handler => this.BuildStarted += handler,
                handler => this.BuildStarted -= handler);
        }

        /// <summary>
        /// Internal event used to re-publish the DTE build event.
        /// </summary>
        internal event EventHandler<EventArgs> BuildStarted = (sender, args) => { };

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<IEvent<EventArgs>> observer)
        {
            return this.sourceEvent.Subscribe(observer);
        }

        private void OnDteBuildBegin(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action)
        {
            if (action == EnvDTE.vsBuildAction.vsBuildActionBuild ||
                action == EnvDTE.vsBuildAction.vsBuildActionRebuildAll)
            {
                this.BuildStarted(this, EventArgs.Empty);
            }
        }
    }
}
