using System.Threading;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Library.Events;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Library.IntegrationTests.Events
{
	public class OnBuildFinishedSpec
	{
		[DeploymentItem("Content\\ClassLibrary\\", "Content\\ClassLibrary\\")]
		[TestClass]
		public class GivenAnEmptySolution
		{
			internal static readonly IAssertion Assert = new Assertion();

			private IOnBuildFinishedEvent buildFinished;

			[TestInitialize]
			public void Initialize()
			{
				var components = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();

				this.buildFinished = components.GetService<IOnBuildFinishedEvent>();
			}

			//// For some reason, I can manually test that these work, but can't automate it!
			//// The DTE events are not being raised :(
			[Ignore]
			[HostType("VS IDE")]
			[TestMethod]
			public void WhenBuildingSolution_ThenBuildFinishedRaised()
			{
				var called = false;
				this.buildFinished.Subscribe(e => called = true);

				VsIdeTestHostContext.Dte.Solution.Open("Content\\ClassLibrary\\ClassLibrary.sln");

				while (!VsIdeTestHostContext.Dte.Solution.IsOpen)
				{
					Thread.Sleep(10);
				}

				VsIdeTestHostContext.Dte.Solution.SolutionBuild.Build(true);

				while (VsIdeTestHostContext.Dte.Solution.SolutionBuild.BuildState != EnvDTE.vsBuildState.vsBuildStateDone)
				{
					Thread.Sleep(10);
				}

				Assert.True(called);
			}
		}
	}
}
