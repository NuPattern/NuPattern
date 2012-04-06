//using System;
//using Microsoft.VisualStudio.Patterning.Authoring.Authoring.Automation.Commands;
//using Microsoft.VisualStudio.Patterning.Runtime;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests.Toolkit.Automation.Commands
//{
//    [TestClass]
//    public class CreateAutomationLibraryExtensionCommandSpec
//    {
//        internal static readonly IAssertion Assert = new Assertion();

//        [TestClass]
//        public class GivenACommand
//        {
//            private CreateDescendantElementsCommand command;

//            [TestInitialize]
//            public void InitializeContext()
//            {
//                this.command = new CreateDescendantElementsCommand();
//            }

//            [TestMethod]
//            public void WhenDescendantElementPathIsEmpty_ThenThrows()
//            {
//                this.command.DescendantElementPath = string.Empty;

//                Assert.Throws<NullReferenceException>(() =>
//                    this.command.Execute());
//            }

//            //[TestMethod]
//            //public void WhenDescendantElementPathHasInvalidChars_ThenThrows()
//            //{
//            //    this.command.DescendantElementPath = "Foo[";

//            //    Assert.Throws<InvalidOperationException>(() =>
//            //        this.command.Execute());
//            //}
//        }

//        [TestClass]
//        public class GivenACommandWithAnElementContainer
//        {
//            private CreateDescendantElementsCommand command;
//            private Mock<IElementContainer> currentElement;

//            [TestInitialize]
//            public void InitializeContext()
//            {
//                this.currentElement = new Mock<IElementContainer>();
//                this.command = new CreateDescendantElementsCommand();
//                this.command.CurrentElement = this.currentElement.Object;
//            }

//            [TestMethod]
//            public void WhenDescendantElementPathElementNotExist_ThenDoesNothing()
//            {
//                this.command.DescendantElementPath = "Foo";

//                this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Never());
//            }

//            //[TestMethod]
//            //public void WhenDescendantElementPathContainsSinglePath_ThenElementCreated()
//            //{
//            //    this.command.DescendantElementPath = "Bar";

//            //    this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Once());
//            //}

//            //[TestMethod]
//            //public void WhenDescendantElementPathContainsMultiplePaths_ThenEachMultipleElementsCreated()
//            //{
//            //    this.command.DescendantElementPath = "Bar1;Bar2";

//            //    this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Exactly(2));
//            //}
//        }
//    }
//}
