using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.UserTests
{
    public class ProjectUnfoldingSpec
    {
        [CodedUITest]
        public class GivenAllProjectTemplates: AddNewProjectTest
        {
            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void ThenPatternToolkitProjectTemplateInstalled()
            {
                this.SearchForProjectTemplate("Pattern Toolkit");
                this.AssertProjectTemplateSelected("PatternToolkit");

                // Close the dialog
                Assert.True(this.UIMap.UINewProjectWindow.UICancelButton.Enabled);
                Mouse.Click(this.UIMap.UINewProjectWindow.UICancelButton);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void ThenPatternToolkitLibraryProjectTemplateInstalled()
            {
                this.SearchForProjectTemplate("Pattern Toolkit Library");
                this.AssertProjectTemplateSelected("PatternToolkitLibrary");

                // Close the dialog
                Assert.True(this.UIMap.UINewProjectWindow.UICancelButton.Enabled);
                Mouse.Click(this.UIMap.UINewProjectWindow.UICancelButton);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void WhenCreateNewPatternToolkitProject_ThenWizardDisplayed()
            {
                this.SearchForProjectTemplate("Pattern Toolkit");
                var projectName = this.CreateNewProjectFromSelectedTemplate();

                Assert.NotNull(this.UIMap.UINewPatternToolkitWindow);

                // Dismiss Wizard
                Assert.True(this.UIMap.UINewPatternToolkitWindow.UICancelButton.Enabled);
                Mouse.Click(this.UIMap.UINewPatternToolkitWindow.UICancelButton);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void WhenCreateNewPatternToolkitProject_ThenWizardPopulated()
            {
                this.SearchForProjectTemplate("Pattern Toolkit");
                var projectName = this.CreateNewProjectFromSelectedTemplate();

                Assert.Equal(true, this.UIMap.UINewPatternToolkitWindow.UICreateanewtoolkitRadioButton.Selected);

                // Move to next page
                Assert.True(this.UIMap.UINewPatternToolkitWindow.UINextButton.Enabled);
                Mouse.Click(this.UIMap.UINewPatternToolkitWindow.UINextButton);

                Assert.Equal(projectName, this.UIMap.UINewPatternToolkitWindow.UIToolkitNameEditorEdit.Text);
                Assert.Equal("A description of the toolkit.", this.UIMap.UINewPatternToolkitWindow.UIPART_EditEdit.Text);
                Assert.Equal("Unknown", this.UIMap.UINewPatternToolkitWindow.UIPART_EditEdit1.Text);
                Assert.Equal("MyPattern", this.UIMap.UINewPatternToolkitWindow.UIPART_EditEdit2.Text);
                Assert.Equal("A Description of MyPattern", this.UIMap.UINewPatternToolkitWindow.UIPART_EditEdit3.Text);

                // Dismiss Wizard
                Assert.True(this.UIMap.UINewPatternToolkitWindow.UICancelButton.Enabled);
                Mouse.Click(this.UIMap.UINewPatternToolkitWindow.UICancelButton);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void WhenCreateNewPatternToolkitProject_ThenNewPatternToolkitProjectCreated()
            {
                this.SearchForProjectTemplate("Pattern Toolkit");
                var projectName = this.CreateNewProjectFromSelectedTemplate();

                // Complete Wizard
                Assert.True(this.UIMap.UINewPatternToolkitWindow.UINextButton.Enabled);
                Mouse.Click(this.UIMap.UINewPatternToolkitWindow.UINextButton);
                Assert.True(this.UIMap.UINewPatternToolkitWindow.UIFinishButton.Enabled);
                Mouse.Click(this.UIMap.UINewPatternToolkitWindow.UIFinishButton);

                // Wait until project finishes being created
                this.UIMap.UIVsMainWindow.WaitForControlReady(30000);

                var dte = (EnvDTE.DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(EnvDTE.DTE));
                Assert.NotNull(dte);

                // Verify active document is the PatternModel
                Assert.Equal("PatternModel.patterndefinition", dte.ActiveDocument.Name);

                // Switch to Solution Explorer
                Keyboard.SendKeys("WS", ModifierKeys.Control);

                var solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                Assert.NotNull(solution);

                // Verify projects are created
                Assert.NotNull(solution.Find<IProject>(projectName).FirstOrDefault());
                Assert.NotNull(solution.Find<IProject>(projectName + ".Automation").FirstOrDefault());

                //Switch to Solution Builder
                Keyboard.SendKeys("WH", ModifierKeys.Control);

                var patternManager = (IPatternManager)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IPatternManager));
                Assert.NotNull(patternManager);

                // Verify solution elements created
                Assert.NotNull(patternManager.Find(projectName));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("User Interactive")]
            public void WhenCreateNewPatternToolkitProject_ThenNewPatternToolkitLibraryProjectCreated()
            {
                this.SearchForProjectTemplate("Pattern Toolkit Library");
                var projectName = this.CreateNewProjectFromSelectedTemplate();

                // Wait until project finishes being created
                this.UIMap.UIVsMainWindow.WaitForControlReady(30000);

                var dte = (EnvDTE.DTE)VsIdeTestHostContext.ServiceProvider.GetService(typeof(EnvDTE.DTE));
                Assert.NotNull(dte);

                // Switch to Solution Explorer
                Keyboard.SendKeys("WS", ModifierKeys.Control);

                var solution = (ISolution)VsIdeTestHostContext.ServiceProvider.GetService(typeof(ISolution));
                Assert.NotNull(solution);

                // Verify projects are created
                Assert.NotNull(solution.Find<IProject>(projectName).FirstOrDefault());

                //Switch to Solution Builder
                Keyboard.SendKeys("WH", ModifierKeys.Control);

                var patternManager = (IPatternManager)VsIdeTestHostContext.ServiceProvider.GetService(typeof(IPatternManager));
                Assert.NotNull(patternManager);

                // Verify solution elements created
                Assert.NotNull(patternManager.Find(projectName));
            }
        }
    }
}
