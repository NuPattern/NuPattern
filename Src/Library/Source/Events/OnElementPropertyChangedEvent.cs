using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Events;

namespace NuPattern.Library.Events
{
    /// <summary>
    /// Exposes the event raised when a property of a runtime component is changed.
    /// </summary>
    public interface IOnElementPropertyChangedEvent : IObservable<IEvent<PropertyChangedEventArgs>>, IObservableEvent
    {
    }

    /// <summary>
    /// Assumes there can only be one state opened at any given time.
    /// </summary>
    [DisplayNameResource("OnElementPropertyChangedEvent_DisplayName", typeof(Resources))]
    [DescriptionResource("OnElementPropertyChangedEvent_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [Event(typeof(IOnElementPropertyChangedEvent))]
    [Export(typeof(IOnElementPropertyChangedEvent))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal sealed class OnElementPropertyChangedEvent : IOnElementPropertyChangedEvent, IDisposable
    {
        private IProductElement productElement;

        /// <summary>
        /// The event that is the source to which subscribers subscribe. 
        /// In this case, it's our own event that we republish from 
        /// the underlying event.
        /// </summary>
        private IObservable<IEvent<PropertyChangedEventArgs>> localSourceEvent;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnElementPropertyChangedEvent"/> class.
        /// </summary>
        [ImportingConstructor]
        public OnElementPropertyChangedEvent([Import(AllowDefault = true)] IProductElement productElement)
        {
            // We get null when the event is being validated as it's constructed from the global 
            // composition container to retrieve the event type. 
            // See EventSettingsValidations constructor.
            if (productElement != null)
            {
                this.productElement = productElement;
                this.productElement.PropertyChanged += OnPropertyChanged;
            }

            this.localSourceEvent = WeakObservable.FromEvent<PropertyChangedEventArgs>(
                handler => this.ElementPropertyChanged += handler,
                handler => this.ElementPropertyChanged -= handler);
        }

        /// <summary>
        /// Private event used to re-publish the state event.
        /// </summary>
        private event EventHandler<PropertyChangedEventArgs> ElementPropertyChanged = (sender, args) => { };

        /// <summary>
        /// Subscribes the specified observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<IEvent<PropertyChangedEventArgs>> observer)
        {
            Guard.NotNull(() => observer, observer);

            return this.localSourceEvent.Subscribe(observer);
        }

        /// <summary>
        /// Cleans up subscriptions.
        /// </summary>
        public void Dispose()
        {
            this.productElement.PropertyChanged -= OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnStoreElementChanged(Event.Create(sender, e));
        }

        private void OnStoreElementChanged(IEvent<PropertyChangedEventArgs> args)
        {
            this.ElementPropertyChanged(args.Sender, args.EventArgs);
        }
    }
}
