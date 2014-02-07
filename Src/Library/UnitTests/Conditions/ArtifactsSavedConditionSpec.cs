using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Conditions;
using NuPattern.Runtime;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests.Conditions
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
                var mockUriReferenceService = new Mock<IUriReferenceService>();
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
            private Mock<EnvDTE.ProjectItem> projectItem;
            private Mock<IUriReferenceService> referenceService;
            private Mock<IReference> reference;

            [TestInitialize]
            public void InitializeContext()
            {
                this.projectItem = new Mock<EnvDTE.ProjectItem>();
                this.projectItem.Setup(pi => pi.Saved).Returns(true);
                var item = new Mock<IItem>();
                item.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(this.projectItem.Object);
                this.referenceService = new Mock<IUriReferenceService>();
                this.referenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>()))
                    .Returns(item.Object);

                var element = new Mock<IProductElement>();
                this.reference = new Mock<IReference>();
                this.reference.SetupGet(r => r.Kind).Returns(typeof(SolutionArtifactLinkReference).FullName);
                this.reference.SetupGet(r => r.Value).Returns("solution://");
                element.SetupGet(e => e.References).Returns(new[] { reference.Object });

                this.condition = new ArtifactsSavedCondition();
                this.condition.UriReferenceService = this.referenceService.Object;
                this.condition.CurrentElement = element.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsNotDirty_ThenEvaluateReturnsTrue()
            {
                var result = this.condition.Evaluate();

                Assert.True(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsDirty_ThenEvaluateReturnsTrue()
            {
                this.projectItem.SetupGet(pi => pi.Saved).Returns(false);
                var result = this.condition.Evaluate();

                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactIsNotAnItem_ThenEvaluateReturnsTrue()
            {
                this.referenceService.Setup(s => s.ResolveUri<IItemContainer>(It.IsAny<Uri>())).Returns(new Mock<ISolution>().Object);

                var result = this.condition.Evaluate();

                Assert.True(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactDirtyWithNotMatchingTag_ThenEvaluateReturnsTrue()
            {
                this.projectItem.SetupGet(pi => pi.Saved).Returns(false);
                this.reference.Setup(r => r.Tag).Returns("tag1");
                this.condition.Tag = "tag2";

                var result = this.condition.Evaluate();

                Assert.True(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactDirtyWithMatchingTag_ThenEvaluateReturnsFalse()
            {
                this.projectItem.SetupGet(pi => pi.Saved).Returns(false);
                this.reference.Setup(r => r.Tag).Returns("tag1");
                this.condition.Tag = "tag1";
                
                var result = this.condition.Evaluate();

                Assert.False(result);
            }
        }
    }
}