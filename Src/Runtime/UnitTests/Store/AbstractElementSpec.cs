using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Runtime.Store;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.UnitTests.Store
{
    public class AbstractElementSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAnExtensionPoint
        {
            private IView view;
            private IElement element;
            private Dsl.Store store;
            private ProductState productStore;
            private string storeFilePath = Path.GetTempFileName();

            private static class Ids
            {
                public const string ValidExtensionToolkitId = "ValidToolkitExtension";
                public static readonly Guid ValidExtensionProductId = Guid.NewGuid();
                public const string InvalidExtensionToolkitId = "InvalidToolkitExtension";
                public static readonly Guid InvalidExtensionProductId = Guid.NewGuid();

                public const string MainToolkitId = "MainToolkit";
                public static readonly Guid MainProductId = Guid.NewGuid();
                public static readonly Guid MainViewId = Guid.NewGuid();
                public static readonly Guid MainElementId = Guid.NewGuid();
                public static readonly Guid MainCollectionId = Guid.NewGuid();
                public static readonly Guid MainExtensionPointId = Guid.NewGuid();
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
                                toolkitInfo.Id == Ids.ValidExtensionToolkitId &&
                                toolkitInfo.Schema.Pattern.Id == Ids.ValidExtensionProductId &&
                                toolkitInfo.Schema.Pattern.ExtensionId == Ids.ValidExtensionToolkitId &&
                                toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
                                {
                                    Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == "ValidExtensionPointId")
                                }),
                            Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
                                toolkitInfo.Id == Ids.InvalidExtensionToolkitId &&
                                toolkitInfo.Schema.Pattern.Id == Ids.InvalidExtensionProductId &&
                                toolkitInfo.Schema.Pattern.ExtensionId == Ids.InvalidExtensionToolkitId &&
                                toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
                                {
                                    Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == "InvalidExtensionPointId")
                                }),
                            Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
                                toolkitInfo.Id == Ids.MainToolkitId &&
                                toolkitInfo.Schema.Pattern.Id == Ids.MainProductId &&
                                toolkitInfo.Schema.Pattern.Name == "Pattern" &&
                                toolkitInfo.Schema.Pattern.ExtensionId == Ids.MainToolkitId &&
                                toolkitInfo.Schema.Pattern.Views == new []
                                {
                                    //  View
                                    //      MainElement
                                    //          MainCollection
                                    //          MainExtensionPoint
                                    Mocks.Of<IViewInfo>().First(view => 
                                        view.Id == Ids.MainViewId && 
                                        view.Name == "View" &&
                                        view.Elements == new IAbstractElementInfo[]
                                        {
                                            Mocks.Of<IElementInfo>().First(element =>
                                                element.Id == Ids.MainElementId && 
                                                element.Name == "Element" && 
                                                element.Cardinality == Cardinality.ZeroToMany && 
                                                element.Elements == new IAbstractElementInfo[] 
                                                {
                                                    Mocks.Of<ICollectionInfo>().First(collection =>
                                                        collection.Id == Ids.MainCollectionId && 
                                                        collection.Name == "Collection" && 
                                                        collection.Cardinality == Cardinality.ZeroToMany
                                                        ), 
                                                } &&
                                                element.ExtensionPoints == new IExtensionPointInfo[] 
                                                {
                                                    Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
                                                        extensionPoint.Id == Ids.MainExtensionPointId && 
                                                        extensionPoint.Name == "ExtensionPoint" && 
                                                        extensionPoint.Cardinality == Cardinality.ZeroToMany && 
                                                        extensionPoint.RequiredExtensionPointId == "ValidExtensionPointId"
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

                this.element = this.store.ElementDirectory.FindElements<Element>(true).First();
                this.view = this.store.ElementDirectory.FindElements<View>(true).First();
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingValidExtension_ThenAddsItToExtensionsCollection()
            {
                var extension = this.element.CreateExtension(prod => { prod.ProductState = this.productStore; prod.ExtensionId = Ids.ValidExtensionToolkitId; prod.DefinitionId = Ids.ValidExtensionProductId; });

                Assert.NotNull(extension);
                Assert.Equal(1, this.element.Extensions.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingInvalidExtension_ThenThrows()
            {
                Assert.Throws<ArgumentException>(() => this.element
                    .CreateExtension(prod => { prod.ProductState = this.productStore; prod.ExtensionId = Ids.InvalidExtensionToolkitId; prod.DefinitionId = Ids.InvalidExtensionProductId; }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingChildCollection_ThenAddsItToElementsCollection()
            {
                var existing = this.element.Elements.Count();
                var collection = this.element.CreateCollection(e => e.DefinitionId = Ids.MainCollectionId);

                Assert.NotNull(collection);
                Assert.Equal(existing + 1, this.element.Elements.Count());
            }
        }
    }
}
