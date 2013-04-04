using System;
using System.ComponentModel.Composition;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Events;

namespace NuPattern.Library.Events
{
    /// <summary>
    /// Exposes the event raised when the runtime state is saved.
    /// </summary>
    public interface IOnProductStoreSavedEvent : IObservable<IEvent<EventArgs>>, IObservableEvent
    {
    }

    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource("OnProductStoreSavedEvent_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("OnProductStoreSavedEvent_Description", typeof(Resources))]
    [Event(typeof(IOnProductStoreSavedEvent))]
    [Export(typeof(IOnProductStoreSavedEvent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnProductStoreSavedEvent : IOnProductStoreSavedEvent, IDisposable
    {
        private IPatternManager patternManager;

        /// <summary>
        /// The event that is the source to which subscribers subscribe. 
        /// In this case, it's our own event that we republish from 
        /// the underlying event, as we change the sender and the 
        /// event args.
        /// </summary>
        private IObservable<IEvent<EventArgs>> sourceEvent;

        /// <summary>
        /// This is the underlying state event that we internally 
        /// subscribe for replublishing.
        /// </summary>
        private IObservable<IEvent<ValueEventArgs<IProductState>>> storeEvent;

        /// <summary>
        /// The subscription to the <see cref="storeEvent"/> for disposal when 
        /// IsOpen changes in the <see cref="patternManager"/>.
        /// </summary>
        private IDisposable storeEventSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnProductStoreSavedEvent"/> class.
        /// </summary>
        [ImportingConstructor]
        public OnProductStoreSavedEvent(IPatternManager patternManager)
        {
            Guard.NotNull(() => patternManager, patternManager);

            this.patternManager = patternManager;
            this.patternManager.IsOpenChanged += (sender, args) => this.OnOpenChanged();

            this.sourceEvent = WeakObservable.FromEvent<EventArgs>(
                handler => this.ProductStoreSaved += handler,
                handler => this.ProductStoreSaved -= handler);

            if (this.patternManager.IsOpen)
            {
                this.storeEvent = WeakObservable.FromEvent<ValueEventArgs<IProductState>>(
                    handler => this.patternManager.StoreSaved += handler,
                    handler => this.patternManager.StoreSaved -= handler);

                this.storeEventSubscription = this.storeEvent.Subscribe(this.OnStoreProductStoreSaved);
            }
        }

        /// <summary>
        /// Private event used to re-publish the state event with the right sender.
        /// </summary>
        private event EventHandler<EventArgs> ProductStoreSaved = (sender, args) =>
        {
        };

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<IEvent<EventArgs>> observer)
        {
            Guard.NotNull(() => observer, observer);

            return this.sourceEvent.Subscribe(observer);
        }

        /// <summary>
        /// Cleans up subscriptions.
        /// </summary>
        public void Dispose()
        {
            if (this.storeEventSubscription != null)
            {
                this.storeEventSubscription.Dispose();
            }
        }

        private void OnOpenChanged()
        {
            if (this.storeEventSubscription != null)
            {
                this.storeEventSubscription.Dispose();
            }

            if (this.patternManager.IsOpen)
            {
                this.storeEvent = WeakObservable.FromEvent<ValueEventArgs<IProductState>>(
                    handler => this.patternManager.StoreSaved += handler,
                    handler => this.patternManager.StoreSaved -= handler);

                this.storeEventSubscription = this.storeEvent.Subscribe(this.OnStoreProductStoreSaved);
            }
        }

        private void OnStoreProductStoreSaved(IEvent<ValueEventArgs<IProductState>> args)
        {
            this.ProductStoreSaved(args.EventArgs.Value, EventArgs.Empty);
        }
    }
}
