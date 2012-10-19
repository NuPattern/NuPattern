using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
	[TestClass]
	[SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
	public class ShellEventsSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		private ShellEvents shellEvents;
		private bool initializeRaised;

		[TestInitialize]
		public void Initialize()
		{
			this.shellEvents = new ShellEvents(ServiceProvider.GlobalProvider);
			this.shellEvents.ShellInitialized += (sender, args) => initializeRaised = true;
			this.shellEvents.ShellInitialized += OnShellInitalized;
		}

		private void OnShellInitalized(object sender, EventArgs args)
		{
			this.initializeRaised = true;
		}

		[HostType("VS IDE")]
		[TestMethod]
		public void WhenShellInitializeRaised_ThenShellIsInitializedIsTrue()
		{
			while (!this.initializeRaised &&
				/* It might be too late for the test runner to catch the shell event */
				!this.shellEvents.IsInitialized)
			{
				Thread.Sleep(100);
			}

			Assert.True(this.shellEvents.IsInitialized);
		}
	}
}
