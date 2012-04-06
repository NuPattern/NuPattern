using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI.UnitTests
{
    [TestClass]
    [CLSCompliant(false)]
    public class ProductElementViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod]
        public void WhenContainerIsNotAutomationContainer_ThenAutomationMenusIsEmpty()
        {
            var container = new Mock<IProductElement>();
            var ctx = new SolutionBuilderContext
            {
                ShowProperties = () => { },
            };

            var viewModel = new TestProductElementViewModel<IAbstractElement>(container.Object, ctx);

            Assert.False(viewModel.MenuOptions.OfType<AutomationMenuOptionViewModel>().Any());
        }

        [TestClass]
        [CLSCompliant(false)]
        public class GivenAnAutomationContainerModelWithOneMenuAutomation
        {
            private Mock<IProductElement> element;
            private Mock<IAutomationExtension> automation;
            private TestProductElementViewModel<IAbstractElement> target;
            private SolutionBuilderContext context;

            [TestInitialize]
            public void Initialize()
            {
                this.element = new Mock<IProductElement>();
                this.element.As<IProductElement>().Setup(x => x.Info).Returns(Mocks.Of<IPatternElementInfo>().First());

                this.automation = new Mock<IAutomationExtension>();
                this.automation.As<IAutomationMenuCommand>().Setup(m => m.Text).Returns("Foo");
                this.automation.As<INotifyPropertyChanged>();
                this.automation.As<ICommandStatus>();

                this.element.Setup(x => x.AutomationExtensions).Returns(new[] { this.automation.Object });

                this.context = new SolutionBuilderContext
                {
                    PatternManager = Mock.Of<IPatternManager>(),
                    ShowProperties = () => { },
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                };

                this.target = new TestProductElementViewModel<IAbstractElement>(this.element.Object, this.context);
            }

            [TestMethod]
            public void WhenCreatingNewInstance_ThenContainsOneAutomationMenu()
            {
                Assert.Equal(1, target.MenuOptions.OfType<AutomationMenuOptionViewModel>().Count());
            }

            [TestMethod]
            public void WhenCreatingNewInstance_ThenCreatesDeleteMenuOption()
            {
                var option = this.target.MenuOptions.First(o => o.Caption.Equals("Delete"));

                Assert.NotNull(option);
            }

            [TestMethod]
            public void WhenCreatingNewInstance_ThenCreatesPropertiesMenuOption()
            {
                var options = this.target.MenuOptions.First(o => o.Caption.Equals("Properties"));

                Assert.NotNull(options);
            }

            [TestMethod]
            public void WhenDelete_ThenPromptsConfirmation()
            {
                this.target.DeleteCommand.Execute(null);

                Mock.Get(this.context.UserMessageService)
                    .Verify(x => x.PromptWarning(It.IsAny<string>()));
            }

            [TestMethod]
            public void WhenDeleteThrows_ThenShowsMessageError()
            {
                this.element.Setup(x => x.Delete()).Throws<InvalidOperationException>();

                this.target.DeleteCommand.Execute(null);

                Mock.Get(this.context.UserMessageService)
                    .Verify(x => x.ShowError(It.IsAny<string>()));
            }
        }

        [TestClass]
        [CLSCompliant(false)]
        public class GivenAProductElement
        {
            private Mock<IProductElement> element;
            private TestProductElementViewModel<IAbstractElement> target;
            private SolutionBuilderContext context;

            [TestInitialize]
            public void Initialize()
            {
                this.element = new Mock<IProductElement>();
                this.element.As<IProductElement>().Setup(x => x.Info).Returns(Mocks.Of<IPatternElementInfo>().First());
                this.element.As<IAbstractElement>().Setup(e => e.Elements).Returns(Enumerable.Empty<IAbstractElement>());

                this.context = new SolutionBuilderContext
                {
                    PatternManager = Mock.Of<IPatternManager>(),
                    ShowProperties = () => { },
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                };

                this.target = new TestProductElementViewModel<IAbstractElement>(this.element.Object, this.context);
            }

            [TestMethod]
            public void WhenShowingNonAbstractElement_ThenMenuAllowed()
            {
                var result = this.target.CanCreateAddMenuInternal(Mock.Of<IPatternElementInfo>());
                Assert.True(result);
            }

            [TestMethod]
            public void WhenShowingElementWithNotIsVisible_ThenMenuNotAllowed()
            {
                var result = this.target.CanCreateAddMenuInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == false));
                Assert.False(result);
            }

            [TestMethod]
            public void WhenShowingElementWithVisibleAndNotAllowAddNewElement_ThenMenuNotAllowed()
            {
                var result = this.target.CanCreateAddMenuInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == false));
                Assert.False(result);
            }

            [TestMethod]
            public void WhenShowingElementWithVisibleAndAllowAddNewElement_ThenMenuAllowed()
            {
                var result = this.target.CanCreateAddMenuInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true));
                Assert.True(result);
            }

            [TestMethod]
            public void WhenCreatingNonAbstractElement_ThenAddNotAllowed()
            {
                var result = this.target.CanAddNewInstanceInternal(Mock.Of<IPatternElementInfo>());
                Assert.False(result);
            }

            [TestMethod]
            public void WhenCreatingElementWithAnyManyRelationship_ThenAddAllowed()
            {
                var result = this.target.CanAddNewInstanceInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true &&
                    info.Cardinality == Cardinality.OneToMany));
                Assert.True(result);
                var result2 = this.target.CanAddNewInstanceInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true &&
                    info.Cardinality == Cardinality.ZeroToMany));
                Assert.True(result2);
            }

            [TestMethod]
            public void WhenCreatingElementWithAnyOneRelationshipAndNoExistingInstances_ThenAddAllowed()
            {
                var result = this.target.CanAddNewInstanceInternal(Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true &&
                    info.Cardinality == Cardinality.OneToOne));
                Assert.True(result);
            }

            [TestMethod]
            public void WhenCreatingElementWithAnyOneRelationshipAndExistingInstance_ThenAddNotAllowed()
            {
                var mockInfo = Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true &&
                    info.Cardinality == Cardinality.OneToOne);
                this.element.As<IAbstractElement>().Setup(e => e.Elements).Returns(new[] { Mock.Of<IAbstractElement>(e => e.Info == mockInfo) });

                var result = this.target.CanAddNewInstanceInternal(mockInfo);
                Assert.False(result);


                var mockInfo2 = Mock.Of<IAbstractElementInfo>(info => info.IsVisible == true && info.AllowAddNew == true &&
                    info.Cardinality == Cardinality.OneToOne);
                this.element.As<IAbstractElement>().Setup(e => e.Elements).Returns(new[] { Mock.Of<IAbstractElement>(e => e.Info == mockInfo2) });

                var result2 = this.target.CanAddNewInstanceInternal(mockInfo2);
                Assert.False(result2);
            }

            [TestMethod]
            public void WhenDeletingProduct_ThenDeleteAllowed()
            {
                this.element.As<IProductElement>().Setup(x => x.Info).Returns(Mocks.Of<IPatternInfo>().First());

                var result = this.target.CanDeleteInstanceInternal();
                Assert.True(result);
            }
        }

        [TestClass]
        [CLSCompliant(false)]
        public class GivenAnAbstractElement
        {
            private Mock<IAbstractElement> element;
            private TestElementViewModel<IAbstractElement> target;
            private SolutionBuilderContext context;

            [TestInitialize]
            public void Initialize()
            {
                this.element = new Mock<IAbstractElement>();
                this.element.As<IProductElement>().Setup(x => x.Info).Returns(Mocks.Of<IPatternElementInfo>().First());
                this.element.As<IAbstractElement>().Setup(e => e.Elements).Returns(Enumerable.Empty<IAbstractElement>());

                this.context = new SolutionBuilderContext
                {
                    PatternManager = Mock.Of<IPatternManager>(),
                    ShowProperties = () => { },
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                };

                this.target = new TestElementViewModel<IAbstractElement>(this.element.Object, this.context);
            }

            [TestMethod]
            public void WhenDeletingElementWithAnyZeroRelationship_ThenAllowed()
            {
                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.Cardinality == Cardinality.ZeroToOne));
                var result = this.target.CanDeleteInstanceInternal();
                Assert.True(result);
                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.Cardinality == Cardinality.ZeroToMany));
                var result2 = this.target.CanDeleteInstanceInternal();
                Assert.True(result2);
            }

            [TestMethod]
            public void WhenDeletingElementWithOneToOneRelationshipAndNotAllowNew_ThenAllowedIfAddMenuPresent()
            {
                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.AllowAddNew == false && info.Cardinality == Cardinality.OneToOne));
                var result = this.target.CanDeleteInstanceInternal();
                Assert.False(result);

                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.AllowAddNew == true && info.Cardinality == Cardinality.OneToOne));
                var result2 = this.target.CanDeleteInstanceInternal();
                Assert.True(result2);
            }

            [TestMethod]
            public void WhenDeletingElementWithOneToManyRelationshipAndNoSiblings_ThenAllowedIfAddMenuPresent()
            {
                this.element.Setup(x => x.Parent).Returns(Mock.Of<IElementContainer>(p => p.Elements == Enumerable.Empty<IAbstractElement>()));

                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.AllowAddNew == false && info.Cardinality == Cardinality.OneToMany));
                var result = this.target.CanDeleteInstanceInternal();
                Assert.False(result);
                this.element.Setup(x => x.Info).Returns(Mock.Of<IAbstractElementInfo>(info => info.AllowAddNew == true && info.Cardinality == Cardinality.OneToMany));
                var result2 = this.target.CanDeleteInstanceInternal();
                Assert.True(result2);
            }

            [TestMethod]
            public void WhenDeletingElementWithOneToManyRelationshipAndMoreThanOneSibling_ThenAllowed()
            {
                var mockInfo = Mock.Of<IAbstractElementInfo>(info => info.Cardinality == Cardinality.OneToMany);
                this.element.Setup(x => x.Parent).Returns(Mock.Of<IElementContainer>(p => p.Elements == new[] 
                { 
                    Mock.Of<IAbstractElement>(e => e.Info == mockInfo), 
                    Mock.Of<IAbstractElement>(e => e.Info == mockInfo) 
                }));

                this.element.Setup(x => x.Info).Returns(mockInfo);
                var result = this.target.CanDeleteInstanceInternal();
                Assert.True(result);
            }
        }

        [CLSCompliant(false)]
        public class TestProductElementViewModel<TElement> : ProductElementViewModel where TElement : IElementContainer
        {
            public TestProductElementViewModel(IProductElement element, SolutionBuilderContext context)
                : base(element, context)
            {
            }

            internal override IElementContainer ElementContainer
            {
                get { return (TElement)base.Model; }
            }

            protected override bool CanCreateAddMenu(IPatternElementInfo info)
            {
                return base.CanCreateAddMenu(info);
            }

            protected override bool CanAddNewInstance(IPatternElementInfo info)
            {
                return base.CanAddNewInstance(info);
            }

            protected override bool CanDeleteInstance()
            {
                return base.CanDeleteInstance();
            }

            internal bool CanCreateAddMenuInternal(IPatternElementInfo info)
            { return CanCreateAddMenu(info); }

            internal bool CanAddNewInstanceInternal(IPatternElementInfo info)
            { return CanAddNewInstance(info); }

            internal bool CanDeleteInstanceInternal()
            { return CanDeleteInstance(); }
        }

        [CLSCompliant(false)]
        public class TestElementViewModel<TElement> : ElementViewModel where TElement : IElementContainer
        {
            public TestElementViewModel(IAbstractElement element, SolutionBuilderContext context)
                : base(element, context)
            {
            }

            internal override IElementContainer ElementContainer
            {
                get { return (TElement)base.Model; }
            }

            protected override bool CanCreateAddMenu(IPatternElementInfo info)
            {
                return base.CanCreateAddMenu(info);
            }

            protected override bool CanAddNewInstance(IPatternElementInfo info)
            {
                return base.CanAddNewInstance(info);
            }

            protected override bool CanDeleteInstance()
            {
                return base.CanDeleteInstance();
            }

            internal bool CanCreateAddMenuInternal(IPatternElementInfo info)
            { return CanCreateAddMenu(info); }

            internal bool CanAddNewInstanceInternal(IPatternElementInfo info)
            { return CanAddNewInstance(info); }

            internal bool CanDeleteInstanceInternal()
            { return CanDeleteInstance(); }
        }
    }
}