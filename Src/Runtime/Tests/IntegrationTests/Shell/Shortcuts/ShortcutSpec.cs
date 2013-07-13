using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests.Shell.Shortcuts
{
    public class ShortcutSpec
    {
        [DeploymentItem(@"Runtime.IntegrationTests.Content\Shortcuts", @"Runtime.IntegrationTests.Content\Shortcuts")]
        [TestClass]
        public class GivenASolution : IntegrationTest
        {
            private ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
                this.solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\Shortcuts.sln"));
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
            public void WhenEmptyGuidanceShortcut_ThenMessage()
            {
                var shortcutFile = this.solution.Find<IItem>(@"Solution Items\EmptyGuidanceShortcut.guidance").First();

                using (new DialogBoxPurger(0))
                {
                    UIThreadInvoker.Invoke((Action)(shortcutFile.Open));
                }
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenEmptyPatternShortcut_ThenMessage()
            {
                var shortcutFile = this.solution.Find<IItem>(@"Solution Items\EmptyPatternShortcut.pattern").First();

                using (new DialogBoxPurger(0))
                {
                    UIThreadInvoker.Invoke((Action)(shortcutFile.Open));
                }
            }
        }
    }
}
