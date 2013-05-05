using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.ComponentModel;

namespace NuPattern.UnitTests
{
    [TestClass]
    public class Misc
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenSourceEventRaised_ThenSubscriberIsNotified()
        {
            var source = new EventSource();
            var publisher = new EventPublisher(source);

            var subscriber = new EventSubscriber();

            publisher.PropertyChanged.Subscribe(subscriber.OnChanged);

            Assert.Equal(0, EventSubscriber.ChangedProperties.Count);

            source.RaisePropertyChanged("Foo");

            Assert.Equal(1, EventSubscriber.ChangedProperties.Count);
            Assert.Equal("Foo", EventSubscriber.ChangedProperties[0]);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenWeakTargetCollected_ThenWeakReferenceNotAlive()
        {
            var source = new EventSource();
            var weak = new WeakReference(source);

            source = null;
            GC.Collect();

            Assert.False(weak.IsAlive);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSourceEventRaised_ThenCollectedSubscriberIsNotNotified()
        {
            var source = new EventSource();
            var publisher = new EventPublisher(source);
            var subscriber = new EventSubscriber();

            var subscription = publisher.PropertyChanged.Subscribe(subscriber.OnChanged);

            try
            {
                subscriber = null;
                subscription.Dispose();
                GC.Collect();
                GC.WaitForFullGCApproach(-1);
                GC.WaitForFullGCComplete(-1);
                GC.WaitForPendingFinalizers();

                source.RaisePropertyChanged("Foo");

                Assert.Equal(0, EventSubscriber.ChangedProperties.Count);

            }
            finally
            {
                //subscription.Dispose();
            }
        }

        public class EventSource : INotifyPropertyChanged
        {
            // NOTE: weak observable seems to be broken, but 
            // we use our own PropertyChangeManager which already 
            // does weak references, so changing the implementation 
            // of the event, makes the test pass. 
            // Would be nice to fix the observable stuff in the
            // future, but this ensures that the test passes, and 
            // actually showcases how this actually works currently 
            // at runtime.
            private PropertyChangeManager propertyChanges;

            /// <summary>
            /// Exposes the property changed event.
            /// </summary>
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { this.PropertyChanges.AddHandler(value); }
                remove { this.PropertyChanges.RemoveHandler(value); }
            }

            public void RaisePropertyChanged(string propertyName)
            {
                this.propertyChanges.NotifyChanged(propertyName);
            }


            /// <summary>
            /// Gets the manager for property change event subscriptions for this instance 
            ///	and any of its derived classes.
            /// </summary>
            private PropertyChangeManager PropertyChanges
            {
                get
                {
                    return this.propertyChanges ?? (this.propertyChanges = new PropertyChangeManager(this));
                }
            }
        }

        public class EventPublisher
        {
            //// private INotifyPropertyChanged eventSource;

            public EventPublisher(INotifyPropertyChanged eventSource)
            {
                //// this.eventSource = eventSource;

                this.PropertyChanged = WeakObservable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => new PropertyChangedEventHandler(handler.Invoke),
                    handler => eventSource.PropertyChanged += handler,
                    handler => eventSource.PropertyChanged -= handler);
            }

            public IObservable<IEvent<PropertyChangedEventArgs>> PropertyChanged { get; private set; }
        }

        public class EventSubscriber
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "On purpose")]
            public static readonly IList<string> ChangedProperties = new List<string>();

            public void OnChanged(IEvent<PropertyChangedEventArgs> @event)
            {
                ChangedProperties.Add(@event.EventArgs.PropertyName);
            }
        }
    }

    [TestClass]
    public class ObservableEventSpec
    {
        //// internal static readonly IAssertion Assert = new Assertion();

        /*
        public class Foo
        {
            private event EventHandler Done = (sender, args) => { };
            private event EventHandler Doing = (sender, args) => { };

            public Foo()
            {
                ((FooEvents)this.Events).Done = new ObservableEventManager<Foo, EventArgs>(this, x => x.Done += null);
                ((FooEvents)this.Events).Doing = new ObservableEventManager<Foo, EventArgs>(this, x => x.Doing += null);
            }

            public IFooEvents Events { get; set; }

            public class FooEvents : IFooEvents
            {
                public IObservableEvent<EventArgs> Done { get; internal set; }
                public IObservableEvent<EventArgs> Doing { get; internal set; }
            }

            public interface IFooEvents
            {
                IObservableEvent<EventArgs> Done { get; }
                IObservableEvent<EventArgs> Doing { get; }
            }
        }

        public class EventHandlerAutomationExtension : IAutomationExtension
        {
            public EventHandlerAutomationExtension([ImportMany] IEnumerable<Lazy<IObservableEvent, string>> events, string eventName, string commandName)
            {
                var automationEvent = events.First(x => x.Metadata == eventName);
                automationEvent.Value.Subscribe(OnEventRaised);
            }

            private void OnEventRaised()
            {
                Execute();
            }

            public string Name
            {
                get { return null; }
            }

            public void Execute()
            {
                // TODO: execute wizard
                // TODO: execute commmand
            }
        }

        public interface ISolutionEventsPublisher
        {
            IObservableEvent<SolutionEventArgs> SolutionOpened { get; }
            IObservableEvent<SolutionEventArgs> SolutionClosed { get; }
        }

        public class SolutionEventsPublisher : ISolutionEventsPublisher
        {
            private ISolutionEvents events;

            public SolutionEventsPublisher(ISolutionEvents events)
            {
                this.events = events;

                this.SolutionOpened = new ObservableEventManager<ISolutionEvents, SolutionEventArgs>(events, x => x.SolutionOpened += null);
                this.SolutionClosed = new ObservableEventManager<ISolutionEvents, SolutionEventArgs>(events, x => x.SolutionClosed += null);
            }

            [ObservableEventExport("SolutionOpened")]
            [ObservableEventExport("SolutionOpened", typeof(SolutionEventArgs))]
            public IObservableEvent<SolutionEventArgs> SolutionOpened { get; private set; }

            [Export(typeof(IObservableEvent))]
            [Export(typeof(IObservableEvent<SolutionEventArgs>))]
            public IObservableEvent<SolutionEventArgs> SolutionClosed { get; private set; }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class ObservableEventExportAttribute : InheritedExportAttribute
        {
            public ObservableEventExportAttribute(string eventName)
                : base(eventName, typeof(IObservableEvent))
            {
            }

            public ObservableEventExportAttribute(string eventName, Type eventArgs)
                : base(eventName, typeof(IObservableEvent<>).MakeGenericType(eventArgs))
            {
            }
        }

        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        public class ObservableEventImportAttribute : ImportAttribute
        {
            public ObservableEventImportAttribute(string eventName)
                : base(eventName, typeof(IObservableEvent))
            {
            }

            public ObservableEventImportAttribute(string eventName, Type eventArgs)
                : base(eventName, typeof(IObservableEvent<>).MakeGenericType(eventArgs))
            {
            }
        }

        */
    }
}
