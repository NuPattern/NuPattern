using System.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Library.Events;

namespace NuPattern.Library.IntegrationTests.Events
{
    public class OnBuildStartedSpec
    {
        [DeploymentItem("Library.IntegrationTests.Content\\ClassLibrary\\", "Library.IntegrationTests.Content\\ClassLibrary\\")]
        [TestClass]
        public class GivenAnEmptySolution
        {
            internal static readonly IAssertion Assert = new Assertion();

            private IOnBuildStartedEvent buildStarted;

            [TestInitialize]
            public void Initialize()
            {
                var components = (IComponentModel)VsIdeTestHostContext.ServiceProvider.GetService(typeof(SComponentModel));

                this.buildStarted = components.GetService<IOnBuildStartedEvent>();
            }

            //// For some reason, I can manually test that these work, but can't automate it!
            //// The DTE events are not being raised :(
            [Ignore]
            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenBuildingSolution_ThenBuildStartedRaised()
            {
                var called = false;
                this.buildStarted.Subscribe(e => called = true);

                VsIdeTestHostContext.Dte.Solution.Open("Library.IntegrationTests.Content\\ClassLibrary\\ClassLibrary.sln");
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
