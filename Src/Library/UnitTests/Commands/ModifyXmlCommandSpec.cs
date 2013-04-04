using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Commands;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Shell;
using NuPattern.Xml;

namespace NuPattern.Library.UnitTests.Commands
{
    [TestClass]
    public class ModifyXmlCommandSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private ModifyXmlCommand command;
            private Mock<IErrorList> errorList = new Mock<IErrorList>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.command = new ModifyXmlCommand
                    {
                        Solution = Mock.Of<ISolution>(),
                        UriReferenceService = Mock.Of<IFxrUriReferenceService>(),
                        XmlProcessor = Mock.Of<IXmlProcessor>(),
                        ErrorList = this.errorList.Object,
                        CurrentElement = Mock.Of<IProductElement>(),
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenActionIsDefault()
            {
                this.command.Action = ModifyAction.Update;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlPathIsNull_ThenThrows()
            {
                this.command.SourcePath = "foo";

                Assert.Throws<ValidationException>(
                    () => this.command.Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathInvalid_ThenReturns()
            {
                this.command.SourcePath = "foo";
                this.command.XmlPath = "bar";

                this.command.Execute();

                this.errorList.Verify(el => el.Clear(It.IsAny<string>()), Times.Never());
            }
        }

        [TestClass]
        public class GivenAnXmlFile
        {
            private ModifyXmlCommand command;
            private Mock<IErrorList> errorList = new Mock<IErrorList>();
            private Mock<IFxrUriReferenceService> uriService = new Mock<IFxrUriReferenceService>();
            private Mock<IXmlProcessor> xmlProcessor = new Mock<IXmlProcessor>();
            private Mock<IProductElement> currentElement = new Mock<IProductElement>();

            private ISolution solution = new Solution
                {
                    Name = "Solution",
                    Items = 
                    {
                        new Item
                        {
                            Name = "foo.xml",
                            PhysicalPath = "C:\temp\foo.xml"
                        },
                        new Item
                        {
                            Name = "bar.xml",
                            PhysicalPath = "C:\temp\bar.xml"
                        },
                    }
                };

            [TestInitialize]
            public void InitializeContext()
            {
                this.currentElement.Setup(ce => ce.InstanceName).Returns("foo");

                this.command = new ModifyXmlCommand
                    {
                        Solution = this.solution,
                        UriReferenceService = this.uriService.Object,
                        XmlProcessor = this.xmlProcessor.Object,
                        ErrorList = this.errorList.Object,
                        CurrentElement = this.currentElement.Object,
                        SourcePath = "foo.xml",
                        XmlPath = "bar",
                        NewValue = "foo"
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDocumentLoadFails_ThenThrows()
            {
                this.xmlProcessor.Setup(xp => xp.LoadDocument(It.IsAny<string>())).Throws<XmlException>();

                Assert.Throws<XmlException>(
                    () => this.command.Execute());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathContainsExpressionButDocumentLoadFails_ThenThrows()
            {
                this.command.SourcePath = "{InstanceName}.xml";
                this.currentElement.Setup(ce => ce.InstanceName).Returns("bar");
                this.xmlProcessor.Setup(xp => xp.LoadDocument(It.IsAny<string>())).Throws<XmlException>();

                Assert.Throws<XmlException>(
                    () => this.command.Execute());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\bar.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ErrorCategory>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXPathInvalid_ThenThrows()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Throws<XPathException>();

                Assert.Throws<XPathException>(
                    () => this.command.Execute());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ErrorCategory>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementNotFound_ThenErrorReported()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns((IXmlProcessorNode)null);

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), "C:\temp\foo.xml", ErrorCategory.Error), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundAndDelete_TheNodeDeletedAndDocumentSaved()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.command.Action = ModifyAction.Delete;

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                node.Verify(n => n.Remove(), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Once());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), ErrorCategory.Error), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundAndUpdate_TheNodeUpdatedAndDocumentSaved()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.command.Action = ModifyAction.Update;

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                node.Verify(n => n.Remove(), Times.Never());
                node.VerifySet(n => n.Value = "foo", Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Once());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), ErrorCategory.Error), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundAndUpdateAndNewValueExpression_TheNodeUpdatedAndDocumentSaved()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.command.NewValue = "{InstanceName}{InstanceOrder}";
                this.currentElement.Setup(ce => ce.InstanceName).Returns("bar");
                this.currentElement.Setup(ce => ce.InstanceOrder).Returns(5.0);

                this.command.Action = ModifyAction.Update;

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                node.Verify(n => n.Remove(), Times.Never());
                node.VerifySet(n => n.Value = "bar5", Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Once());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), ErrorCategory.Error), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundAndUpdateAndNewValueSame_TheNodeNotUpdatedAndDocumentNotSaved()
            {
                var node = new Mock<IXmlProcessorNode>();
                node.Setup(n => n.Value).Returns("bar");
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.command.NewValue = "bar";

                this.command.Action = ModifyAction.Update;

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                node.Verify(n => n.Remove(), Times.Never());
                node.VerifySet(n => n.Value = It.IsAny<string>(), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), ErrorCategory.Error), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundAndAnyActionButSaveDocumentThrows_ThenNotThrow()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.xmlProcessor.Setup(xp => xp.SaveDocument(It.IsAny<bool>())).Throws<Exception>();
                this.command.Action = ModifyAction.Delete;

                this.command.Execute();

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                node.Verify(n => n.Remove(), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Once());
                this.errorList.Verify(el => el.AddMessage(It.IsAny<string>(), It.IsAny<string>(), ErrorCategory.Error), Times.Never());
            }
        }
    }
}
