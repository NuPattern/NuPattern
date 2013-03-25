using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NuPattern.UnitTests
{
    [TestClass]
    public class WeakObserverEventSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenOnNextIsNull_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WeakObserverEvent<EventArgs>(null, null, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSubscriberIsAlive_ThenObserverIsAlive()
        {
            var subscriber = new Mock<ISubscriber>();

            var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, null, null);

            Assert.True(observer.IsAlive);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenSubscriberIsNotAlive_ThenObserverIsNotAlive()
        {
            var subscriber = new Mock<ISubscriber>();

            var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, null, null);

            subscriber = null;
            GC.Collect();

            Assert.False(observer.IsAlive);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCompleted_ThenInvokesCallback()
        {
            var subscriber = new Mock<ISubscriber>();
            var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, subscriber.Object.OnError, subscriber.Object.OnCompleted);

            observer.OnCompleted();

            subscriber.Verify(x => x.OnCompleted());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenError_ThenInvokesCallback()
        {
            var subscriber = new Mock<ISubscriber>();
            var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, subscriber.Object.OnError, subscriber.Object.OnCompleted);
            var exception = new InvalidOperationException();

            observer.OnError(exception);

            subscriber.Verify(x => x.OnError(exception));
        }

        public interface ISubscriber
        {
            void OnNext(IEvent<EventArgs> data);
            void OnCompleted();
            void OnError(Exception exception);
        }
    }
}
