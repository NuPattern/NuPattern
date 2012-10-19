using System;
using Microsoft.VisualStudio.Patterning.Library.Events;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Events
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
    [TestClass]
    public class OnElementActivatedSpec
    {
        private Mock<IPatternManager> manager;
        private OnElementActivatedEvent publisher;
        private IProductElement current;

        [TestInitialize]
        public void Initialize()
        {
            this.manager = new Mock<IPatternManager>();
            this.current = Mock.Of<IProductElement>();

            this.publisher = new OnElementActivatedEvent(this.manager.Object, this.current);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.publisher.Dispose();
        }

        [TestMethod]
        public void WhenPatternManagerOpenedAndNoSubscribers_ThenNoOp()
        {
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);
        }

        [TestMethod]
        public void WhenManagerOpenedThenProductActivated_ThenSubscriberIsNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Raise(x => x.ElementActivated += null, new ValueEventArgs<IProductElement>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)));
        }

        [TestMethod]
        public void WhenManagerOpenedThenProductActivatedForOtherElement_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Raise(x => x.ElementActivated += null, new ValueEventArgs<IProductElement>(Mock.Of<IProductElement>()));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)), Times.Never());
        }

        [TestMethod]
        public void WhenManagerClosedThenProductActivated_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Setup(x => x.IsOpen).Returns(false);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            this.manager.Raise(x => x.ElementActivated += null, new ValueEventArgs<IProductElement>(this.current));

            subscriber.Verify(x => x.OnNext(
                It.Is<IEvent<EventArgs>>(e => e.Sender == this.current)),
                Times.Never());
        }

        [TestMethod]
        public void WhenDisposingSubscriptionThenProductActivated_ThenSubscriberIsNotNotified()
        {
            var subscriber = new Mock<ISubscriber>();
            var subscription = this.publisher.Subscribe(subscriber.Object.OnNext);

            // Open the manager to cause subscription of everything.
            this.manager.Setup(x => x.IsOpen).Returns(true);
            this.manager.Raise(x => x.IsOpenChanged += null, EventArgs.Empty);

            subscription.Dispose();

            this.manager.Raise(x => x.ElementActivated += null, new ValueEventArgs<IProductElement>(this.current));

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
