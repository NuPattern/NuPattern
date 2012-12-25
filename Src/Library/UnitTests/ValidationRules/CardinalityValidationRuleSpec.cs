using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.ValidationRules;
using NuPattern.Runtime;

namespace NuPattern.Library.UnitTests
{
    public class CardinalityValidationRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenNoChildElementName_ThenThrows()
            {
                var rule = new CardinalityValidationRule();

                Assert.Throws<ValidationException>(
                    () => rule.Validate().ToList());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMinimunIsGreaterThanMaximun_ThenThrows()
            {
                var rule = new CardinalityValidationRule();

                rule.ChildElementName = "Foo";
                rule.Maximum = 1;
                rule.Minimum = 3;
                rule.CurrentElement = new Mock<IProduct>().Object;

                Assert.Throws<InvalidOperationException>(() =>
                    rule.Validate().ToList());
            }
        }

        [TestClass]
        public class GivenAProductWithOneViewAndTwoElements
        {
            private CardinalityValidationRule rule;

            [TestInitialize]
            public void InitializeContext()
            {
                var elementInfo1 = Mocks.Of<IElementInfo>().First(i => i.Name == "FooElementName" && i.DisplayName == "FooElementNameDisplayName");
                var element1 = new Mock<IElement>();
                element1.SetupGet(e => e.Info).Returns(elementInfo1);
                element1.As<IAbstractElement>().SetupGet(e => e.Info).Returns(elementInfo1);
                element1.As<IProductElement>().SetupGet(e => e.Info).Returns(elementInfo1);

                var elementInfo2 = Mocks.Of<IElementInfo>().First(i => i.Name == "BarElementName" && i.DisplayName == "BarElementNameDisplayName");
                var element2 = new Mock<IElement>();
                element2.Setup(e => e.InstanceName).Returns("FooElement2NameDisplayName");
                element2.SetupGet(e => e.Info).Returns(elementInfo2);
                element2.As<IAbstractElement>().SetupGet(e => e.Info).Returns(elementInfo2);
                element2.As<IProductElement>().SetupGet(e => e.Info).Returns(elementInfo2);

                var viewInfo = new Mock<IViewInfo>();
                viewInfo.SetupGet(v => v.Elements).Returns(new[] { elementInfo1, elementInfo2 });
                var view = new Mock<IView>();
                view.SetupGet(v => v.Info).Returns(viewInfo.Object);
                view.SetupGet(v => v.Elements).Returns(new[] { element1.Object, element2.Object });

                var productInfo = new Mock<IPatternInfo>();
                productInfo.SetupGet(pi => pi.Name).Returns("FooProductName");
                productInfo.SetupGet(pi => pi.Views).Returns(new[] { viewInfo.Object });

                var product = new Mock<IProduct>();
                product.Setup(p => p.InstanceName).Returns("TestProduct");
                product.As<IProduct>().SetupGet(p => p.Info).Returns(productInfo.Object);
                product.As<IProductElement>().SetupGet(p => p.Info).Returns(productInfo.Object);
                product.SetupGet(p => p.Views).Returns(new[] { view.Object });

                this.rule = new CardinalityValidationRule();
                this.rule.CurrentElement = product.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoChildInfo_ThenThrows()
            {
                this.rule.Maximum = 1;
                this.rule.Minimum = 1;
                this.rule.ChildElementName = "Bar";

                Assert.Throws<InvalidOperationException>(() =>
                    rule.Validate().ToList());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCardinalityIsWrong_ThenErrorsAreReturned()
            {
                this.rule.Maximum = 2;
                this.rule.Minimum = 2;
                this.rule.ChildElementName = "FooElementName";

                var results = rule.Validate();

                Assert.Equal(1, results.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCardinalityIsCorrect_ThenNoErrorsAreReturned()
            {
                rule.Maximum = 3;
                rule.Minimum = 1;
                rule.ChildElementName = "FooElementName";

                var results = rule.Validate();

                Assert.Equal(0, results.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenErrorsAreReturned_UseChildDisplayNameForError()
            {
                rule.Maximum = 2;
                rule.Minimum = 2;
                rule.ChildElementName = "FooElementName";

                var results = rule.Validate();

                Assert.Equal(1, results.Count());
                Assert.True(results.FirstOrDefault().ErrorMessage.EndsWith("of the 'FooElementNameDisplayName' element."));
            }
        }
    }
}