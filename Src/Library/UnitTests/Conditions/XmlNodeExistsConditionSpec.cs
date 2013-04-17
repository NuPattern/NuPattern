using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Conditions;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;
using NuPattern.Xml;

namespace NuPattern.Library.UnitTests.Conditions
{
    [TestClass]
    public class XmlNodeExistsConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private XmlNodeExistsCondition command;
            private Mock<IXmlProcessor> xmlProcessor = new Mock<IXmlProcessor>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.command = new XmlNodeExistsCondition
                    {
                        Solution = Mock.Of<ISolution>(),
                        UriReferenceService = Mock.Of<IUriReferenceService>(),
                        XmlProcessor = this.xmlProcessor.Object,
                        CurrentElement = Mock.Of<IProductElement>(),
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlPathIsNull_ThenThrows()
            {
                this.command.SourcePath = "foo";

                Assert.Throws<ValidationException>(
                    () => this.command.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathInvalid_ThenReturns()
            {
                this.command.SourcePath = "foo";
                this.command.XmlPath = "bar";

                var result = this.command.Evaluate();

                Assert.False(result);
                this.xmlProcessor.Verify(el => el.LoadDocument(It.IsAny<string>()), Times.Never());
            }
        }

        [TestClass]
        public class GivenAnXmlFile
        {
            private XmlNodeExistsCondition condition;
            private Mock<IUriReferenceService> uriService = new Mock<IUriReferenceService>();
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

                this.condition = new XmlNodeExistsCondition
                    {
                        Solution = this.solution,
                        UriReferenceService = this.uriService.Object,
                        XmlProcessor = this.xmlProcessor.Object,
                        CurrentElement = this.currentElement.Object,
                        SourcePath = "foo.xml",
                        XmlPath = "bar",
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDocumentLoadFails_ThenThrows()
            {
                this.xmlProcessor.Setup(xp => xp.LoadDocument(It.IsAny<string>())).Throws<XmlException>();

                Assert.Throws<XmlException>(
                    () => this.condition.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathContainsExpressionButDocumentLoadFails_ThenThrows()
            {
                this.condition.SourcePath = "{InstanceName}.xml";
                this.currentElement.Setup(ce => ce.InstanceName).Returns("bar");
                this.xmlProcessor.Setup(xp => xp.LoadDocument(It.IsAny<string>())).Throws<XmlException>();

                Assert.Throws<XmlException>(
                    () => this.condition.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\bar.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXPathInvalid_ThenThrows()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Throws<XPathException>();

                Assert.Throws<XPathException>(
                    () => this.condition.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementNotFound_ThenReturnsFalse()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns((IXmlProcessorNode)null);

                var result = this.condition.Evaluate();

                Assert.False(result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFound_TheReturnsTrue()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);

                var result = this.condition.Evaluate();

                Assert.True(result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundButCloseDocumentThrows_ThenNotThrow()
            {
                var node = new Mock<IXmlProcessorNode>();
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.xmlProcessor.Setup(xp => xp.CloseDocument()).Throws<Exception>();

                var result = this.condition.Evaluate();

                Assert.True(result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }
        }
    }
}
