using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class ObservableEventExtensionsSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenSourceIsNullWithOnNext_ThenThrowsArgumentNullException()
		{
			IObservable<IEvent<EventArgs>> source = null;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe(e => { }));
		}

		[TestMethod]
		public void WhenSourceIsNullWithOnNextAndOnCompleted_ThenThrowsArgumentNullException()
		{
			IObservable<IEvent<EventArgs>> source = null;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe(e => { }, () => { }));
		}

		[TestMethod]
		public void WhenSourceIsNullWithOnNextAndOnCompletedAndOnError_ThenThrowsArgumentNullException()
		{
			IObservable<IEvent<EventArgs>> source = null;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe(e => { }, e => { }, () => { }));
		}

		[TestMethod]
		public void WhenOnNextIsNull_ThenThrowsArgumentNullException()
		{
			var source = new Mock<IObservable<IEvent<EventArgs>>>().Object;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe((Action<IEvent<EventArgs>>)null));
		}

		[TestMethod]
		public void WhenOnNextIsNullWithOnCompleted_ThenThrowsArgumentNullException()
		{
			var source = new Mock<IObservable<IEvent<EventArgs>>>().Object;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe((Action<IEvent<EventArgs>>)null, () => { }));
		}

		[TestMethod]
		public void WhenOnNextIsNullWithOnCompletedAndOnError_ThenThrowsArgumentNullException()
		{
			var source = new Mock<IObservable<IEvent<EventArgs>>>().Object;

			Assert.Throws<ArgumentNullException>(() =>
				source.Subscribe((Action<IEvent<EventArgs>>)null, e => { }, () => { }));
		}
	}
}
