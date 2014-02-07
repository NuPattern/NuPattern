using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Conditions;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Library.UnitTests.Conditions
{
    [TestClass]
    public class ArtifactReferenceExistsConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private ArtifactReferenceExistsCondition condition;

            [TestInitialize]
            public void InitializeContext()
            {
                this.condition = new ArtifactReferenceExistsCondition();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoReferencesExist_ThenEvaluateReturnsFalse()
            {
                var mockCurrentElement = new Mock<IProductElement>();
                this.condition.CurrentElement = mockCurrentElement.Object;

                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoArtifactReferencesExist_ThenEvaluateReturnsFalse()
            {
                var fooReference = new Mock<IReference>();
                fooReference.SetupGet(r => r.Kind).Returns("foo");
                fooReference.SetupGet(r => r.Value).Returns("unfolded://");

                var mockCurrentElement = new Mock<IProductElement>();
                mockCurrentElement.Setup(owner => owner.References).Returns(new[] { fooReference.Object });
                this.condition.CurrentElement = mockCurrentElement.Object;

                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferencesExist_ThenEvaluateReturnsTrue()
            {
                var generatedReference = new Mock<IReference>();
                generatedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                generatedReference.SetupGet(r => r.Value).Returns("generated://");

                var mockCurrentElement = new Mock<IProductElement>();
                mockCurrentElement.Setup(owner => owner.References).Returns(new[] { generatedReference.Object });
                this.condition.CurrentElement = mockCurrentElement.Object;

                Assert.True(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferencesExistWithNotMatchingTag_ThenEvaluateReturnsFalse()
            {
                var generatedReference = new Mock<IReference>();
                generatedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                generatedReference.SetupGet(r => r.Value).Returns("generated://");

                var mockCurrentElement = new Mock<IProductElement>();
                mockCurrentElement.Setup(owner => owner.References).Returns(new[] { generatedReference.Object });
                this.condition.CurrentElement = mockCurrentElement.Object;
                this.condition.Tag = "tag1";

                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenArtifactReferencesExistWithTMatchingTag_ThenEvaluateReturnsFalse()
            {
                var generatedReference = new Mock<IReference>();
                generatedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.SolutionItem);
                generatedReference.SetupGet(r => r.Value).Returns("generated://");
                generatedReference.SetupGet(r => r.Tag).Returns("tag1");

                var mockCurrentElement = new Mock<IProductElement>();
                mockCurrentElement.Setup(owner => owner.References).Returns(new[] { generatedReference.Object });
                this.condition.CurrentElement = mockCurrentElement.Object;
                this.condition.Tag = "tag1";

                Assert.True(this.condition.Evaluate());
            }
        }
    }
}
