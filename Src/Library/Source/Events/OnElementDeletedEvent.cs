using System;
using System.ComponentModel.Composition;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Events;

namespace NuPattern.Library.Events
{
    /// <summary>
    /// Exposes the event raised when any runtime component in the pattern state 
    /// was deleted.
    /// </summary>
    public interface IOnElementDeletedEvent : IObservable<IEvent<EventArgs>>, IObservableEvent
    {
    }

    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource(@"OnElementDeletedEvent_DisplayName", typeof(Resources))]
    [DescriptionResource(@"OnElementDeletedEvent_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [Event(typeof(IOnElementDeletedEvent))]
    [Export(typeof(IOnElementDeletedEvent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class OnElementDeletedEvent : IOnElementDeletedEvent, IDisposable
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
        /// subscribe for republishing.
        /// </summary>
        private IObservable<IEvent<ValueEventArgs<IInstanceBase>>> storeEvent;

        /// <summary>
        /// The subscription to the <see cref="storeEvent"/> for disposal when 
        /// IsOpen changes in the <see cref="patternManager"/>.
        /// </summary>
        private IDisposable storeEventSubscription;

        /// <summary>
        /// Current element that the event exists on and which is used to 
        /// filter out the events that do not belong to this element.
        /// </summary>
        private IInstanceBase currentElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnElementDeletedEvent"/> class.
        /// </summary>
        [ImportingConstructor]
        public OnElementDeletedEvent(
            [Import(AllowDefault = true)] IPatternManager patternManager,
            [Import(AllowDefault = true)] IInstanceBase currentElement)
        {
            this.currentElement = currentElement;
            this.patternManager = patternManager;

            this.sourceEvent = WeakObservable.FromEvent<EventArgs>(
                handler => this.ElementDeleted += handler,
                handler => this.ElementDeleted -= handler);

            if (patternManager != null)
            {
                this.patternManager.IsOpenChanged += (sender, args) => this.OnOpenChanged();

                if (this.patternManager.IsOpen)
                {
                    this.storeEvent = WeakObservable.FromEvent<ValueEventArgs<IInstanceBase>>(
                        handler => this.patternManager.Store.ElementDeleted += handler,
                        handler => this.patternManager.Store.ElementDeleted -= handler);

                    this.storeEventSubscription = this.storeEvent.Subscribe(this.OnStoreElementDeleted);
                }
            }
        }

        /// <summary>
        /// Private event used to re-publish the state event with the right sender (the Activated element).
        /// </summary>
        private event EventHandler<EventArgs> ElementDeleted = (sender, args) => { };

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

            if (this.patternManager != null)
                this.patternManager.IsOpenChanged -= (sender, args) => this.OnOpenChanged();
        }

        private void OnOpenChanged()
        {
            if (this.storeEventSubscription != null)
            {
                this.storeEventSubscription.Dispose();
            }

            if (this.patternManager.IsOpen)
            {
                this.storeEvent = WeakObservable.FromEvent<ValueEventArgs<IInstanceBase>>(
                    handler => this.patternManager.Store.ElementDeleted += handler,
                    handler => this.patternManager.Store.ElementDeleted -= handler);

                this.storeEventSubscription = this.storeEvent.Subscribe(this.OnStoreElementDeleted);
            }
        }

        private void OnStoreElementDeleted(IEvent<ValueEventArgs<IInstanceBase>> args)
        {
            if (args.EventArgs.Value == this.currentElement)
                this.ElementDeleted(args.EventArgs.Value, EventArgs.Empty);
        }
    }
}
