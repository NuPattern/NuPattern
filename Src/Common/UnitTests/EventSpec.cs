using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.Common.UnitTests
{
    [TestClass]
    public class EventSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenCreatingWithNullSender_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Event.Create<EventArgs>(null, EventArgs.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCreatingWithNullArgs_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Event.Create<EventArgs>(new object(), null));
        }

        [TestMethod, TestCategory("Unit")]
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
