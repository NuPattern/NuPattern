using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.UnitTests.UI
{
    public class SolutionBuilderViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private SolutionBuilderContext ctx;

            [TestInitialize]
            public void Initilize()
            {
                this.ctx = new SolutionBuilderContext
                {
                    PatternManager = new Mock<IPatternManager>().Object,
                    NewProductDialogFactory = x => new Mock<IDialogWindow>(x).Object,
                    NewNodeDialogFactory = x => new Mock<IDialogWindow>(x).Object,
                    UserMessageService = new Mock<IUserMessageService>().Object
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullContext_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SolutionBuilderViewModel(null, new Mock<IServiceProvider>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullServiceProvider_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new SolutionBuilderViewModel(new SolutionBuilderContext(), null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewAndSolutionEventsNotFound_ThenThrowsInvalidOperationException()
            {
                var serviceProvider = new Mock<IServiceProvider>();
                serviceProvider.Setup(x => x.GetService(typeof(IPatternManager)))
                    .Returns(new Mock<IPatternManager>().Object);

                Assert.Throws<InvalidOperationException>(() => new SolutionBuilderViewModel(this.ctx, serviceProvider.Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionIsNotOpen_ThenAddNewProductIsDisabled()
            {
                var target = new SolutionBuilderViewModel(this.ctx, GetServiceProvider());

                Assert.False(target.AddNewProductCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionIsOpened_ThenAddNewProductIsEnabled()
            {
                var serviceProvider = GetServiceProvider();

                var solutionEvents = Mock.Get(serviceProvider.GetService<ISolutionEvents>());
                solutionEvents.Setup(x => x.IsSolutionOpened).Returns(true);

                var target = new SolutionBuilderViewModel(this.ctx, serviceProvider);

                Assert.True(target.AddNewProductCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInvokingGuidanceAndGuidanceNotInstalled_ThenDisablesGuidance()
            {
                var serviceProvider = GetServiceProvider();

                var guidanceManager = Mock.Get(serviceProvider.GetService<IGuidanceManager>());
                guidanceManager.Setup(x => x.InstalledGuidanceExtensions).Returns(Enumerable.Empty<IGuidanceExtensionRegistration>());

                var target = new SolutionBuilderViewModel(this.ctx, serviceProvider);

                target.GuidanceCommand.Execute(null);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInvokingGuidanceAndGuidanceInstantiated_ThenActivatesGuidance()
            {
                var serviceProvider = GetServiceProvider();

                var extension = Mocks.Of<IGuidanceExtension>().First(x => x.ExtensionId == SolutionBuilderViewModel.UsingGuidanceExtensionId);

                var guidanceManager = Mock.Get(serviceProvider.GetService<IGuidanceManager>());
                guidanceManager.Setup(x => x.InstalledGuidanceExtensions)
                    .Returns(new[] { Mocks.Of<IGuidanceExtensionRegistration>().First(x => x.ExtensionId == SolutionBuilderViewModel.UsingGuidanceExtensionId) });
                guidanceManager.Setup(x => x.InstantiatedGuidanceExtensions)
                    .Returns(new[] { extension });

                var target = new SolutionBuilderViewModel(this.ctx, serviceProvider);
                target.GuidanceCommand.Execute(null);

                guidanceManager.VerifySet(x => x.ActiveGuidanceExtension = extension);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInvokingGuidanceAndGuidanceNotInstantiated_ThenInstantiatesGuidance()
            {
                var serviceProvider = GetServiceProvider();

                var guidanceManager = Mock.Get(serviceProvider.GetService<IGuidanceManager>());
                guidanceManager.Setup(x => x.InstalledGuidanceExtensions)
                    .Returns(new[] { Mocks.Of<IGuidanceExtensionRegistration>().First(x => x.ExtensionId == SolutionBuilderViewModel.UsingGuidanceExtensionId) });
                guidanceManager.Setup(x => x.InstantiatedGuidanceExtensions)
                    .Returns(Enumerable.Empty<IGuidanceExtension>());

                var target = new SolutionBuilderViewModel(this.ctx, serviceProvider);
                target.GuidanceCommand.Execute(null);

                guidanceManager.Verify(manager => manager.Instantiate(SolutionBuilderViewModel.UsingGuidanceExtensionId, It.IsAny<string>()), Times.Once());
            }
        }

        [TestClass]
        public class GivenAnOpenedSolution
        {
            private Mock<IPatternManager> patternManager;
            private Mock<IDialogWindow> dialog;
            private SolutionBuilderViewModel target;
            private SolutionBuilderContext context;

            [TestInitialize]
            public void Initialize()
            {
                var serviceProvider = GetServiceProvider();

                IProduct product = null;

                this.patternManager = new Mock<IPatternManager>();
                this.patternManager.Setup(p => p.CreateProduct(It.IsAny<IInstalledToolkitInfo>(), It.IsAny<string>(), true))
                    .Callback(() =>
                    {
                        product = Mocks.Of<IProduct>().First(p =>
                            p.Info.Name == "Foo" &&
                            p.Views == GetViews() &&
                            p.CurrentView == GetViews().FirstOrDefault(v => v.Info.IsDefault) &&
                            p.InstanceName == Path.GetRandomFileName());
                        this.patternManager.Setup(p => p.Products).Returns(new[] { product });
                        this.patternManager.Raise(p => p.ElementCreated += null, new ValueEventArgs<IProductElement>(product));
                        this.patternManager.Raise(p => p.ElementInstantiated += null, new ValueEventArgs<IProductElement>(product));
                    })
                    .Returns(() => product);

                var solutionListener = Mock.Get(serviceProvider.GetService<ISolutionEvents>());
                solutionListener.Setup(sl => sl.IsSolutionOpened).Returns(true);

                this.dialog = new Mock<IDialogWindow>();

                this.context = new SolutionBuilderContext
                {
                    PatternManager = patternManager.Object,
                    NewProductDialogFactory = x => this.dialog.Object,
                    NewNodeDialogFactory = x => new Mock<IDialogWindow>().Object,
                    UserMessageService = new Mock<IUserMessageService>().Object,
                    ShowProperties = () => { }
                };

                this.target = new SolutionBuilderViewModel(this.context, serviceProvider);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProduct_ThenShowDialogIsCalled()
            {
                this.target.AddNewProductCommand.Execute(null);

                this.dialog.Verify(d => d.ShowDialog(), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProductAndDialogCanceled_ThenDoesNotAddProductToProducts()
            {
                this.dialog.Setup(d => d.ShowDialog()).Returns(false);

                this.target.AddNewProductCommand.Execute(null);

                Assert.Equal(0, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProductAndCancelExceptionThrown_ThenShowsCancelExceptionMessage()
            {
                Mock.Get(this.context.NewProductDialogFactory.Invoke(null))
                    .Setup(x => x.ShowDialog())
                    .Returns(true);

                this.patternManager
                    .Setup(x => x.CreateProduct(It.IsAny<IInstalledToolkitInfo>(), It.IsAny<string>(), true))
                    .Throws(new OperationCanceledException("foo"));

                this.target.AddNewProductCommand.Execute(null);

                Mock.Get(this.context.UserMessageService)
                    .Verify(x => x.ShowError("foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProduct_ThenAddsNewProductToProducts()
            {
                this.dialog.Setup(d => d.ShowDialog()).Returns(true);
                this.dialog.SetupSet(d => d.DataContext = It.IsAny<object>());

                this.target.AddNewProductCommand.Execute(null);

                this.patternManager.Verify(p => p.CreateProduct(It.IsAny<IInstalledToolkitInfo>(), It.IsAny<string>(), true), Times.Once());
                Assert.Equal(1, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProductInStore_ThenAddsProductToProducts()
            {
                Assert.Equal(0, this.target.TopLevelNodes.Count());

                this.patternManager.Object.CreateProduct(new Mock<IInstalledToolkitInfo>().Object, "Foo");

                Assert.Equal(1, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingProduct_ThenSetsAsCurrentNodeAndRaisesCurrentNodeChanged()
            {
                var changeRaised = false;
                this.dialog.Setup(d => d.ShowDialog()).Returns(true);
                this.dialog.SetupSet(d => d.DataContext = It.IsAny<object>());

                this.target.CurrentNodeChanged += (s, e) => changeRaised = true;
                this.target.AddNewProductCommand.Execute(null);

                Assert.True(changeRaised);
                Assert.NotNull(this.target.CurrentNode);
            }
        }

        [TestClass]
        public class GivenAProductStateWithTwoProducts
        {
            private SolutionBuilderViewModel target;
            private Mock<IPatternManager> patternManager;
            private Mock<ISolutionEvents> solutionEvents;

            [TestInitialize]
            public void Initialize()
            {
                var serviceProvider = GetServiceProvider();

                this.patternManager = new Mock<IPatternManager>();
                this.patternManager.Setup(p => p.IsOpen).Returns(true);

                var products = Mocks.Of<IProduct>().Where(p =>
                    p.Views == GetViews() &&
                    p.CurrentView == GetViews().FirstOrDefault(v => v.Info.IsDefault) &&
                    p.InstanceName == Path.GetRandomFileName() &&
                    p.Info == new Mock<IPatternInfo>().Object &&
                    p.BeginTransaction() == new Mock<ITransaction>().Object)
                    .Take(2)
                    .ToList();

                this.patternManager.Setup(p => p.Products).Returns(products);
                this.patternManager.Setup(p => p.DeleteProduct(It.IsAny<IProduct>()))
                    .Callback<IProduct>((p) =>
                    {
                        products.Remove(p);
                        this.patternManager.Raise(m => m.ElementDeleted += null, new ValueEventArgs<IProductElement>(p));
                    })
                    .Returns(true);

                this.solutionEvents = Mock.Get(serviceProvider.GetService<ISolutionEvents>());
                this.solutionEvents.Setup(e => e.IsSolutionOpened).Returns(true);

                var ctx = new SolutionBuilderContext
                {
                    PatternManager = this.patternManager.Object,
                    NewProductDialogFactory = x => new Mock<IDialogWindow>().Object,
                    NewNodeDialogFactory = x => new Mock<IDialogWindow>().Object,
                    ShowProperties = () => { },
                    UserMessageService = new Mock<IUserMessageService>().Object
                };

                this.target = new SolutionBuilderViewModel(ctx, serviceProvider);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenLoadProducts()
            {
                Assert.Equal(2, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingCurrentNode_TheChangesCurrentNodeAndRaisesPropertyChanged()
            {
                var propertyChangedRaised = false;
                var eventRaised = false;

                this.target.TopLevelNodes.ElementAt(0).IsSelected = true;
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflect<SolutionBuilderViewModel>.GetProperty(x => x.CurrentNode).Name)
                    {
                        propertyChangedRaised = true;
                    }
                };
                this.target.CurrentNodeChanged += (s, e) => eventRaised = true;
                this.target.TopLevelNodes.ElementAt(1).IsSelected = true;

                Assert.True(propertyChangedRaised);
                Assert.True(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingCurrentNodeToTheSame_ThenDoesNotRaisePropertyChanged()
            {
                var propertyChangedRaised = false;
                var eventRaised = false;

                this.target.TopLevelNodes.First().IsSelected = true;
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflect<SolutionBuilderViewModel>.GetProperty(x => x.CurrentNode).Name)
                    {
                        propertyChangedRaised = true;
                    }
                };
                this.target.CurrentNodeChanged += (s, e) => eventRaised = true;
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.False(propertyChangedRaised);
                Assert.False(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemovingProductFromStore_ThenRemovesProductFromProducts()
            {
                Assert.Equal(2, this.target.TopLevelNodes.Count());

                this.patternManager.Object.DeleteProduct(this.target.TopLevelNodes.Cast<ProductViewModel>().First().Data);

                Assert.Equal(1, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenClosingSolution_ThenAddNewProductIsDisabled()
            {
                this.solutionEvents.Raise(e => e.SolutionClosed += null, new SolutionEventArgs(null));

                Assert.False(this.target.AddNewProductCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenClosingPatternManager_ThenProductsAreRemovedFromTheScreen()
            {
                this.patternManager.SetupGet(p => p.IsOpen).Returns(false);
                this.patternManager.Raise(e => e.IsOpenChanged += null, EventArgs.Empty);

                Assert.Equal(0, this.target.TopLevelNodes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenClosingPatternManagerAndSolutionStillOpened_ThenAddNewProductIsEnabled()
            {
                this.patternManager.SetupGet(p => p.IsOpen).Returns(false);
                this.patternManager.Raise(e => e.IsOpenChanged += null, EventArgs.Empty);

                Assert.True(this.target.AddNewProductCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeletingNodeAndCurrentNodeIsNull_ThenCanDeleteNodeReturnsFalse()
            {
                Assert.False(this.target.DeleteCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeletingNodeAndCurrentNodeIsSelected_ThenCanDeleteNodeReturnsTrue()
            {
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.True(this.target.DeleteCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBeginingEditNodeAndCurrentNodeIsNull_ThenCanBeginEditNodeReturnsFalse()
            {
                Assert.False(this.target.BeginEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBeginingEditNodeAndCurrentNodeIsSelected_ThenCanBeginEditNodeReturnsTrue()
            {
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.True(this.target.BeginEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBeginingEdition_ThenSetsIsEditingToTrue()
            {
                var product = this.target.TopLevelNodes.Cast<ProductViewModel>().First();
                var productMock = Mock.Get(product.Data);
                product.IsSelected = true;

                this.target.BeginEditCommand.Execute(null);

                Assert.True(product.IsEditing);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEndingEditNodeAndCurrentNodeIsNull_ThenCanEndEditNodeReturnsFalse()
            {
                Assert.False(this.target.EndEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCancelingEditNodeAndCurrentNodeIsNull_ThenCanCancelEditNodeReturnsFalse()
            {
                Assert.False(this.target.CancelEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEndingEditNodeAndCurrentNodeDoesNotEditing_ThenCanEndEditNodeReturnsFalse()
            {
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.False(this.target.EndEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCancelingEditNodeAndCurrentNodeDoesNotEditing_ThenCanCancelEditNodeReturnsFalse()
            {
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.False(this.target.CancelEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEndingEditNodeAndCurrentNodeIsEditing_ThenCanEndEditNodeReturnsTrue()
            {
                this.target.TopLevelNodes.First().IsSelected = true;
                this.target.BeginEditCommand.Execute(null);

                Assert.True(this.target.EndEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCancelingEditNodeAndCurrentNodeIsEditing_ThenCanCancelEditNodeReturnsTrue()
            {
                this.target.TopLevelNodes.First().IsSelected = true;
                this.target.BeginEditCommand.Execute(null);

                Assert.True(this.target.CancelEditCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenEndingEdition_ThenSetsIsEditingToFalse()
            {
                var productViewModel = this.target.TopLevelNodes.First();
                productViewModel.IsSelected = true;
                this.target.BeginEditCommand.Execute(null);


                this.target.EndEditCommand.Execute(null);

                Assert.False(productViewModel.IsEditing);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCancelingEdition_ThenSetsIsEditingToFalse()
            {
                var productViewModel = this.target.TopLevelNodes.First();
                productViewModel.IsSelected = true;
                this.target.BeginEditCommand.Execute(null);

                this.target.CancelEditCommand.Execute(null);

                Assert.False(productViewModel.IsEditing);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenActivatingNodeAndCurrentNodeIsNull_ThenCanActivateNodeReturnsFalse()
            {
                Assert.False(this.target.ActivateCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenActivatingNodeAndCurrentNodeIsSelected_ThenCanActivateNodeReturnsTrue()
            {
                this.target.TopLevelNodes.First().IsSelected = true;

                Assert.True(this.target.ActivateCommand.CanExecute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenActivatingNode_ThenInvokesPatternManagerActivateElement()
            {
                var element = this.target.TopLevelNodes.First();
                element.IsSelected = true;

                this.target.ActivateCommand.Execute(null);

                this.patternManager.Verify(x => x.ActivateElement(element.Data), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingElementToView_ThenAddsTheElement()
            {
                var product = this.target.TopLevelNodes.First();
                var view = ((ProductViewModel)product).CurrentViewData;
                SetupCreateElement(this.patternManager.Object, view);

                Assert.Equal(2, product.ChildNodes.Count);

                view.CreateElement();

                Assert.Equal(3, product.ChildNodes.Count);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingCollectionToView_ThenAddsTheCollection()
            {
                var product = this.target.TopLevelNodes.First();
                var view = ((ProductViewModel)product).CurrentViewData;
                SetupCreateCollection(this.patternManager.Object, view);

                view.CreateCollection();

                Assert.Equal(3, product.ChildNodes.Count);
            }
        }

        private static IServiceProvider GetServiceProvider()
        {
            return Mocks.Of<IServiceProvider>().First(t =>
                t.GetService(typeof(ISolutionEvents)) == new Mock<ISolutionEvents>().Object &&
                t.GetService(typeof(IGuidanceManager)) == new Mock<IGuidanceManager>().Object);
        }

        private static IView[] GetViews()
        {
            var info = Mocks.Of<IViewInfo>().First(v => v.IsDefault);

            var view = new Mock<IView>();
            view.Setup(v => v.Info).Returns(info);
            view.As<IElementContainer>().Setup(v => v.Info).Returns(info);

            var elements = new List<IAbstractElement>
            {
                Mocks.Of<IElement>()
                    .First(e =>	e.Info.Parent == view.Object.Info && e.Info.Cardinality == Cardinality.OneToOne && e.InstanceName == "1first"),
                Mocks.Of<ICollection>()
                    .First(e =>	e.Info.Parent == view.Object.Info && e.Info.Cardinality == Cardinality.OneToOne && e.InstanceName == "3third")
            };

            view.Setup(v => v.AllElements).Returns(elements);

            return new[] { view.Object };
        }

        internal static void SetupCreateCollection<T>(IPatternManager manager, T container) where T : class, IElementContainer
        {
            ICollection element = null;
            Mock.Get<T>(container)
                .Setup(v => v.CreateCollection(It.IsAny<Action<ICollection>>(), It.IsAny<bool>()))
                .Callback(() =>
                {
                    element = Mocks.Of<ICollection>().First(x => x.Parent == container);
                    ((IList<IAbstractElement>)container.AllElements).Add(element);
                    Mock.Get(manager).Raise(x => x.ElementCreated += null, new ValueEventArgs<IProductElement>(element));
                    Mock.Get(manager).Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IProductElement>(element));
                })
                .Returns(() => element);
        }

        internal static void SetupCreateElement<T>(IPatternManager manager, T container) where T : class, IElementContainer
        {
            IElement element = null;
            Mock.Get<T>(container)
                .Setup(v => v.CreateElement(It.IsAny<Action<IElement>>(), It.IsAny<bool>()))
                .Callback(() =>
                {
                    element = Mocks.Of<IElement>().First(x => x.Parent == container && x.InstanceName == "2second");
                    ((IList<IAbstractElement>)container.AllElements).Add(element);
                    Mock.Get(manager).Raise(x => x.ElementCreated += null, new ValueEventArgs<IProductElement>(element));
                    Mock.Get(manager).Raise(x => x.ElementInstantiated += null, new ValueEventArgs<IProductElement>(element));
                })
                .Returns(() => element);
        }
    }
}