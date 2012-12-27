using System.Windows.Input;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.Authoring.UserTests
{
    [CodedUITest]
    public abstract class AddNewProjectTest : UserTest
    {
        private VsUIMap map;

        protected VsUIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new VsUIMap();
                }

                return this.map;
            }
        }

        protected virtual void OpenAddNewProjectDialog()
        {
            // Open AddNewProject dialog
            Keyboard.SendKeys("N", (ModifierKeys.Control | ModifierKeys.Shift));
        }

        protected virtual void SearchForProjectTemplate(string searchText)
        {
            //Set caret to Search control
            Keyboard.SendKeys("E", ModifierKeys.Control);

            // Enter the  search text
            Keyboard.SendKeys(searchText, ModifierKeys.None);

            // Delay waiting for template search to complete
            Playback.Wait(2000);
        }

        protected virtual string CreateNewProjectFromSelectedTemplate()
        {
            // Save the current project name
            var projectName = this.UIMap.UINewProjectWindow.UINameEdit.Text;

            // Create the project
            Mouse.Click(this.UIMap.UINewProjectWindow.UIOKButton);

            return projectName;
        }

        protected virtual void AssertProjectTemplateSelected(string newProjectName)
        {
            var projectName = this.UIMap.UINewProjectWindow.UINameEdit.Text;

            // Remove trailing numerics
            projectName = projectName.TrimEnd("0123456789".ToCharArray());

            // Aseert project name
            Assert.Equal(newProjectName, projectName);
        }

        [TestInitialize]
        public override void InitializeContext()
        {
            base.InitializeContext();

            // Open the AddNewProject dialog
            this.OpenAddNewProjectDialog();
        }
    }
}
