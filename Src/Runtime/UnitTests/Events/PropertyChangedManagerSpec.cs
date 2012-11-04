using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
    [TestClass]
    public class PropertyChangedManagerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenSubscriberIsAlive_ThenNotifiesSubscriber()
        {
            var notified = false;
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);

            manager.SubscribeChanged((Expression<Func<Foo, object>>)(x => x.Name), (Action<Foo>)(f => notified = true));

            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.True(notified);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSubscriberIsNotAlive_ThenDoesNotNotifySubscriber()
        {
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);
            var subscriber = new TestSubscriber("Name", 1);

            manager.SubscribeChanged((Expression<Func<Foo, object>>)(x => x.Name), (Action<Foo>)(subscriber.SubscriptionHandler));
            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.Equal(1, subscriber.ActualCalls);

            subscriber = null;
            GC.Collect();

            manager.NotifyChanged<Foo>(x => x.Name);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSubscriptionIsDisposed_ThenDoesNotNotifySubscriber()
        {
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);
            var subscriber = new TestSubscriber("Name", 1);

            var subscription = manager.SubscribeChanged((Expression<Func<Foo, object>>)(x => x.Name), (Action<Foo>)(subscriber.SubscriptionHandler));
            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.Equal(1, subscriber.ActualCalls);

            subscription.Dispose();
            GC.Collect();

            manager.NotifyChanged<Foo>(x => x.Name);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAddingPropertyChangedHandler_ThenNotifiesSubscriber()
        {
            var notified = false;
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);
            PropertyChangedEventHandler handler = (sender, args) => notified = true;

            manager.AddHandler(handler);

            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.True(notified);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenRemovingPropertyChangedHandler_ThenDoesNotNotifySubscriberAnymore()
        {
            var notified = false;
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);
            PropertyChangedEventHandler handler = (sender, args) => notified = true;

            manager.AddHandler(handler);
            manager.RemoveHandler(handler);

            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.False(notified);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAddedPropertyChangedHandlerTargetIsNotAlive_ThenDoesNotNotify()
        {
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);
            var subscriber = new TestSubscriber("Name", 1);

            manager.AddHandler(subscriber.PropertyChangedHandler);
            manager.NotifyChanged<Foo>(x => x.Name);

            Assert.Equal(1, subscriber.ActualCalls);

            subscriber = null;
            GC.Collect();

            manager.NotifyChanged<Foo>(x => x.Name);
        }

        [Ignore]
        [TestMethod, TestCategory("Unit")]
        public void WhenStaticMethodIsSubscribed_ThenNotifiesSubscriber()
        {
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);

            manager.SubscribeChanged((Expression<Func<Foo, object>>)(x => x.Name), (Action<Foo>)(OnStaticChanged));

            Assert.Throws<InvalidOperationException>(() => manager.NotifyChanged<Foo>(x => x.Name));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDisposingStaticSubscription_ThenDoesNotNotifySubscriber()
        {
            var foo = new Foo();
            var manager = new PropertyChangeManager(foo);

            var subscription = manager.SubscribeChanged((Expression<Func<Foo, object>>)(x => x.Name), (Action<Foo>)(OnStaticChanged));
            subscription.Dispose();

            manager.NotifyChanged<Foo>(x => x.Name);
        }

        private static void OnStaticChanged(Foo foo)
        {
            throw new InvalidOperationException();
        }

        public class TestSubscriber
        {
            public TestSubscriber(string propertyName, int expectedCalls)
            {
                this.PropertyName = propertyName;
                this.ExpectedCalls = expectedCalls;
            }

            public int ExpectedCalls { get; private set; }

            public int ActualCalls { get; private set; }

            public string PropertyName { get; private set; }

            public void SubscriptionHandler(Foo foo)
            {
                this.ActualCalls++;

                if (this.ActualCalls > this.ExpectedCalls)
                {
                    Assert.Fail("Maximum expected calls {0} has been reached", this.ExpectedCalls);
                }
            }

            public void PropertyChangedHandler(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == this.PropertyName)
                {
                    this.ActualCalls++;

                    if (this.ActualCalls > this.ExpectedCalls)
                    {
                        Assert.Fail("Maximum expected calls {0} has been reached", this.ExpectedCalls);
                    }
                }
            }
        }

        public class Foo : INotifyPropertyChanged
        {
            private PropertyChangeManager propertyChanges;
            private string name;
            private int value;

            public string Name
            {
                get
                {
                    return this.name;
                }

                set
                {
                    this.name = value;
                    this.PropertyChanges.NotifyChanged<Foo>(x => x.Name);
                }
            }

            public int Value
            {
                get
                {
                    return this.value;
                }

                set
                {
                    this.value = value;
                    this.PropertyChanges.NotifyChanged<Foo>(x => x.Value);
                }
            }

            public IDisposable SubscribeChanged(Expression<Func<Foo, object>> propertyExpression, Action<Foo> callbackAction)
            {
                return this.PropertyChanges.SubscribeChanged(propertyExpression, callbackAction);
            }

            public event PropertyChangedEventHandler PropertyChanged
            {
                add { this.PropertyChanges.AddHandler(value); }
                remove { this.PropertyChanges.RemoveHandler(value); }
            }

            private PropertyChangeManager PropertyChanges
            {
                get
                {
                    if (this.propertyChanges == null)
                    {
                        this.propertyChanges = new PropertyChangeManager(this);
                    }

                    return this.propertyChanges;
                }
            }
        }
    }
}
