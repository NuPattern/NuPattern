using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Conditions
{
    [TestClass]
    public class PropertyChangedEventArgsMatchesPropertyNameConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private PropertyChangedEventArgsMatchesPropertyNameCondition condition;

        [TestInitialize]
        public void InitializeContext()
        {
            this.condition = new PropertyChangedEventArgsMatchesPropertyNameCondition();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenChangedEventIsNull_ThenEvaluateFalse()
        {
            this.condition.PropertyName = "Foo";
            Assert.False(this.condition.Evaluate());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyNameIsNull_ThenEvaluateFalse()
        {
            var mockEvent = new Mock<IEvent<PropertyChangedEventArgs>>();
            this.condition.Event = mockEvent.Object;

            Assert.Throws<ValidationException>(() => this.condition.Evaluate());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyNameIsEmpty_ThenEvaluateThrows()
        {
            var mockEvent = new Mock<IEvent<PropertyChangedEventArgs>>();
            this.condition.Event = mockEvent.Object;
            this.condition.PropertyName = string.Empty;

            Assert.Throws<ValidationException>(() => this.condition.Evaluate());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenEventArgsMatchesPropertyName_ThenEvaluateReturnsTrue()
        {
            var mockEvent = new Mock<IEvent<PropertyChangedEventArgs>>();
            mockEvent.Setup(x => x.EventArgs).Returns(new PropertyChangedEventArgs("Foo"));
            this.condition.Event = mockEvent.Object;
            this.condition.PropertyName = "Foo";

            var result = this.condition.Evaluate();

            Assert.True(result);
        }
    }
}
