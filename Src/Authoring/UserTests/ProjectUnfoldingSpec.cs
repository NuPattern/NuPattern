using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Extensibility;

namespace NuPattern.Authoring.UserTests
{
    public class ProjectUnfoldingSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [CodedUITest]
        public class GivenNoSolution: CodedUITest
        {
            private ISolution solution;
            private ITemplate templateToUnfold;
            private IProject unfoldedProject;

            public UIMap UIMap
            {
                get
                {
                    if ((this.map == null))
                    {
                        this.map = new UIMap();
                    }

                    return this.map;
                }
            }

            private UIMap map;

            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.SelectProjectTemplate("Pattern Toolkit");
            }

            [TestCleanup]
            public void Cleanup()
            {
                base.Cleanup();
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void WhenCreateNewPatternToolkitProject_ThenCreated()
            {
                this.CreateNewProject("PatternToolkit1");

                // Psuedo Assertions
                //Assert.True(Toolkit Project is created);
                //Assert.True(ToolkitLibrary Project is created);
                //Assert.True(Solution Builder contains toolkitproject node);
                //Assert.True(Solution Builder contains toolkitprojectlibrary node);
                //Assert.True(toolkitproject node is called same as toolkit project);
                //Assert.True(toolkitprojectlibrary node is called same as library project);

                //this.UIMap.UnfoldPatternToolkitProject();

            }
        }
    }
}
