using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Events;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.Events
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
    [TestClass]
    public class OnElementInstantiatedSpec
    {
        private Mock<IPatternManager> manager;
        private Mock<IProductState> store;
        private OnElementInstantiatedEvent publisher;
        private IInstanceBase current;

        [TestInitialize]
        public void Initialize()
        {
            this.manager = new Mock<IPatternManager>();
            this.store = new Mock<IProductState>();
            this.current = Mock.Of<IInstanceBase>();
            this.manager.Setup(x => x.Store).Returns(this.store.Object);

            this.publisher = new OnElementInstantiatedEvent(this.manager.Object, this.current);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.publisher.Dispose();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPatternManagerOpenedAndNoSubscribers_ThenNoOp()
        {
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerOpenedThenProductInstantiated_ThenSubscriberIsNotifiedIfValueIsCurrentElement()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerOpenedThenProductInstantiated_ThenSubscriberIsNotNotifiedIfValueIsNotCurrentElement()
        {
            var product = new Mock<IProduct>();
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IInstanceBase>(product.Object));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == product.Object)), Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerClosedThenProductInstantiated_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Setup(x => x.IsOpen).Returns(false);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)),
                Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDisposingSubscriptionThenProductInstantiated_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            var subscription = this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            subscription.Dispose();

            this.store.Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)),
                Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerClosedAndReopenedAndProductInstantiated_ThenOldSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Setup(x => x.IsOpen).Returns(false);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            // Create a new state. Old one should be inactive and no longer 
            // notifying anyone.
            var newStore = new Mock<IProductState>();
            this.manager.Setup(x => x.Store).Returns(newStore.Object);
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            // Raise element instantiated on the old state.
            this.store.Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)),
                Times.Never());
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
