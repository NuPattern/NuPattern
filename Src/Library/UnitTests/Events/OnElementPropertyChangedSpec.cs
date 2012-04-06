using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Library.Events;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Events
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
	[TestClass]
	public class OnElementPropertyChangedEventSpec
	{
		private Mock<IProductElement> productElement;
		private OnElementPropertyChangedEvent publisher;

		[TestInitialize]
		public void Initialize()
		{
			this.productElement = new Mock<IProductElement>();

			this.publisher = new OnElementPropertyChangedEvent(this.productElement.Object);
		}

		[TestCleanup]
		public void Cleanup()
		{
			this.publisher.Dispose();
		}

		[TestMethod]
		public void WhenManagerOpenedThenPropertyChanged_ThenSubscriberIsNotified()
		{
			var subscriber = new Mock<ISubscriber>();
			this.publisher.Subscribe(subscriber.Object.OnNext);

			this.productElement.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

			subscriber.Verify(x => x.OnNext(
				It.Is<IEvent<PropertyChangedEventArgs>>(e => e.EventArgs.PropertyName == "Foo")));
		}

		[TestMethod]
		public void WhenDisposingSubscriptionThenPropertyChanged_ThenSubscriberIsNotNotified()
		{
			var subscriber = new Mock<ISubscriber>();
			var subscription = this.publisher.Subscribe(subscriber.Object.OnNext);

			subscription.Dispose();

			this.productElement.Raise(x => x.PropertyChanged += null, new PropertyChangedEventArgs("Foo"));

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
