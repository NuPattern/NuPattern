//using System;
//using NuPattern.Authoring.PatternToolkit.Automation.Commands;
//using NuPattern.Runtime;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace NuPattern.Authoring.UnitTests.Toolkit.Automation.Commands
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

//            [TestMethod, TestCategory("Unit")]
//            public void WhenDescendantElementPathIsEmpty_ThenThrows()
//            {
//                this.command.DescendantElementPath = string.Empty;

//                Assert.Throws<NullReferenceException>(() =>
//                    this.command.Execute());
//            }

//            //[TestMethod, TestCategory("Unit")]
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

//            [TestMethod, TestCategory("Unit")]
//            public void WhenDescendantElementPathElementNotExist_ThenDoesNothing()
//            {
//                this.command.DescendantElementPath = "Foo";

//                this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Never());
//            }

//            //[TestMethod, TestCategory("Unit")]
//            //public void WhenDescendantElementPathContainsSinglePath_ThenElementCreated()
//            //{
//            //    this.command.DescendantElementPath = "Bar";

//            //    this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Once());
//            //}

//            //[TestMethod, TestCategory("Unit")]
//            //public void WhenDescendantElementPathContainsMultiplePaths_ThenEachMultipleElementsCreated()
//            //{
//            //    this.command.DescendantElementPath = "Bar1;Bar2";

//            //    this.currentElement.Verify(e => e.CreateElement(It.IsAny<Action<IElement>>()), Times.Exactly(2));
//            //}
//        }
//    }
//}
