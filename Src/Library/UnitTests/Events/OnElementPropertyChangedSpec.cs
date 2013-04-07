using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Events;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.Events
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
    [TestClass]
    public class OnElementPropertyChangedEventSpec
    {
        private Mock<IProductElement> current;
        private OnElementPropertyChangedEvent publisher;

        [TestInitialize]
        public void Initialize()
        {
            this.current = new Mock<IProductElement>();
            this.current.Setup(x => x.InstanceName).Returns("current");

            this.publisher = new OnElementPropertyChangedEvent(this.current.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.publisher.Dispose();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerOpenedThenPropertyChanged_ThenSubscriberIsNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            this.current.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<PropertyChangedEventArgs>>(e => e.EventArgs.PropertyName == "Foo")));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerOpenedThenPropertyChangedForOtherElement_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            var otherElement = new Mock<IProductElement>();
            otherElement.Setup(x => x.InstanceName).Returns("other");
            var otherPublisher = new OnElementPropertyChangedEvent(otherElement.Object);
            otherPublisher.Subscribe(subscriber.Object.OnNext);

            otherElement.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<PropertyChangedEventArgs>>(e => e.EventArgs.PropertyName == "Foo")));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<PropertyChangedEventArgs>>(e => e.Sender == this.current)), Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDisposingSubscriptionThenPropertyChanged_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            var subscription = this.publisher.Subscribe(subscriber.Object.OnNext);

            subscription.Dispose();

            this.current.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

            subscriber.Verify(x => x.OnNext(It.IsAny<IEvent<PropertyChangedEventArgs>>()), Times.Never());
        }

        public interface ISubscriber
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Intened design")]
            void OnNext(IEvent<EventArgs> data);
            void OnCompleted();
            void OnError(Exception exception);
        }
    }
}
