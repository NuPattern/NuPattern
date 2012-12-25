using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Store.UnitTests
{
    public class ProductElementSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAContainerWithADisposableExtension
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private Mock<IAutomationExtension> extension;
            private ProductElement element;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                this.extension = new Mock<IAutomationExtension>();
                this.extension.As<IDisposable>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Product>();
                    tx.Commit();
                }

                this.element.AutomationExtensions.Add(this.extension.Object);
            }

            [TestCleanup]
            public void CleanUp()
            {
                if (!this.store.Store.Disposed)
                {
                    this.store.Dispose();
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenContainerDeletedFromParent_ThenDisposesExtension()
            {
                this.element.WithTransaction(c => c.Delete());

                this.extension.As<IDisposable>().Verify(x => x.Dispose());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisposingStore_ThenDisposesExtension()
            {
                this.store.Dispose();

                this.extension.As<IDisposable>().Verify(x => x.Dispose());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExtensionThrowsOnDisposing_ThenDoesNotFailToDelete()
            {
                this.extension.As<IDisposable>().Setup(x => x.Dispose()).Throws<InvalidOperationException>();

                this.element.WithTransaction(c => c.Delete());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExtensionThrowsOnDisposing_ThenDoesNotFailToDisposeStore()
            {
                this.extension.As<IDisposable>().Setup(x => x.Dispose()).Throws<InvalidOperationException>();

                this.store.Dispose();
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAnElementWithValidationBinding
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private ProductElement element;
            private Mock<IDynamicBindingContext> bindingContext;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Product>();
                    tx.Commit();
                }

                this.bindingContext = new Mock<IDynamicBindingContext> { DefaultValue = DefaultValue.Mock };

                Mock.Get(this.store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingFactory)))
                    .Returns(Mock.Of<IBindingFactory>(factory =>
                        factory.CreateContext() == this.bindingContext.Object));

                this.element.InstanceName = "Hello";
                this.element.Info = Mock.Of<IElementInfo>(info =>
                    info.Name == "My" &&
                    info.ValidationSettings == new IBindingSettings[] { Mock.Of<IBindingSettings>() });
            }

            [TestCleanup]
            public void CleanUp()
            {
                if (!this.store.Store.Disposed)
                {
                    this.store.Dispose();
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValidating_ThenBindingContextHasElement()
            {
                var controller = new ValidationController();

                controller.ValidateCustom(this.element, ValidationConstants.RuntimeValidationCategory);

                this.bindingContext.Verify(x => x.AddExport<IProductElement>(this.element));
            }
        }


        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenMultipleElementsAndCollections
        {
            private IView view;
            private IElement mainElement;
            private Dsl.Store store;
            private ProductState productStore;
            private string storeFilePath = Path.GetTempFileName();

            private static class Ids
            {
                public const string ExtensionToolkit1Id = "ToolkitExtension1";
                public const string ExtensionToolkit2Id = "ToolkitExtension2";
                public static readonly Guid ExtensionProduct1Id = Guid.NewGuid();
                public static readonly Guid ExtensionProduct2Id = Guid.NewGuid();

                public const string MainToolkitId = "MainToolkit";
                public static readonly Guid MainProductId = Guid.NewGuid();
                public static readonly Guid MainViewId = Guid.NewGuid();
                public static readonly Guid MainElementId = Guid.NewGuid();
                public static readonly Guid Collection1Id = Guid.NewGuid();
                public static readonly Guid Collection2Id = Guid.NewGuid();
                public static readonly Guid Element1Id = Guid.NewGuid();
                public static readonly Guid Element2Id = Guid.NewGuid();
                public static readonly Guid ExtensionPoint1Id = Guid.NewGuid();
                public static readonly Guid ExtensionPoint2Id = Guid.NewGuid();
                public static readonly Guid Property1Id = Guid.NewGuid();
            }

            [TestInitialize]
            [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
            public void Initialize()
            {
                var serviceProvider = Mocks.Of<IServiceProvider>().First(provider =>
                        provider.GetService(typeof(IPatternManager)) == Mocks.Of<IPatternManager>().First(manager =>
                        manager.InstalledToolkits == new IInstalledToolkitInfo[] 
						{
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.ExtensionToolkit1Id &&
								toolkitInfo.Schema.Pattern.Id == Ids.ExtensionProduct1Id &&
								toolkitInfo.Schema.Pattern.Name == "Product1" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.ExtensionToolkit1Id &&
								toolkitInfo.Schema.Pattern.OrderGroup == 7 &&
								toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
								{
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == "ExtensionPoint1Id")
								}),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.ExtensionToolkit2Id &&
								toolkitInfo.Schema.Pattern.Id == Ids.ExtensionProduct2Id &&
								toolkitInfo.Schema.Pattern.Name == "Product2" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.ExtensionToolkit2Id &&
								toolkitInfo.Schema.Pattern.OrderGroup == 8 &&
								toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
								{
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == "ExtensionPoint2Id")
								}),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.MainToolkitId &&
								toolkitInfo.Schema.Pattern.Id == Ids.MainProductId &&
								toolkitInfo.Schema.Pattern.Name == "MainProduct" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.MainToolkitId &&
								toolkitInfo.Schema.Pattern.Views == new []
								{
                                    //MainProduct
                                    //  View
                                    //      MainElement
                                    //          Collection1
                                    //              Property1
                                    //          Collection2
                                    //          Element1
                                    //          Element2
                                    //          Element3
                                    //          ExtensionPoint1
                                    //          ExtensionPoint2
									Mocks.Of<IViewInfo>().First(view => 
										view.Id == Ids.MainViewId && 
										view.Name == "View" &&
										view.Elements == new IAbstractElementInfo[]
										{
											Mocks.Of<IElementInfo>().First(element =>
												element.Id == Ids.MainElementId && 
												element.Name == "MainElement" && 
												element.Cardinality == Cardinality.ZeroToMany && 
												element.Elements == new IAbstractElementInfo[] 
												{
													Mocks.Of<ICollectionInfo>().First(ce =>
														ce.Id == Ids.Collection1Id && 
														ce.Name == "Collection1" && 
                                                        ce.OrderGroup == 1 &&
														ce.Cardinality == Cardinality.ZeroToMany &&
                                                        ce.Properties == new [] {Mocks.Of<IPropertyInfo>().First(p => 
                                                            p.Name == "Property1" && 
                                                            p.Id == Ids.Property1Id &&
                                                            p.Type == typeof(string).FullName)}
														), 
													Mocks.Of<ICollectionInfo>().First(ce =>
														ce.Id == Ids.Collection2Id && 
														ce.Name == "Collection2" && 
                                                        ce.OrderGroup == 1 &&
														ce.Cardinality == Cardinality.ZeroToMany
														), 
													Mocks.Of<IElementInfo>().First(ce =>
														ce.Id == Ids.Element1Id && 
														ce.Name == "Element1" && 
                                                        ce.OrderGroup == 2 &&
														ce.Cardinality == Cardinality.ZeroToMany
														), 
													Mocks.Of<IElementInfo>().First(ce =>
														ce.Id == Ids.Element2Id && 
														ce.Name == "Element2" && 
                                                        ce.OrderGroup == 3 &&
														ce.Cardinality == Cardinality.ZeroToMany
														), 
												} &&
												element.ExtensionPoints == new IExtensionPointInfo[] 
												{
													Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
														extensionPoint.Id == Ids.ExtensionPoint1Id && 
														extensionPoint.Name == "ExtensionPoint1" && 
                                                        extensionPoint.OrderGroup == 4 &&
														extensionPoint.Cardinality == Cardinality.ZeroToMany && 
														extensionPoint.RequiredExtensionPointId == "ExtensionPoint1Id"
														),
													Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
														extensionPoint.Id == Ids.ExtensionPoint2Id && 
														extensionPoint.Name == "ExtensionPoint2" && 
                                                        extensionPoint.OrderGroup == 5 &&
														extensionPoint.Cardinality == Cardinality.ZeroToMany && 
														extensionPoint.RequiredExtensionPointId == "ExtensionPoint2Id"
														),
												}), 
										}),
								}),
						})
                    );

                using (var store = new Dsl.Store(serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();
                    productStore
                        .CreateProduct(p => { p.ExtensionId = Ids.MainToolkitId; p.DefinitionId = Ids.MainProductId; })
                        .CreateView(v => v.DefinitionId = Ids.MainViewId)
                        .CreateElement(e => e.DefinitionId = Ids.MainElementId);

                    ProductStateStoreSerializationHelper.Instance.SaveModel(new Dsl.SerializationResult(), productStore, this.storeFilePath);
                    tx.Commit();
                }

                this.store = new Dsl.Store(serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel));
                using (var tx = this.store.TransactionManager.BeginTransaction("Loading", true))
                {
                    ProductStateStoreSerializationHelper.Instance.LoadModel(this.store, this.storeFilePath, null, null, null);
                    tx.Commit();

                    this.productStore = this.store.ElementDirectory.AllElements.OfType<ProductState>().First();
                }

                this.mainElement = this.store.ElementDirectory.FindElements<Element>(true).First();
                this.view = this.store.ElementDirectory.FindElements<View>(true).First();
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenOrderedAsInitiallyConfigured()
            {
                var collection1 = CreateCollection(Ids.Collection1Id);
                var collection2 = CreateCollection(Ids.Collection2Id);
                var element1 = CreateElement(Ids.Element1Id);
                var element2 = CreateElement(Ids.Element2Id);
                var product1 = CreateExtension(Ids.ExtensionToolkit1Id, Ids.ExtensionProduct1Id);
                var product2 = CreateExtension(Ids.ExtensionToolkit2Id, Ids.ExtensionProduct2Id);

                var elements = this.mainElement.AllElements.ToArray();

                Assert.Equal(6, elements.Count());

                Assert.True((elements[0] == collection1) && (collection1.InstanceOrder == 1.1));
                Assert.True((elements[1] == collection2) && (collection2.InstanceOrder == 1.2));

                Assert.True((elements[2] == element1) && (element1.InstanceOrder == 2.1));
                Assert.True((elements[3] == element2) && (element2.InstanceOrder == 3.1));

                Assert.True((elements[4] == product1) && (product1.InstanceOrder == 7.1));
                Assert.True((elements[5] == product2) && (product2.InstanceOrder == 8.1));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameOrder_ThenOrderedInSameGroup()
            {
                var collection1 = CreateCollection(Ids.Collection1Id);
                var collection2 = CreateCollection(Ids.Collection2Id);
                var element1 = CreateElement(Ids.Element1Id);
                var element2 = CreateElement(Ids.Element2Id);
                var product1 = CreateExtension(Ids.ExtensionToolkit1Id, Ids.ExtensionProduct1Id);
                var product2 = CreateExtension(Ids.ExtensionToolkit2Id, Ids.ExtensionProduct2Id);

                var collectionInfo1 = Mock.Get(collection1.Info);
                var collectionInfo2 = Mock.Get(collection2.Info);
                var elementInfo1 = Mock.Get(element1.Info);
                var elementInfo2 = Mock.Get(element2.Info);
                var extensionInfo1 = Mock.Get(product1.Info);
                var extensionInfo2 = Mock.Get(product2.Info);
                collectionInfo1.SetupGet(c => c.OrderGroup).Returns(5);
                collectionInfo2.SetupGet(c => c.OrderGroup).Returns(5);
                elementInfo1.SetupGet(c => c.OrderGroup).Returns(5);
                elementInfo2.SetupGet(c => c.OrderGroup).Returns(5);
                extensionInfo1.SetupGet(c => c.OrderGroup).Returns(5);
                extensionInfo2.SetupGet(c => c.OrderGroup).Returns(5);
                collectionInfo1.SetupGet(c => c.Name).Returns("z");
                collectionInfo2.SetupGet(c => c.Name).Returns("y");
                elementInfo1.SetupGet(c => c.Name).Returns("x");
                elementInfo2.SetupGet(c => c.Name).Returns("w");
                extensionInfo1.SetupGet(c => c.Name).Returns("b");
                extensionInfo2.SetupGet(c => c.Name).Returns("a");

                var elements = this.mainElement.AllElements.ToArray();

                Assert.Equal(6, elements.Count());
                Assert.True((elements[0] == element2) && (element2.InstanceOrder == 5.1));
                Assert.True((elements[1] == element1) && (element1.InstanceOrder == 5.2));
                Assert.True((elements[2] == collection2) && (collection2.InstanceOrder == 5.3));
                Assert.True((elements[3] == collection1) && (collection1.InstanceOrder == 5.4));
                Assert.True((elements[4] == product2) && (product2.InstanceOrder == 5.5));
                Assert.True((elements[5] == product1) && (product1.InstanceOrder == 5.6));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDiffOrder_ThenOrderedInDiffGroups()
            {
                var collection1 = CreateCollection(Ids.Collection1Id);
                var collection2 = CreateCollection(Ids.Collection2Id);
                var element1 = CreateElement(Ids.Element1Id);
                var element2 = CreateElement(Ids.Element2Id);
                var product1 = CreateExtension(Ids.ExtensionToolkit1Id, Ids.ExtensionProduct1Id);
                var product2 = CreateExtension(Ids.ExtensionToolkit2Id, Ids.ExtensionProduct2Id);

                var collectionInfo1 = Mock.Get(collection1.Info);
                var collectionInfo2 = Mock.Get(collection2.Info);
                var elementInfo1 = Mock.Get(element1.Info);
                var elementInfo2 = Mock.Get(element2.Info);
                var extensionInfo1 = Mock.Get(product1.Info);
                var extensionInfo2 = Mock.Get(product2.Info);
                collectionInfo1.SetupGet(c => c.OrderGroup).Returns(5);
                collectionInfo2.SetupGet(c => c.OrderGroup).Returns(5);
                elementInfo1.SetupGet(c => c.OrderGroup).Returns(7);
                elementInfo2.SetupGet(c => c.OrderGroup).Returns(7);
                extensionInfo1.SetupGet(c => c.OrderGroup).Returns(9);
                extensionInfo2.SetupGet(c => c.OrderGroup).Returns(9);
                collectionInfo1.SetupGet(c => c.Name).Returns("z");
                collectionInfo2.SetupGet(c => c.Name).Returns("y");
                elementInfo1.SetupGet(c => c.Name).Returns("x");
                elementInfo2.SetupGet(c => c.Name).Returns("w");
                extensionInfo1.SetupGet(c => c.Name).Returns("b");
                extensionInfo2.SetupGet(c => c.Name).Returns("a");

                var elements = this.mainElement.AllElements.ToArray();

                Assert.Equal(6, elements.Count());
                Assert.True((elements[0] == collection2) && (collection2.InstanceOrder == 5.1));
                Assert.True((elements[1] == collection1) && (collection1.InstanceOrder == 5.2));
                Assert.True((elements[2] == element2) && (element2.InstanceOrder == 7.1));
                Assert.True((elements[3] == element1) && (element1.InstanceOrder == 7.2));
                Assert.True((elements[4] == product2) && (product2.InstanceOrder == 9.1));
                Assert.True((elements[5] == product1) && (product1.InstanceOrder == 9.2));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameOrderAndSameDefinitionName_TheOrderedByInstanceName()
            {
                var collection1 = CreateCollection(Ids.Collection1Id, "f");
                var collection2 = CreateCollection(Ids.Collection1Id, "e");
                var collection3 = CreateCollection(Ids.Collection1Id, "d");
                var collection4 = CreateCollection(Ids.Collection1Id, "c");
                var collection5 = CreateCollection(Ids.Collection1Id, "b");
                var collection6 = CreateCollection(Ids.Collection1Id, "a");

                var collectionInfo = Mock.Get(collection1.Info);
                collectionInfo.SetupGet(c => c.OrderGroup).Returns(1);

                var elements = this.mainElement.AllElements.ToArray();

                Assert.True((elements[0] == collection6) && (collection6.InstanceOrder == 1.1));
                Assert.True((elements[1] == collection5) && (collection5.InstanceOrder == 1.2));
                Assert.True((elements[2] == collection4) && (collection4.InstanceOrder == 1.3));
                Assert.True((elements[3] == collection3) && (collection3.InstanceOrder == 1.4));
                Assert.True((elements[4] == collection2) && (collection2.InstanceOrder == 1.5));
                Assert.True((elements[5] == collection1) && (collection1.InstanceOrder == 1.6));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenComparerDefined_ThenGroupOrderedByComparer()
            {
                var collection1 = CreateCollection(Ids.Collection1Id, "f");
                SetProperty(collection1, Ids.Property1Id, "a");
                var collection2 = CreateCollection(Ids.Collection1Id, "e");
                SetProperty(collection2, Ids.Property1Id, "c");
                var collection3 = CreateCollection(Ids.Collection1Id, "d");
                SetProperty(collection3, Ids.Property1Id, "e");
                var collection4 = CreateCollection(Ids.Collection1Id, "c");
                SetProperty(collection4, Ids.Property1Id, "b");
                var collection5 = CreateCollection(Ids.Collection1Id, "b");
                SetProperty(collection5, Ids.Property1Id, "f");
                var collection6 = CreateCollection(Ids.Collection1Id, "a");
                SetProperty(collection6, Ids.Property1Id, "d");

                var collectionInfo = Mock.Get(collection1.Info);
                collectionInfo.SetupGet(c => c.OrderGroup).Returns(7);
                collectionInfo.SetupGet(c => c.OrderGroupComparerTypeName).Returns(typeof(FirstPropertyValueProductElementComparer).FullName);

                var elements = this.mainElement.AllElements.ToArray();

                Assert.True((elements[0] == collection1) && (collection1.InstanceOrder == 7.1));
                Assert.True((elements[1] == collection4) && (collection4.InstanceOrder == 7.2));
                Assert.True((elements[2] == collection2) && (collection2.InstanceOrder == 7.3));
                Assert.True((elements[3] == collection6) && (collection6.InstanceOrder == 7.4));
                Assert.True((elements[4] == collection3) && (collection3.InstanceOrder == 7.5));
                Assert.True((elements[5] == collection5) && (collection5.InstanceOrder == 7.6));
            }


            private void SetProperty(IProductElement element, Guid propertyId, object value)
            {
                element.Properties.FirstOrDefault(p => p.Info.Id == propertyId).Value = value;
            }
            private ICollection CreateCollection(Guid definitionId, string instanceName = "")
            {
                return this.mainElement.CreateCollection(e =>
                    {
                        e.DefinitionId = definitionId;
                        e.InstanceName = instanceName;
                    });
            }
            private IElement CreateElement(Guid definitionId, string instanceName = "")
            {
                return this.mainElement.CreateElement(e =>
                {
                    e.DefinitionId = definitionId;
                    e.InstanceName = instanceName;
                });
            }
            private IProduct CreateExtension(string extensionId, Guid definitionId, string instanceName = "")
            {
                return this.mainElement.CreateExtension(e =>
                {
                    e.ExtensionId = extensionId;
                    e.DefinitionId = definitionId;
                    e.InstanceName = instanceName;
                    e.ProductState = this.productStore;
                });
            }

        }
    }
}