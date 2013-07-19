using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.ShortcutProviders;
using NuPattern.Runtime.Shell.Shortcuts;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests.Shell.Shortcuts
{
    public class ShortcutSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [DeploymentItem(@"Runtime.IntegrationTests.Content\Shortcuts", @"Runtime.IntegrationTests.Content\Shortcuts")]
        [TestClass]
        public class GivenASolution : IntegrationTest
        {
            private ISolution solution;
            private IShortcutLaunchService launchService;
            private ISolutionFolder solutionItemsFolder;
            private IGuidanceManager guidanceManager;

            [TestInitialize]
            public void Initialize()
            {
                this.guidanceManager = VsIdeTestHostContext.ServiceProvider.GetService<IGuidanceManager>();
                this.guidanceManager.ActiveGuidanceExtension = null;
                this.launchService = VsIdeTestHostContext.ServiceProvider.GetService<IShortcutLaunchService>();
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
                this.solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\Shortcuts.sln"));
                this.solutionItemsFolder = this.solution.SolutionFolders.Single(sf => sf.Name == NuPattern.VisualStudio.Solution.SolutionExtensions.SolutionItemsFolderName);
            }

            [TestCleanup]
            public void Cleanup()
            {
                if (this.solution != null
                    && this.solution.IsOpen)
                {
                    this.solution.Close(false);
                }
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpenEmptyShortcut_ThenErrorMessage()
            {
                var shortcutFile = this.solution.Find<IItem>(@"Solution Items\EmptyGuidanceShortcut.guidance").First();

                using (new DialogBoxPurger(0))
                {
                    UIThreadInvoker.Invoke((Action)(shortcutFile.Open));
                }

                Assert.False(this.guidanceManager.InstantiatedGuidanceExtensions.Any());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpenUnregisteredGuidance_ThenErrorMessage()
            {
                var shortcutFile = this.solution.Find<IItem>(@"Solution Items\InvalidGuidanceShortcut.guidance").First();

                using (new DialogBoxPurger(0))
                {
                    UIThreadInvoker.Invoke((Action)(shortcutFile.Open));
                }

                Assert.False(this.guidanceManager.InstantiatedGuidanceExtensions.Any());
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpenRegisteredGuidance_ThenCreatesAndActivatesGuidance()
            {
                var shortcutFile = this.solution.Find<IItem>(@"Solution Items\ValidGuidanceShortcut.guidance").First();
                shortcutFile.Open();

                Assert.Equal(1, this.guidanceManager.InstantiatedGuidanceExtensions.Count());
                Assert.NotNull(this.guidanceManager.ActiveGuidanceExtension);
                Assert.Equal("foo", this.guidanceManager.ActiveGuidanceExtension.InstanceName);
            }
        }
    }
}
