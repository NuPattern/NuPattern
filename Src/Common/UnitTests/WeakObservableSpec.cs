using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.UnitTests
{
    [TestClass]
    public class WeakObservableSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenNullAddHandler_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                WeakObservable.FromEvent<EventArgs>(null, handler => { }));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNullRemoveHandler_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                WeakObservable.FromEvent<EventArgs>(handler => { }, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNullConversionHandler_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                WeakObservable.FromEvent<EventHandler, EventArgs>(null, handler => { }, handler => { }));
        }
    }
}
