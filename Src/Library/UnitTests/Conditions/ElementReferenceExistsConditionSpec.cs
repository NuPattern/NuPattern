using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Conditions
{
    [TestClass]
    public class ElementReferenceExistsConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private ElementReferenceExistsCondition condition;

            [TestInitialize]
            public void InitializeContext()
            {
                this.condition = new ElementReferenceExistsCondition();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenKindIsEmpty_ThenEvaluateThrows()
            {
                Assert.Throws<ValidationException>(() => this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenKindIsNullOrEmpty_ThenEvaluateThrows()
            {
                Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
                this.condition.CurrentElement = mockCurrentElement.Object;

                Assert.Throws<ValidationException>(() => this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceNotExist_ThenEvaluateReturnsFalse()
            {
                Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
                this.condition.CurrentElement = mockCurrentElement.Object;
                this.condition.Kind = "Bar";

                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenReferenceExists_ThenEvaluateReturnsTrue()
            {
                Mock<IProductElement> mockCurrentElement = new Mock<IProductElement>();
                Mock<IReference> reference = new Mock<IReference>();
                reference.Setup(r => r.Kind).Returns("Bar");
                mockCurrentElement.Setup(CurrentElement => CurrentElement.References).Returns(new[] { reference.Object });
                this.condition.CurrentElement = mockCurrentElement.Object;
                this.condition.Kind = "Bar";

                Assert.True(this.condition.Evaluate());
            }
        }
    }
}
