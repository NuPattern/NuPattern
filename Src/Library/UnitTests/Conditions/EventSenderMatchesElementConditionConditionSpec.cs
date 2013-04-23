using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Conditions;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests.Conditions
{
    [TestClass]
    public class EventSenderMatchesElementConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private EventSenderMatchesElementCondition condition;

            [TestInitialize]
            public void InitializeContext()
            {
                this.condition = new EventSenderMatchesElementCondition();
                this.condition.CurrentElement = new Mock<IInstanceBase>().Object;
                this.condition.Event = new Mock<IEvent<EventArgs>>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventIsNull_ThenEvaluateReturnsFalse()
            {
                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCurrentElementIsNull_ThenEvaluateReturnsFalse()
            {
                this.condition.Event = new Mock<IEvent<EventArgs>>().Object;

                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventSenderIsNull_ThenEvaluateReturnsFalse()
            {
                var mockEvent = new Mock<IEvent<EventArgs>>();
                mockEvent.Setup(e => e.Sender).Returns(null);
                this.condition.Event = mockEvent.Object;

                Assert.False(this.condition.Evaluate());
            }
        }

        [TestClass]
        public class GivenAnElement
        {
            private EventSenderMatchesElementCondition condition;
            private Mock<IProductElement> currentElement;
            private Mock<IEvent<EventArgs>> mockEvent;

            [TestInitialize]
            public void InitializeContext()
            {
                this.currentElement = new Mock<IProductElement>();
                this.mockEvent = new Mock<IEvent<EventArgs>>();

                this.condition = new EventSenderMatchesElementCondition();
                this.condition.Event = this.mockEvent.Object;
                this.condition.CurrentElement = this.currentElement.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventSenderIsCurrentElement_ThenEvaluateReturnsTrue()
            {
                this.mockEvent.Setup(e => e.Sender).Returns(this.currentElement.Object);
                Assert.True(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventSenderIsAnotherElement_ThenEvaluateReturnsFalse()
            {
                var anotherElement = new Mock<IProductElement>();
                this.mockEvent.Setup(e => e.Sender).Returns(anotherElement.Object);
                Assert.False(this.condition.Evaluate());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEventSenderIsNotAElement_ThenEvaluateReturnsTrue()
            {
                var anObject = new object();
                this.mockEvent.Setup(e => e.Sender).Returns(anObject);
                Assert.True(this.condition.Evaluate());
            }
        }
    }
}
