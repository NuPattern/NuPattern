using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Moq;
using NuPattern.Diagnostics;
using TraceSourceExtensions = NuPattern.VisualStudio.TraceSourceExtensions;

namespace NuPattern.IntegrationTests.VisualStudio
{
    [TestClass]
    public class TraceSourceExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Integration")]
            public void WhenShieldingNullTraceSource_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => TraceSourceExtensions.ShieldUI(null, () => { }, "foo"));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShieldingNullAction_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => TraceSourceExtensions.ShieldUI(new Mock<ITraceSource>().Object, null, "foo"));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShieldingNullMessage_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => TraceSourceExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, null));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShieldingEmptyMessage_ThenThrowsArgumentOutOfRangeException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => TraceSourceExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, string.Empty));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShieldingNullMessageArgs_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => TraceSourceExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, "foo", null));
            }
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenShieldedActionThrows_ThenExceptionIsLogged()
        {
            var traceSource = new Mock<ITraceSource>();

            using (var purger = new DialogBoxPurger(0))
            {
                UIThreadInvoker.Invoke((Action)(() =>
                {
                    TraceSourceExtensions.ShieldUI(
                        traceSource.Object,
                        () => { throw new ArgumentException("Foo"); },
                        "Bar");
                }));
            }

            traceSource.Verify(x => x.TraceError(
                It.Is<ArgumentException>(ex => ex.Message == "Foo"),
                "Bar"),
                Times.Once());
        }
    }
}