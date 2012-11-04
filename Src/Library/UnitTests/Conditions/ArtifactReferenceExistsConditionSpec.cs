using Microsoft.VisualStudio.Patterning.Extensibility.References;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Conditions
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

			[TestMethod]
			public void WhenNoReferencesExist_ThenEvaluateReturnsFalse()
			{
				Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
				this.condition.CurrentElement = mockCurrentElement.Object;

				Assert.False(this.condition.Evaluate());
			}

			[TestMethod]
			public void WhenNoArtifactReferencesExist_ThenEvaluateReturnsFalse()
			{
				Mock<IReference> fooReference = new Mock<IReference>();
				fooReference.SetupGet(r => r.Kind).Returns("foo");
				fooReference.SetupGet(r => r.Value).Returns("unfolded://");

				Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
				mockCurrentElement.Setup(owner => owner.References).Returns(new[] { fooReference.Object });
				this.condition.CurrentElement = mockCurrentElement.Object;

				Assert.False(this.condition.Evaluate());
			}

			[TestMethod]
			public void WhenArtifactReferencesExist_ThenEvaluateReturnsTrue()
			{
				Mock<IReference> generatedReference = new Mock<IReference>();
				generatedReference.SetupGet(r => r.Kind).Returns(ReferenceKindConstants.ArtifactLink);
				generatedReference.SetupGet(r => r.Value).Returns("generated://");

				Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
				mockCurrentElement.Setup(owner => owner.References).Returns(new[] { generatedReference.Object });
				this.condition.CurrentElement = mockCurrentElement.Object;

				Assert.True(this.condition.Evaluate());
			}
		}
	}
}
