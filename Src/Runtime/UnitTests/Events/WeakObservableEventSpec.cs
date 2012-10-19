using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class WeakObservableEventSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenConstructed_ThenDoesNotAttachToSource()
		{
			var added = false;
			var removed = false;

			var observable = new WeakObservableEvent<EventHandler, EventArgs>(
				handler => new EventHandler(handler.Invoke),
				handler => added = true,
				handler => removed = true);

			Assert.False(added);
			Assert.False(removed);
		}

		[TestMethod]
		public void WhenFirstSubscriberAdded_ThenAttachesToSource()
		{
			var added = 0;
			var removed = 0;

			var observable = new WeakObservableEvent<EventHandler, EventArgs>(
				handler => new EventHandler(handler.Invoke),
				handler => added++,
				handler => removed++);

			observable.Subscribe(new Mock<IObserver<IEvent<EventArgs>>>().Object);

			Assert.Equal(1, added);
			Assert.Equal(0, removed);

			observable.Subscribe(new Mock<IObserver<IEvent<EventArgs>>>().Object);

			Assert.Equal(1, added);
			Assert.Equal(0, removed);
		}

		[TestMethod]
		public void WhenLastSubscriberRemovedByDisposingSubscription_ThenDetachesFromSource()
		{
			var added = 0;
			var removed = 0;
			var subscriber = new Mock<IObserver<IEvent<EventArgs>>>();
			var observable = new WeakObservableEvent<EventHandler, EventArgs>(
				handler => new EventHandler(handler.Invoke),
				handler => added++,
				handler => removed++);

			var subscription = observable.Subscribe(subscriber.Object);

			subscription.Dispose();

			Assert.Equal(1, added);
			Assert.Equal(1, removed);
		}

		[TestMethod]
		public void WhenSourceEventRaised_ThenInvokesOnNextOnSubscriber()
		{
			EventHandler raiseHandler = null;
			EventArgs args = new EventArgs();
			var subscriber = new Mock<IObserver<IEvent<EventArgs>>>();
			var observable = new WeakObservableEvent<EventHandler, EventArgs>(
				handler => new EventHandler(handler.Invoke),
				handler => raiseHandler = handler,
				handler => { });

			var subscription = observable.Subscribe(subscriber.Object);

			raiseHandler(this, args);

			subscriber.Verify(x => x.OnNext(It.Is<IEvent<EventArgs>>(e => e.EventArgs == args)));
		}

		[TestMethod]
		public void WhenSourceEventRaisedAndWeakSubscriberDisposed_ThenRemovesSubscriber()
		{
			EventHandler raiseHandler = null;
			var removed = 0;
			var observer = new Mock<IObserver<IEvent<EventArgs>>>();
			var weakSubscriber = new WeakObserverEvent<EventArgs>(observer.Object.OnNext, observer.Object.OnError, observer.Object.OnCompleted);
			var observable = new WeakObservableEvent<EventHandler, EventArgs>(
				handler => new EventHandler(handler.Invoke),
				handler => raiseHandler = handler,
				handler => removed++);

			observable.Subscribe(weakSubscriber);

			Assert.NotNull(raiseHandler);
			Assert.Equal(0, removed);

			observer = null;
			GC.Collect();

			raiseHandler(this, EventArgs.Empty);

			Assert.Equal(1, removed);
		}
	}
}
