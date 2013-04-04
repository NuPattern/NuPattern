using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.ValueProviders;
using NuPattern.Runtime;
using NuPattern.Xml;

namespace NuPattern.Library.UnitTests.ValueProviders
{
    [TestClass]
    public class XmlNodeValueProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private XmlNodeValueProvider provider;
            private Mock<IXmlProcessor> xmlProcessor = new Mock<IXmlProcessor>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.provider = new XmlNodeValueProvider
                    {
                        Solution = Mock.Of<ISolution>(),
                        UriReferenceService = Mock.Of<IFxrUriReferenceService>(),
                        XmlProcessor = this.xmlProcessor.Object,
                        CurrentElement = Mock.Of<IProductElement>(),
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlPathIsNull_ThenThrows()
            {
                this.provider.SourcePath = "foo";

                Assert.Throws<ValidationException>(
                    () => this.provider.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathInvalid_ThenReturns()
            {
                this.provider.SourcePath = "foo";
                this.provider.XmlPath = "bar";

                var result = this.provider.Evaluate();

                Assert.Null(result);
                this.xmlProcessor.Verify(el => el.LoadDocument(It.IsAny<string>()), Times.Never());
            }
        }

        [TestClass]
        public class GivenAnXmlFile
        {
            private XmlNodeValueProvider provider;
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

                this.provider = new XmlNodeValueProvider
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
                    () => this.provider.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSourcePathContainsExpressionButDocumentLoadFails_ThenThrows()
            {
                this.provider.SourcePath = "{InstanceName}.xml";
                this.currentElement.Setup(ce => ce.InstanceName).Returns("bar");
                this.xmlProcessor.Setup(xp => xp.LoadDocument(It.IsAny<string>())).Throws<XmlException>();

                Assert.Throws<XmlException>(
                    () => this.provider.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\bar.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Never());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXPathInvalid_ThenThrows()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Throws<XPathException>();

                Assert.Throws<XPathException>(
                    () => this.provider.Evaluate());

                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.SaveDocument(It.IsAny<bool>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementNotFound_ThenReturnsNull()
            {
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns((IXmlProcessorNode)null);

                var result = this.provider.Evaluate();

                Assert.Null(result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFound_TheReturnsValue()
            {
                var node = new Mock<IXmlProcessorNode>();
                node.Setup(n => n.Value).Returns("foo");
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);

                var result = this.provider.Evaluate();

                Assert.Equal("foo", result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenXmlElementFoundButCloseDocumentThrows_ThenNotThrow()
            {
                var node = new Mock<IXmlProcessorNode>();
                node.Setup(n => n.Value).Returns("foo");
                this.xmlProcessor.Setup(xp => xp.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>())).Returns(node.Object);
                this.xmlProcessor.Setup(xp => xp.CloseDocument()).Throws<Exception>();

                var result = this.provider.Evaluate();

                Assert.Equal("foo", result);
                this.xmlProcessor.Verify(el => el.LoadDocument("C:\temp\foo.xml"), Times.Once());
                this.xmlProcessor.Verify(el => el.FindFirst(It.IsAny<string>(), It.IsAny<IDictionary<string, string>>()), Times.Once());
                this.xmlProcessor.Verify(el => el.CloseDocument(), Times.Once());
            }
        }
    }
}
