using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class WeakObserverEventSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenOnNextIsNull_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new WeakObserverEvent<EventArgs>(null, null, null));
		}

		[TestMethod]
		public void WhenSubscriberIsAlive_ThenObserverIsAlive()
		{
			var subscriber = new Mock<ISubscriber>();

			var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, null, null);

			Assert.True(observer.IsAlive);
		}

		[TestMethod]
		public void WhenSubscriberIsNotAlive_ThenObserverIsNotAlive()
		{
			var subscriber = new Mock<ISubscriber>();

			var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, null, null);

			subscriber = null;
			GC.Collect();

			Assert.False(observer.IsAlive);
		}

		[TestMethod]
		public void WhenCompleted_ThenInvokesCallback()
		{
			var subscriber = new Mock<ISubscriber>();
			var observer = new WeakObserverEvent<EventArgs>(subscriber.Object.OnNext, subscriber.Object.OnError, subscriber.Object.OnCompleted);

			observer.OnCompleted();

			subscriber.Verify(x => x.OnCompleted());
		}

		[TestMethod]
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
