using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Commands.Helpers
{
    [TestClass]
    public class CreateElementFromDroppedWindowsFileCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenUnresolvableTargetPath
        {
            private CreateElementFromDroppedWindowsFileCommand command;

            [TestInitialize]
            public void InitializeContext()
            {
                var solution = Mock.Of<ISolution>();
                var uriService = Mock.Of<IFxrUriReferenceService>();
                var currentElement = Mock.Of<IProductElement>();
                var importer = Mock.Of<IWindowsFileImporter>();

                this.command = new CreateElementFromDroppedWindowsFileCommand();
                this.command.CurrentElement = currentElement;
                this.command.UriService = uriService;
                this.command.Solution = solution;
                this.command.FileImporter = importer;
                this.command.TargetPath = "Foo";
                this.command.ChildElementName = "Element1";
                this.command.Extension = "txt";
            }

            //[TestMethod, TestCategory("Unit")]
            //public void ThenInitializeThrows()
            //{
            //    Assert.Throws<InvalidOperationException>(
            //        () => this.command.Execute());
            //}
        }

        [TestClass]
        public class GivenResolvableTargetPath
        {
            private CreateElementFromDroppedWindowsFileCommand command;

            [TestInitialize]
            public void InitializeContext()
            {
                var solution = Mock.Of<ISolution>();
                var uriService = Mock.Of<IFxrUriReferenceService>();
                var currentElement = Mock.Of<IProductElement>();
                var importer = Mock.Of<IWindowsFileImporter>();

                this.command = new CreateElementFromDroppedWindowsFileCommand();
                this.command.CurrentElement = currentElement;
                this.command.UriService = uriService;
                this.command.Solution = solution;
                this.command.FileImporter = importer;
                this.command.TargetPath = "Foo";
                this.command.ChildElementName = "Element1";
                this.command.Extension = "txt";
            }

            //[TestMethod, TestCategory("Unit")]
            //public void WhenTargetContainerNotExist_ThenInitializeCreatesContainer()
            //{
            //    this.command.Execute();

            //    Assert.Equal("Foo", this.command.FileImporter.TargetContainer.Name);
            //}

            //[TestMethod, TestCategory("Unit")]
            //public void WhenImportNewFile_ThenNewFileAdded()
            //{
            //    this.command.Execute();

            //    Assert.Fail();
            //}

            //[TestMethod, TestCategory("Unit")]
            //public void WhenImportExistingFile_ThenUniqueFileAdded()
            //{
            //    this.command.Execute();

            //    Assert.Fail();
            //}

            //[TestMethod, TestCategory("Unit")]
            //public void WhenGetItemWithUnknownFile_ThenReturnNull()
            //{
            //    this.command.Execute();

            //    Assert.Fail();
            //}

            //[TestMethod, TestCategory("Unit")]
            //public void WhenGetItemWithAddedFile_ThenReturnFile()
            //{
            //    this.command.Execute();

            //    Assert.Fail();
            //}
        }
    }
}
