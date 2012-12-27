using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Events;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.Events
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
    [TestClass]
    public class OnElementDeletedEventSpec
    {
        private Mock<IPatternManager> manager;
        private Mock<IProductState> store;
        private IInstanceBase current;
        private OnElementDeletedEvent publisher;

        [TestInitialize]
        public void Initialize()
        {
            this.manager = new Mock<IPatternManager>();
            this.store = new Mock<IProductState>();
            this.current = Mock.Of<IInstanceBase>();
            this.manager.Setup(x => x.Store).Returns(this.store.Object);

            this.publisher = new OnElementDeletedEvent(this.manager.Object, this.current);
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
        public void WhenManagerOpenedThenProductDeleted_ThenSubscriberIsNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementDeleted += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerOpenedThenProductDeletedForOtherElement_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementDeleted += null, new ValueEventArgs<IInstanceBase>(Mock.Of<IInstanceBase>()));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)), Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenManagerClosedThenProductDeleted_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Setup(x => x.IsOpen).Returns(false);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.store.Raise(x => x.ElementDeleted += null, new ValueEventArgs<IInstanceBase>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)),
                Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDisposingSubscriptionThenProductDeleted_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            var subscription = this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            subscription.Dispose();

            this.store.Raise(x => x.ElementDeleted += null, new ValueEventArgs<IInstanceBase>(this.current));

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