using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Presentation;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UnitTests.UI
{
    public class ProductViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private SolutionBuilderContext ctx;

            [TestInitialize]
            public void Initialize()
            {
                this.ctx = new SolutionBuilderContext
                {
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                    PatternManager = Mocks.Of<IPatternManager>().First(),
                    ShowProperties = () => { }
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullProduct_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new ProductViewModel(null, this.ctx));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullContext_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new ProductViewModel(new Mock<IProduct>().Object, null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithInfo_ThenShowsEnabledIcon()
            {
                var viewInfo = Mocks.Of<IViewInfo>().First(v => v.Name == "View1" && v.IsDefault);
                var view = new Mock<IView>();
                view.Setup(v => v.AllElements).Returns(new IAbstractElement[0]);
                view.Setup(v => v.Info).Returns(viewInfo);
                view.As<IElementContainer>().Setup(v => v.Info).Returns(viewInfo);

                var productInfo = new Mock<IPatternInfo>().Object;
                var product = new Mock<IProduct>();
                product.Setup(p => p.InstanceName).Returns("Foo");
                product.Setup(p => p.Info).Returns(productInfo);
                product.As<IProductElement>().Setup(x => x.Info).Returns(productInfo);
                product.Setup(p => p.Views).Returns(new[] { view.Object });
                product.Setup(p => p.CurrentView).Returns(view.Object);
                product.Setup(p => p.Info.Views)
                    .Returns(product.Object.Views.Select(p => p.Info).ToArray());

                var target = new ProductViewModel(product.Object, this.ctx);

                Assert.Equal("../../Resources/NodeProductDefault.png", target.IconPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithMissingInfo_ThenShowsUninstalledIcon()
            {
                var product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == Mocks.Of<IView>().Where(v => v.Info.Name == "View1" && v.Info.IsDefault && v.AllElements == new IAbstractElement[0]).Take(1).ToArray());

                var target = new ProductViewModel(product, this.ctx);

                Assert.Equal("../../Resources/NodeProductVersionNotFound.png", target.IconPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithOneView_ThenDoesNotAddViewsMenuOption()
            {
                var product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == Mocks.Of<IView>().Where(v => v.Info.Name == "View1" && v.Info.IsDefault == true && v.AllElements == new IAbstractElement[0]).Take(1).ToArray());

                var target = new ProductViewModel(product, this.ctx);

                Assert.False(target.MenuOptions.Any(o => o.Caption == Resources.ProductViewModel_ViewsMenuText));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecutingDelete_ThenInvokesPatternManagerDelete()
            {
                var product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == Mocks.Of<IView>().Where(v => v.Info.Name == "View1" && v.Info.IsDefault == true && v.AllElements == new IAbstractElement[0]).Take(1).ToArray());

                var target = new ProductViewModel(product, this.ctx);
                target.DeleteCommand.Execute(null);

                Mock.Get(this.ctx.PatternManager).Verify(p => p.DeleteProduct(product));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductDoesNotHaveInfo_ThenLoadsProductWithoutAnyChildren()
            {
                var product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == new[]
                    {
                        Mocks.Of<IView>().First(v => v.Info.Name == "View1" && v.Info.IsDefault == true && v.AllElements == new[] { Mocks.Of<IElement>().First() })
                    });

                var target = new ProductViewModel(product, this.ctx);

                Assert.Equal(0, target.ChildNodes.Count);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductDoesNotHaveInfo_ThenLoadsProductWithDeleteAndPropertiesMenuItemsOnly()
            {
                var product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == new[]
                    {
                        Mocks.Of<IView>().First(v => v.Info.Name == "View1" && v.Info.IsDefault == true && v.AllElements == new[] { Mocks.Of<IElement>().First() })
                    });

                var target = new ProductViewModel(product, this.ctx);

                Assert.Equal(3, target.MenuOptions.Count);
                Assert.True(target.MenuOptions.Any(x => x.Caption == "Delete"));
                Assert.True(target.MenuOptions.Any(x => x.Caption == "Properties"));
                Assert.True(target.MenuOptions.Any(x => x.Caption == "Rename"));
            }
        }

        [TestClass]
        public class GivenAProductWithTwoViews
        {
            private IProduct product;
            private ProductViewModel target;
            private IPatternManager patternManager;
            private SolutionBuilderContext context;

            [TestInitialize]
            public void Initialize()
            {
                var hiddenView = CreateView("HiddenView", false);
                Mock.Get(hiddenView.Info).Setup(x => x.IsVisible).Returns(false);

                var views = new[]
                {
                    CreateView("View1", false, Create<IElement, IElementInfo>("Element1", Cardinality.OneToOne)),
                    CreateView("View2", true, Create<ICollection, ICollectionInfo>("Collection2", Cardinality.OneToOne), Create<IElement, IElementInfo>("Element2", Cardinality.ZeroToMany)), 
                    hiddenView,
                };

                var view1Children = (List<IAbstractElementInfo>)views[0].Info.Elements;
                Mock.Get((IElementInfo)view1Children[0])
                    .Setup(e => e.Parent)
                    .Returns(views[0].Info);

                var view2Children = (List<IAbstractElementInfo>)views[1].Info.Elements;
                Mock.Get((ICollectionInfo)view2Children[0])
                    .Setup(e => e.Parent)
                    .Returns(views[1].Info);
                Mock.Get((IElementInfo)view2Children[1])
                    .Setup(e => e.Parent)
                    .Returns(views[1].Info);
                view2Children.Add(Mocks.Of<IElementInfo>().First(e =>
                    e.Name == "Foo1" &&
                    e.IsVisible == true &&
                    e.AllowAddNew == true &&
                    e.DisplayName == "Foo1" &&
                    e.Cardinality == Cardinality.ZeroToMany));
                view2Children.Add(Mocks.Of<ICollectionInfo>().First(e =>
                    e.Name == "Foo2" &&
                    e.IsVisible == true &&
                    e.AllowAddNew == true &&
                    e.DisplayName == "Foo2" &&
                    e.Cardinality == Cardinality.ZeroToMany));

                this.product = Mocks.Of<IProduct>().First(p =>
                    p.InstanceName == "Foo" &&
                    p.Views == views &&
                    p.CurrentView == views.FirstOrDefault(v => v.Info.IsDefault) &&
                    p.Info == new Mock<IPatternInfo>().Object);

                this.patternManager = Mocks.Of<IPatternManager>()
                    .First(x => x.InstalledToolkits == Enumerable.Empty<IInstalledToolkitInfo>());

                var newNodeDialog = new Mock<IDialogWindow>();
                newNodeDialog.Setup(x => x.ShowDialog()).Returns(true);

                this.context = new SolutionBuilderContext
                {
                    PatternManager = this.patternManager,
                    UserMessageService = Mock.Of<IUserMessageService>(ums => ums.PromptWarning(It.IsAny<string>()) == true),
                    ShowProperties = () => { },
                    NewNodeDialogFactory = x => newNodeDialog.Object,
                };

                var serviceProvider = Mocks.Of<IServiceProvider>()
                    .First(x => x.GetService(typeof(ISolutionEvents)) == new Mock<ISolutionEvents>().Object);
                var explorer = new SolutionBuilderViewModel(context, serviceProvider);

                this.target = new ProductViewModel(this.product, context);

                this.target.RenderHierarchyRecursive();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenExposesModel()
            {
                Assert.Same(this.product, this.target.Data);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenCreatesAddMenuOption()
            {
                var options = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);

                Assert.NotNull(options);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenLoadViews()
            {
                Assert.Equal(
                    this.product.Views.Count(),
                    this.target.MenuOptions
                        .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                        .MenuOptions
                        .Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenViewIsHidden_ThenOptionIsNotVisible()
            {
                Assert.True(this.target.MenuOptions
                        .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                        .MenuOptions
                        .Any(x => !x.IsVisible));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOptionsAreAdded_ThenListIsSortedByDisplayName()
            {
                Assert.Equal("HiddenView", this.target.MenuOptions
                        .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                        .MenuOptions
                        .Select(o => o.Caption)
                        .First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOptionsAreAdded_ThenDisplayNameIsUsedForMenu()
            {
                Assert.True(this.target.MenuOptions
                        .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                        .MenuOptions
                        .All(o => o.Caption == ((IView)o.Data).Info.DisplayName));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewInstance_ThenDefaultViewIsSelectedAsCurrentView()
            {
                Assert.Same(this.product.Views.ElementAt(1), this.target.CurrentViewData);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingCurrentView_ThenRenderNodesFromSelectedView()
            {
                var view = (IView)this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                    .MenuOptions
                    .First(o => !o.IsSelected && o.IsVisible)
                    .Data;

                this.target.ChangeViewCommand.Execute(view);

                Assert.Equal(1, this.target.ChildNodes.Count);
                Assert.Contains(this.target.CurrentViewData.AllElements.ElementAt(0), this.target.ChildNodes.Select(n => n.Data));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNew_ThenAddMenuOptions()
            {
                var viewsOption = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);

                Assert.Equal(4, viewsOption.MenuOptions.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingCurrentView_ThenRefreshAddMenuElements()
            {
                var addOption = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);
                var view = (IView)this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductViewModel_ViewsMenuText)
                    .MenuOptions
                    .First(o => !o.IsSelected && o.IsVisible)
                    .Data;

                this.target.ChangeViewCommand.Execute(view);

                Assert.Equal(1, addOption.MenuOptions.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingNewElement_ThenCreatesElementInStore()
            {
                var addOption = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);
                var info = addOption.MenuOptions
                    .Select(o => o.Data)
                    .OfType<IElementInfo>()
                    .Last(e => e.Cardinality == Cardinality.ZeroToMany && e.IsVisible);

                this.target.AddElementCommand.Execute(info);

                Mock.Get(this.target.CurrentViewData)
                    .Verify(x => x.CreateElement(It.IsAny<Action<IElement>>(), It.IsAny<bool>()), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingNewCollection_ThenCreateCollectionInStore()
            {
                SolutionBuilderViewModelSpec.SetupCreateCollection(this.patternManager, this.target.CurrentViewData);

                var addOption = this.target.MenuOptions
                    .First(o => o.Caption == Resources.ProductElementViewModel_AddMenuText);
                var info = addOption.MenuOptions
                    .Select(o => o.Data)
                    .OfType<ICollectionInfo>()
                    .Last(e => e.Cardinality == Cardinality.ZeroToMany && e.IsVisible);

                this.target.AddElementCommand.Execute(info);

                Mock.Get(this.target.CurrentViewData)
                    .Verify(x => x.CreateCollection(It.IsAny<Action<ICollection>>(), It.IsAny<bool>()), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDelete_ThenPromptsConfirmation()
            {
                this.target.DeleteCommand.Execute(null);

                Mock.Get(this.context.UserMessageService)
                    .Verify(x => x.PromptWarning(It.IsAny<string>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeleteThrows_ThenShowsMessageError()
            {
                Mock.Get(this.patternManager).Setup(x => x.DeleteProduct(this.product)).Throws<InvalidOperationException>();

                this.target.DeleteCommand.Execute(null);

                Mock.Get(this.context.UserMessageService)
                    .Verify(x => x.ShowError(It.IsAny<string>()));
            }
        }

        private static T Create<T, TInfo>(string name, Cardinality cardinality, IViewInfo view = null, params IAbstractElement[] elements)
            where T : class, IAbstractElement
            where TInfo : class, IAbstractElementInfo
        {
            var info = Mocks.Of<TInfo>().First(e =>
                e.Name == name &&
                e.IsVisible == true &&
                e.AllowAddNew == true &&
                e.DisplayName == name &&
                e.Cardinality == cardinality &&
                e.Parent == view &&
                e.Elements == elements.Select(elem => elem.Info).ToArray());

            var element = new Mock<T>();
            element.Setup(e => e.InstanceName).Returns(name);
            element.Setup(e => e.AllElements).Returns(elements);
            element.Setup(e => e.Info).Returns(info);
            element.As<IElementContainer>().Setup(e => e.Info).Returns(info);
            return element.Object;
        }

        private static IView CreateView(string name, bool isDefault, params IAbstractElement[] elements)
        {
            var info = Mocks.Of<IViewInfo>()
                .First(v =>
                    v.Name == name &&
                    v.DisplayName == name &&
                    v.IsDefault == isDefault &&
                    v.IsVisible == true &&
                    v.Elements == elements.Select(e => e.Info).ToList());

            var view = new Mock<IView>();
            view.Setup(v => v.DefinitionName).Returns(name);
            view.Setup(v => v.AllElements).Returns(elements.ToList());
            view.Setup(v => v.Info).Returns(info);
            view.As<IElementContainer>().Setup(v => v.Info).Returns(info);
            return view.Object;
        }
    }
}
