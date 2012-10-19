using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace $rootnamespace$
{
    /// <summary>
    /// The On$safeitemname$ event.
    /// </summary>
    public interface IOn$safeitemname$ : IObservable<IEvent<EventArgs>>, IObservableEvent
    {
    }

    /// <summary>
    /// The $safeitemname$ event, that republishes the event for listening automation.
    /// </summary>
    [DisplayName("On$safeitemname$ Custom VS Event")]
    [Category("General")]
    [Description("Raises the On$safeitemname$ custom event.")]
    [Event(typeof(IOn$safeitemname$))]
    [Export(typeof(IOn$safeitemname$))]
    public class On$safeitemname$ : IOn$safeitemname$
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<On$safeitemname$>();
        private IObservable<IEvent<EventArgs>> sourceEvent;

		/// <summary>
		/// Initializes a new instance of the <see cref="On$safeitemname$"/> class.
		/// </summary>
		[ImportingConstructor]
        public On$safeitemname$(/*[Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider*/)
		{
			//Guard.NotNull(() => serviceProvider, serviceProvider);
			
            // Subscribe to originating event and republish it from this event handler
            // Example:
                //this.dte = serviceProvider.GetService<EnvDTE.DTE>();
                //this.dte.Events.BuildEvents.OnBuildDone += this.OnEventRaised; //Subscribes to the built-in VS event

            // 
			this.sourceEvent = WeakObservable.FromEvent<EventArgs>(
				handler => this.$safeitemname$ += handler,
				handler => this.$safeitemname$ -= handler);
		}

		/// <summary>
		/// Defines the automation event.
		/// </summary>
		public event EventHandler<EventArgs> $safeitemname$ = (sender, args) => { };

		/// <summary>
		/// Subscribes the specified observer.
		/// </summary>
		public IDisposable Subscribe(IObserver<IEvent<EventArgs>> observer)
		{
			return this.sourceEvent.Subscribe(observer);
		}

        /// <summary>
        /// Re-publishes the event for listening automation
        /// </summary>
        private void OnEventRaised()
        {
            // Raise custom event
            this.$safeitemname$(this, EventArgs.Empty);
        }
    }
}
