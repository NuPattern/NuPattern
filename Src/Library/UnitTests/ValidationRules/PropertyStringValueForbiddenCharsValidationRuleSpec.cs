using System.Linq;
using Microsoft.VisualStudio.Patterning.Library.ValidationRules;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests
{
    public class PropertyStringValueForbiddenCharsValidationRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAStringProperty
        {
            private PropertyStringValueForbiddenCharsValidationRule rule;

            [TestInitialize]
            public void Initialize()
            {
                this.rule = new PropertyStringValueForbiddenCharsValidationRule();

                var property = new Mock<IProperty>();
                property.Setup(p => p.Info.Type).Returns(typeof(string).FullName);
                property.Setup(p => p.Owner.InstanceName).Returns("OwningElementName1");
                property.Setup(p => p.DefinitionName).Returns("OwningElementType");
                this.rule.CurrentProperty = property.Object;
                this.rule.ForbiddenChars = " ";
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoValue_ThenSucceeds()
            {
                Mock.Get(this.rule.CurrentProperty)
                    .Setup(p => p.Value).Returns(string.Empty);

                var result = this.rule.Validate();

                Assert.Equal(result.Count(), 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueContainsNoForbidden_ThenSucceeds()
            {
                Mock.Get(this.rule.CurrentProperty)
                    .Setup(p => p.Value).Returns("foo");

                var result = this.rule.Validate();

                Assert.Equal(result.Count(), 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueContainsForbidden_ThenFails()
            {
                Mock.Get(this.rule.CurrentProperty)
                    .Setup(p => p.Value).Returns("foo bar sum");

                var result = this.rule.Validate();

                Assert.Equal(result.Count(), 1);
            }
        }

        [TestClass]
        public class GivenAIntProperty
        {
            private PropertyStringValueForbiddenCharsValidationRule rule;

            [TestInitialize]
            public void Initialize()
            {
                this.rule = new PropertyStringValueForbiddenCharsValidationRule();

                var property = new Mock<IProperty>();
                property.Setup(p => p.Info.Type).Returns(typeof(int).FullName);
                property.Setup(p => p.Owner.InstanceName).Returns("OwningElementName1");
                property.Setup(p => p.DefinitionName).Returns("OwningElementType");
                this.rule.CurrentProperty = property.Object;
                this.rule.ForbiddenChars = " ";
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenForbiddenValue_ThenSucceeds()
            {
                Mock.Get(this.rule.CurrentProperty)
                    .Setup(p => p.Value).Returns("foo bar");

                var result = this.rule.Validate();

                Assert.Equal(result.Count(), 0);
            }
        }
    }
}