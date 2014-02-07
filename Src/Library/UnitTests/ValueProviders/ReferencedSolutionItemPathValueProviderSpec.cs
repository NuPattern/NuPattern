using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.ValueProviders;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.ValueProviders
{
    public class ReferencedSolutionItemPathValueProviderSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContext
        {
            private ReferencedSolutionItemPathValueProvider provider;
            private Mock<IProductElement> currentElement;
            private Mock<IUriReferenceService> uriService;

            [TestInitialize]
            public void Initialize()
            {
                this.currentElement = new Mock<IProductElement>();
                this.uriService = new Mock<IUriReferenceService>();

                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(new Uri("solution://foo")))
                    .Returns(Mock.Of<IItem>(si => si.PhysicalPath == @"C:\foo.cs"));
                this.uriService.Setup(us => us.ResolveUri<IItemContainer>(new Uri("solution://bar")))
                    .Returns(Mock.Of<IItem>(si => si.PhysicalPath == @"C:\bar.cs"));

                this.provider = new ReferencedSolutionItemPathValueProvider();
                this.provider.CurrentElement = this.currentElement.Object;
                this.provider.UriReferenceService = this.uriService.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEvaluateAndExtensionIsNull_ThenThrows()
            {
                Assert.Throws<ValidationException>(() => this.provider.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEvaluateAndNoReferences_ThenReturnsNull()
            {
                this.provider.Extension = ".cs";
                this.currentElement.Setup(ce => ce.References).Returns(Enumerable.Empty<IReference>());

                Assert.Null(this.provider.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEvaluateAndReferencesWithExtension_ThenReturnsFirstItemPath()
            {
                this.provider.Extension = ".cs";
                this.currentElement.Setup(ce => ce.References)
                    .Returns(new []
                    {
                        Mock.Of<IReference>(r => r.Kind == typeof(SolutionArtifactLinkReference).FullName
                        && r.Value == "solution://foo"),
                        Mock.Of<IReference>(r => r.Kind == typeof(SolutionArtifactLinkReference).FullName
                        && r.Value == "solution://bar"),
                    });

                var result = this.provider.Evaluate();

                this.uriService.Verify(us => us.ResolveUri<IItemContainer>(It.Is<Uri>(x => x.OriginalString == "solution://foo")), Times.AtLeastOnce());

                Assert.Equal(@"C:\foo.cs", result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEvaluateAndReferencesWithExtensionAndTag_ThenReturnsFirstItemWithTagPath()
            {
                this.provider.Tag = "tag1";
                this.provider.Extension = ".cs";
                this.currentElement.Setup(ce => ce.References)
                    .Returns(new[]
                    {
                        Mock.Of<IReference>(r => r.Kind == typeof(SolutionArtifactLinkReference).FullName
                        && r.Value == "solution://foo"),
                        Mock.Of<IReference>(r => r.Kind == typeof(SolutionArtifactLinkReference).FullName
                        && r.Value == "solution://bar"
                        && r.Tag=="tag1"),
                    });

                var result = this.provider.Evaluate();

                this.uriService.Verify(us => us.ResolveUri<IItemContainer>(It.Is<Uri>(x => x.OriginalString == "solution://bar")), Times.AtLeastOnce());

                Assert.Equal(@"C:\bar.cs", result);
            }
        }
    }
}