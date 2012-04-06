using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Windows;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Patterning.Runtime.UriProviders;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Menu
{
    [TestClass]
    public class ImageUriEditorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private ITypeDescriptorContext context;
        private Mock<IServiceProvider> serviceProvider;
        private Solution solution;
        private Mock<IComponentModel> componentModel;
        private Mock<ISolutionPicker> picker;
        private Project project;
        private Folder folder;
        private Item item;

        [TestInitialize]
        public virtual void Initialize()
        {
            context = Mock.Of<ITypeDescriptorContext>();

            solution = new Solution();
            project = new Project { Name = "project", PhysicalPath = @"c:\projects\solution\project\project.csproj" };
            folder = new Folder();
            item = new Item { Data = { CustomTool = "", IncludeInVSIX = "false", CopyToOutputDirectory = CopyToOutput.DoNotCopy, ItemType = "None" }, PhysicalPath = @"c:\projects\solution\project\assets\icon.ico" };
            folder.Items.Add(item);
            project.Items.Add(folder);
            project.Data.AssemblyName = "project";
            solution.Items.Add(project);

            serviceProvider = new Mock<IServiceProvider>();
            componentModel = new Mock<IComponentModel>();
            picker = new Mock<ISolutionPicker>();
            var uriProvider = new PackUriProvider();

            var pack = new ResourcePack(item);

            var uriService = new Mock<IFxrUriReferenceService>();
            uriService.Setup(u => u.CreateUri<ResourcePack>(It.IsAny<ResourcePack>(), "pack")).Returns(uriProvider.CreateUri(pack));

            serviceProvider.Setup(s => s.GetService(typeof(SComponentModel))).Returns(componentModel.Object);
            serviceProvider.Setup(s => s.GetService(typeof(ISolution))).Returns(solution);
            serviceProvider.Setup(s => s.GetService(typeof(IFxrUriReferenceService))).Returns(uriService.Object);
            componentModel.Setup(c => c.GetService<Func<ISolutionPicker>>()).Returns(new Func<ISolutionPicker>(() => { return picker.Object; }));

            picker.Setup(p => p.Filter).Returns(Mock.Of<IPickerFilter>());
        }

        [TestClass]
        public class WhenCancelDialog : ImageUriEditorSpec
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                picker.Setup(p => p.ShowDialog()).Returns(false);
                picker.Setup(p => p.SelectedItem).Returns(item);
            }

            [TestMethod]
            public void WhenCancelDialog_ThenReturnPreviousValue()
            {
                var editor = new ImageUriEditor(new Window());

                picker.Setup(p => p.ShowDialog()).Returns(false);

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("image", value);
            }


            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "value"), TestMethod]
            public void ItemTypeIsUnchanged()
            {
                item.Data.ItemType = "None";
                var editor = new ImageUriEditor(new Window());

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("None", (string)item.Data.ItemType);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "value"), TestMethod]
            public void CustomToolIsUnchanged()
            {
                item.Data.CustomTool = "MyTool";
                var editor = new ImageUriEditor(new Window());

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("MyTool", (string)item.Data.CustomTool);
            }
        }

        [TestClass]
        public class WhenAcceptDialog : ImageUriEditorSpec
        {
            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                picker.Setup(p => p.ShowDialog()).Returns(true);
                picker.Setup(p => p.SelectedItem).Returns(item);
            }

            [TestMethod]
            public void ReturnUriForComponent()
            {
                var editor = new ImageUriEditor(new Window());

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("pack://application:,,,/project;component/assets/icon.ico", value);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "value"), TestMethod]
            public void SetResourceAsItemType()
            {
                var editor = new ImageUriEditor(new Window());

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("Resource", (string)item.Data.ItemType);
            }

            [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "value"), TestMethod]
            public void CustomToolIsUnchanged()
            {
                item.Data.CustomTool = "MyTool";
                var editor = new ImageUriEditor(new Window());

                var value = editor.EditValue(context, serviceProvider.Object, "image");

                Assert.Equal("MyTool", (string)item.Data.CustomTool);
            }
        }

        [TestMethod]
        public void WhenContextIsNotNull_EditStyleIsModal()
        {
            var editor = new ImageUriEditor(new Window());

            var context = Mock.Of<ITypeDescriptorContext>();

            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(context));
        }

        [TestMethod]
        public void WhenContextIsNull_EditStyleIsModal()
        {
            var editor = new ImageUriEditor(new Window());

            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(null));
        }

        [TestMethod]
        public void WhenContextIsNotPassed_EditStyleIsModal()
        {
            var editor = new ImageUriEditor(new Window());

            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle());
        }

    }
}
