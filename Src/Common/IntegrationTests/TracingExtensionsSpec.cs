using System;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Common.IntegrationTests
{
	[TestClass]
	public class TracingExtensionsSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod, TestCategory("Integration")]
			public void WhenShieldingNullTraceSource_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => TracingExtensions.ShieldUI(null, () => { }, "foo"));
			}

			[TestMethod, TestCategory("Integration")]
			public void WhenShieldingNullAction_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => TracingExtensions.ShieldUI(new Mock<ITraceSource>().Object, null, "foo"));
			}

			[TestMethod, TestCategory("Integration")]
			public void WhenShieldingNullMessage_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => TracingExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, null));
			}

			[TestMethod, TestCategory("Integration")]
			public void WhenShieldingEmptyMessage_ThenThrowsArgumentOutOfRangeException()
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => TracingExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, string.Empty));
			}

			[TestMethod, TestCategory("Integration")]
			public void WhenShieldingNullMessageArgs_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => TracingExtensions.ShieldUI(new Mock<ITraceSource>().Object, () => { }, "foo", null));
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
					TracingExtensions.ShieldUI(
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