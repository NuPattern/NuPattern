using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.Events
{
	[TestClass]
	public class EventSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenCreatingWithNullSender_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => Event.Create<EventArgs>(null, EventArgs.Empty));
		}

		[TestMethod]
		public void WhenCreatingWithNullArgs_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => Event.Create<EventArgs>(new object(), null));
		}

		[TestMethod]
		public void WhenCreatingEvent_ThenInitializesProperties()
		{
			var sender = new object();
			var args = new EventArgs();

			var @event = Event.Create(sender, args);

			Assert.Same(sender, @event.Sender);
			Assert.Same(args, @event.EventArgs);
		}
	}
}
