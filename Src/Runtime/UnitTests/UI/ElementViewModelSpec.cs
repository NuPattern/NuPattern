using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Presentation;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.VisualStudio.Shell;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UnitTests.UI
{
    public class ElementViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullElement_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new ElementViewModel(null, new SolutionBuilderContext()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullContext_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new ElementViewModel(new Mock<IAbstractElement>().Object, null));
            }
        }

        [TestClass]
        public class GivenAnElement
        {
            private IAbstractElement element;
            private ElementViewModel target;

            [TestInitialize]
            public void Initialize()
            {
                var child1 = Create<ICollection, ICollectionInfo>("Bar1", Cardinality.ZeroToMany);
                var child2 = Create<IElement, IElementInfo>("Bar2", Cardinality.OneToOne);

                this.element = Create<IElement, IElementInfo>("Foo1", Cardinality.ZeroToMany, child1, child2);

                var children = (List<IAbstractElementInfo>)this.element.Info.Elements;

                children.Add(Mocks.Of<IElementInfo>().First(e =>
                    e.Name == "Element 1" &&
                    e.DisplayName == "Element 1" &&
                    e.IsVisible == true &&
                    e.AllowAddNew == true &&
                    e.Cardinality == Cardinality.ZeroToMany));

                children.Add(Mocks.Of<ICollectionInfo>().First(e =>
                    e.Name == "Collection 1" &&
                    e.DisplayName == "Collection 1" &&
                    e.IsVisible == true &&
                    e.AllowAddNew == true &&
                    e.Cardinality == Cardinality.ZeroToMany));

                var newNodeDialog = new Mock<IDialogWindow>();
                newNodeDialog.Setup(x => x.ShowDialog()).Returns(true);

                var ctx = new SolutionBuilderContext
                {
                    PatternManager = new Mock<IPatternManager>().Object,
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                    ShowProperties = () => { },
                    NewNodeDialogFactory = x => newNodeDialog.Object
                };

                var serviceProvider = Mocks.Of<IServiceProvider>()
                    .First(x => x.GetService(typeof(ISolutionEvents)) == new Mock<ISolutionEvents>().Object);
                var explorer = new SolutionBuilderViewModel(ctx, serviceProvider);

                this.target = new ElementViewModel(this.element, ctx);

                this.target.RenderHierarchyRecursive();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenExposesModel()
            {
                Assert.Same(this.element, this.target.Model);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenExposesIconPath()
            {
                Assert.Equal(string.Format(CultureInfo.InvariantCulture, ElementViewModel.IconPathFormat, this.element.GetType().Name), this.target.IconPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenRenderChildNodes()
            {
                Assert.Equal(2, this.target.Nodes.Count);
                Assert.Contains(this.element.AllElements.ElementAt(0), this.target.Nodes.Select(n => n.Model));
                Assert.Contains(this.element.AllElements.ElementAt(1), this.target.Nodes.Select(n => n.Model));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNew_ThenAddMenuElements()
            {
                var addMenuOptions = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText)
                    .MenuOptions;

                Assert.Equal(4, addMenuOptions.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenCreatesAddMenuOption()
            {
                var options = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);

                Assert.NotNull(options);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingNewElement_ThenCreatesCollectionInStore()
            {
                var addMenuOptions = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText)
                    .MenuOptions;
                var info = addMenuOptions.Select(o => o.Model)
                    .OfType<IElementInfo>()
                    .Last(e => e.Cardinality == Cardinality.ZeroToMany);

                this.target.AddElementCommand.Execute(info);

                Mock.Get(this.target.Model)
                    .Verify(x => x.CreateElement(It.IsAny<Action<IElement>>(), It.IsAny<bool>()), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingNewCollection_ThenCreatesCollectionInStore()
            {
                var addMenuOptions = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText)
                    .MenuOptions;
                var info = addMenuOptions.Select(o => o.Model)
                    .OfType<ICollectionInfo>()
                    .Last(e => e.Cardinality == Cardinality.ZeroToMany);

                this.target.AddElementCommand.Execute(info);

                Mock.Get(this.target.Model)
                    .Verify(x => x.CreateCollection(It.IsAny<Action<ICollection>>(), It.IsAny<bool>()), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDelete_ThenPromptsConfirmation()
            {
                var toDelete = this.target.Nodes.OfType<ElementViewModel>().First();

                toDelete.DeleteCommand.Execute(null);

                Mock.Get(this.target.Context.UserMessageService)
                    .Verify(x => x.PromptWarning(It.IsAny<string>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecutingDelete_ThenDeletesElementAndRemoveNodeFromNodesParent()
            {
                var toDelete = this.target.Nodes.OfType<ElementViewModel>().First();

                toDelete.DeleteCommand.Execute(null);

                Assert.False(this.target.Nodes.Contains(toDelete));
                Mock.Get((ICollection)toDelete.Model).Verify(e => e.Delete(), Times.Once());
            }
        }

        [TestClass]
        public class GivenAnElementWithExtensionPoints
        {
            private IAbstractElement element;
            private ElementViewModel target;

            [TestInitialize]
            public void Initialize()
            {
                var patternManager = new Mock<IPatternManager>();
                patternManager.Setup(x => x.InstalledToolkits).Returns(Enumerable.Empty<IInstalledToolkitInfo>());

                this.element = Create<IElement, IElementInfo>("Foo", Cardinality.OneToOne);

                var extensions = (List<IExtensionPointInfo>)this.element.Info.ExtensionPoints;
                extensions.Add(Mocks.Of<IExtensionPointInfo>().First(x => x.DisplayName == "Foo"));
                extensions.Add(Mocks.Of<IExtensionPointInfo>().First(x => x.DisplayName == "Bar"));

                var ctx = new SolutionBuilderContext
                {
                    PatternManager = patternManager.Object,
                    ShowProperties = () => { }
                };

                this.target = new ElementViewModel(this.element, ctx);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNew_ThenAddExtensionPointsInAddMenuOption()
            {
                var addOption = this.target.MenuOptions
                    .First(x => x.Caption == Resources.ProductElementViewModel_AddMenuText);

                Assert.Equal(2, addOption.MenuOptions.Count);
            }
        }

        private static T Create<T, TInfo>(string name, Cardinality cardinality, params IAbstractElement[] elements)
            where T : class, IAbstractElement
            where TInfo : class, IAbstractElementInfo
        {
            var info = Mocks.Of<TInfo>().First(e =>
                e.Name == name &&
                e.DisplayName == name &&
                e.IsVisible == true &&
                e.AllowAddNew == true &&
                e.Cardinality == cardinality &&
                e.Elements == elements.Select(x => x.Info).ToList() &&
                e.ExtensionPoints == new List<IExtensionPointInfo>());

            var elementMock = new Mock<T>();
            elementMock.Setup(e => e.InstanceName).Returns(name);
            elementMock.Setup(e => e.AllElements).Returns(elements);
            elementMock.Setup(e => e.Info).Returns(info);
            elementMock.As<IElementContainer>().Setup(e => e.Info).Returns(info);
            elementMock.As<IAbstractElement>().Setup(e => e.Info).Returns(info);
            elementMock.As<IProductElement>().Setup(e => e.Info).Returns(info);
            return elementMock.Object;
        }
    }
}
