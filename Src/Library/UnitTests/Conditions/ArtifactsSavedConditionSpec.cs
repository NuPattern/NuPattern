using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Conditions;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Library.UnitTests
{
    [TestClass]
    public class ArtifactsSavedConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoArtifacts
        {
            private ArtifactsSavedCondition condition;

            [TestInitialize]
            public void InitializeContext()
            {
                Mock<IFxrUriReferenceService> mockUriReferenceService = new Mock<IFxrUriReferenceService>();
                mockUriReferenceService.Setup(uriReferenceService => uriReferenceService.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns((IItemContainer)null);

                this.condition = new ArtifactsSavedCondition();
                this.condition.UriReferenceService = mockUriReferenceService.Object;
                this.condition.CurrentElement = new Mock<IProductElement>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenEvaluateReturnsTrue()
            {
                var result = this.condition.Evaluate();

                Assert.True(result);
            }
        }

        [TestClass]
        public class GivenSingleArtifact
        {
            private ArtifactsSavedCondition condition;
            private Mock<EnvDTE.ProjectItem> mockProjectItem;
            private Mock<IFxrUriReferenceService> mockUriReferenceService;

            [TestInitialize]
            public void InitializeContext()
            {
                this.mockProjectItem = new Mock<EnvDTE.ProjectItem>();
                this.mockProjectItem.Setup(pi => pi.Saved).Returns(true);
                var mockItem = new Mock<IItem>();
                mockItem.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(this.mockProjectItem.Object);
                this.mockUriReferenceService = new Mock<IFxrUriReferenceService>();
                this.mockUriReferenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(mockItem.Object);

                var mockElement = new Mock<IProductElement>();
                var mockReference = new Mock<IReference>();
                mockReference.SetupGet(r => r.Kind).Returns(typeof(SolutionArtifactLinkReference).FullName);
                mockReference.SetupGet(r => r.Value).Returns("solution://");
                mockElement.SetupGet(e => e.References).Returns(new[] { mockReference.Object });

                this.condition = new ArtifactsSavedCondition();
                this.condition.UriReferenceService = this.mockUriReferenceService.Object;
                this.condition.CurrentElement = mockElement.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsNotDirty_ThenEvaluateReturnsTrue()
            {
                var result = this.condition.Evaluate();

                Assert.True(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsDirty_ThenEvaluateReturnsFalse()
            {
                this.mockProjectItem.SetupGet(pi => pi.Saved).Returns(false);
                var result = this.condition.Evaluate();

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsNotAnItem_ThenEvaluateReturnsTrue()
            {
                this.mockUriReferenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(new Mock<ISolution>().Object);
                var result = this.condition.Evaluate();

                Assert.True(result);
            }
        }
    }
}