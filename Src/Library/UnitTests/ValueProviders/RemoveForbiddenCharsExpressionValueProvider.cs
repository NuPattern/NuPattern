using Microsoft.VisualStudio.Patterning.Library.ValueProviders;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.ValueProviders
{
    [TestClass]
    public class RemoveForbiddenCharsExpressionValueProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAProvider
        {
            private RemoveForbiddenCharsExpressionValueProvider provider;
            private Mock<IProductElement> currentElement;

            [TestInitialize]
            public void InitializeContext()
            {
                this.currentElement = new Mock<IProductElement>();
                currentElement.Setup(e => e.InstanceName).Returns("An Element");

                this.provider = new RemoveForbiddenCharsExpressionValueProvider();
                this.provider.CurrentElement = currentElement.Object;
                this.provider.Expression = "{InstanceName} Foo Bar";
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExpressionEvaluatesToEmpty_ThenReturnsEmpty()
            {
                this.currentElement.Setup(e => e.InstanceName).Returns("");

                this.provider.Expression = "{InstanceName}";
                this.provider.ForbiddenChars = string.Empty;
                var result = this.provider.Evaluate();

                Assert.Equal(result, string.Empty);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoForbiddenChars_ThenReturnsExpressionResult()
            {
                this.provider.ForbiddenChars = string.Empty;
                var result = this.provider.Evaluate();

                Assert.Equal(result, "An Element Foo Bar");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExpressionContainsNoForbiddenChars_ThenReturnsExpressionResult()
            {
                this.provider.ForbiddenChars = "%$#@";
                var result = this.provider.Evaluate();

                Assert.Equal(result, "An Element Foo Bar");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExpressionContainsForbiddenChars_ThenReturnsReplacedResult()
            {
                this.provider.ForbiddenChars = " EAFB";
                var result = this.provider.Evaluate();

                Assert.Equal(result, "nlementooar");
            }
        }
    }
}
