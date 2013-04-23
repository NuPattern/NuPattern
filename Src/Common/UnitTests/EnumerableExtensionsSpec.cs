using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.UnitTests
{
    [TestClass]
    public class EnumerableExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenEnumerableIsNull_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.ForEach<string>(null, s => { }));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenActionIsNull_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.ForEach(Enumerable.Empty<string>(), null));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenForEachInvoked_ThenCallsActionWithSourceItems()
        {
            string arg = null;

            EnumerableExtensions.ForEach(new[] { "foo" }, s => arg = s);

            Assert.Equal("foo", arg);
        }
    }
}
