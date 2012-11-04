using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Library.Events
{
	/// <summary>
	/// Visual Studio event that is raised whenever a build completes.
	/// </summary>
	public interface IOnBuildFinishedEvent : IObservable<IEvent<EventArgs>>, IObservableEvent
	{
	}

	/// <summary>
	/// Implements the <see cref="IOnBuildFinishedEvent"/>.
	/// </summary>
	[DisplayNameResource("OnBuildFinishedEvent_DisplayName", typeof(Resources))]
	[CategoryResource("AutomationCategory_VisualStudio", typeof(Resources))]
	[DescriptionResource("OnBuildFinishedEvent_Description", typeof(Resources))]
	[Event(typeof(IOnBuildFinishedEvent))]
	[Export(typeof(IOnBuildFinishedEvent))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	internal sealed class OnBuildFinishedEvent : IOnBuildFinishedEvent
	{
		private IObservable<IEvent<EventArgs>> sourceEvent;
		private EnvDTE.DTE dte;

		/// <summary>
		/// Initializes a new instance of the <see cref="OnBuildFinishedEvent"/> class.
		/// </summary>
		[ImportingConstructor]
		public OnBuildFinishedEvent([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
		{
			Guard.NotNull(() => serviceProvider, serviceProvider);

			this.dte = serviceProvider.GetService<EnvDTE.DTE>();
			this.dte.Events.BuildEvents.OnBuildDone += this.OnDteBuildDone;

			this.sourceEvent = WeakObservable.FromEvent<EventArgs>(
				handler => this.BuildFinished += handler,
				handler => this.BuildFinished -= handler);
		}

		/// <summary>
		/// Internal event used to re-publish the DTE build event.
		/// </summary>
		internal event EventHandler<EventArgs> BuildFinished = (sender, args) => { };

		/// <summary>
		/// Subscribes the specified observer.
		/// </summary>
		public IDisposable Subscribe(IObserver<IEvent<EventArgs>> observer)
		{
			return this.sourceEvent.Subscribe(observer);
		}

		private void OnDteBuildDone(EnvDTE.vsBuildScope scope, EnvDTE.vsBuildAction action)
		{
			if (action == EnvDTE.vsBuildAction.vsBuildActionBuild ||
				action == EnvDTE.vsBuildAction.vsBuildActionRebuildAll)
			{
				this.BuildFinished(this, EventArgs.Empty);
			}
		}
	}
}
