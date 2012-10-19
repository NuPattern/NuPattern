using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class WeakObservableSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenNullAddHandler_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				WeakObservable.FromEvent<EventArgs>(null, handler => { }));
		}

		[TestMethod]
		public void WhenNullRemoveHandler_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				WeakObservable.FromEvent<EventArgs>(handler => { }, null));
		}

		[TestMethod]
		public void WhenNullConversionHandler_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() =>
				WeakObservable.FromEvent<EventHandler, EventArgs>(null, handler => { }, handler => { }));
		}
	}
}
