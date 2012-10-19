using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store.UnitTests
{
    public class ProductAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAProductInfo
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private IInstalledToolkitInfo toolkit;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                this.toolkit = Mocks.Of<IInstalledToolkitInfo>().First(
                    f => f.Id == "foo" && f.Schema.Pattern.Id == Guid.NewGuid());

                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(sp => sp.GetService(typeof(IPatternManager)))
                    .Returns(Mocks.Of<IPatternManager>().First(x => x.InstalledToolkits == new[] { this.toolkit }));
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();
            }

            [TestMethod]
            public void WhenCreatingNewProductWithoutToolkitIdAndDefinitionId_ThenInfoTurnsNull()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Product>();
                    tx.Commit();

                    Assert.Null(target.Info);
                }
            }

            [TestMethod]
            public void WhenCreatingNewProductWithToolkitIdAndDefinitionId_ThenSetsSchemaInfo()
            {
                var target = this.CreateProduct();

                Assert.Equal(this.toolkit.Schema.Pattern, target.Info);
            }

            [TestMethod]
            public void WhenCreatingNewProduct_ThenAddSchemaViews()
            {
                Mock.Get(this.toolkit.Schema.Pattern)
                    .Setup(f => f.Views)
                    .Returns(Mocks.Of<IViewInfo>().Where(v => v.Id == Guid.NewGuid()).Take(5).ToArray());

                var target = this.CreateProduct();

                Assert.Equal(5, target.Views.Count);
            }

            [TestMethod]
            public void WhenCreatingNewProduct_ThenSetsDefinitionIdInAddedViews()
            {
                var viewId = Guid.NewGuid();

                Mock.Get(this.toolkit.Schema.Pattern)
                    .Setup(f => f.Views)
                    .Returns(new[] { Mocks.Of<IViewInfo>().First(v => v.Id == viewId) });

                var target = this.CreateProduct();

                Assert.Equal(viewId, target.Views.First().DefinitionId);
            }

            [TestMethod]
            public void WhenCreatingNewProduct_ThenAddSchemaDynamicProperties()
            {
                Mock.Get(this.toolkit.Schema.Pattern)
                    .Setup(f => f.Properties)
                    .Returns(Mocks.Of<IPropertyInfo>().Where(v =>
                        v.Id == Guid.NewGuid() &&
                        v.Type == "System.String" &&
                        v.Name == "Foo" + Guid.NewGuid().ToString()).Take(4).ToArray());

                var target = this.CreateProduct();

                Assert.Equal(4, target.Properties.Count());
            }

            [TestMethod]
            public void WhenCreatingNewProduct_ThenSetsDefinitionIdInAddedProperties()
            {
                var propertyId = Guid.NewGuid();
                Mock.Get(this.toolkit.Schema.Pattern)
                    .Setup(f => f.Properties)
                    .Returns(new[] { Mock.Of<IPropertyInfo>(p => p.Id == propertyId && p.Name == "Foo" && p.Type == "System.String") });

                var target = this.CreateProduct();

                Assert.Equal(propertyId, target.Properties.First().DefinitionId);
            }

            [TestMethod]
            public void WhenCreatingNewProductWithPropertiesWithDefaultValues_ThenSetsDefinitionIdInAddedProperties()
            {
                Mock.Get(this.toolkit.Schema.Pattern)
                    .Setup(f => f.Properties)
                    .Returns(new[] { Mocks.Of<IPropertyInfo>().First(p => p.Id == Guid.NewGuid() && 
						p.Name == "FooBar" &&
						p.Type == "System.String" &&
						p.DefaultValue == Mock.Of<IPropertyBindingSettings>(s => s.Value == "Foo")) });

                var target = this.CreateProduct();

                Assert.Equal("Foo", target.Properties.First().RawValue);
            }

            private Product CreateProduct()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Product>();
                    target.ExtensionId = this.toolkit.Id;
                    target.DefinitionId = this.toolkit.Schema.Pattern.Id;
                    tx.Commit();
                    return target;
                }
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenASerializedProductStore
        {
            private IPatternInfo productInfo;
            private Guid productId;
            private string storeFilePath;
            private Dsl.Store store;

            [TestInitialize]
            public void Initialize()
            {
                this.storeFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

                var toolkit = Mocks.Of<IInstalledToolkitInfo>().First(
                    x => x.Id == "test_toolkit" && x.Schema.Pattern.Id == Guid.NewGuid());
                this.productInfo = toolkit.Schema.Pattern;

                var productMock = Mock.Get(this.productInfo);
                productMock.Setup(x => x.Views)
                    .Returns(Mocks.Of<IViewInfo>(x => x.Id == Guid.NewGuid() && x.Pattern == this.productInfo).Take(2).ToList());
                productMock.Setup(x => x.Properties)
                    .Returns(Mocks.Of<IPropertyInfo>(x =>
                        x.Id == Guid.NewGuid() &&
                        x.Name == "Foo" + Guid.NewGuid() &&
                        x.Type == "System.String" &&
                        x.Parent == this.productInfo).Take(2).ToList());

                var serviceProvider = new Mock<IServiceProvider>();
                serviceProvider.Setup(x => x.GetService(typeof(IPatternManager)))
                    .Returns(Mocks.Of<IPatternManager>().First(x => x.InstalledToolkits == new[] { toolkit }));

                using (var store = new Dsl.Store(serviceProvider.Object, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();

                    var product = productStore
                        .CreateProduct(x => { x.ExtensionId = "test_toolkit"; x.DefinitionId = toolkit.Schema.Pattern.Id; });

                    this.productId = product.Id;

                    ProductStateStoreSerializationHelper.Instance.SaveModel(new Dsl.SerializationResult(), productStore, this.storeFilePath);
                    tx.Commit();
                }

                this.store = new Dsl.Store(serviceProvider.Object, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel));
            }

            [TestMethod]
            public void WhenDeserializingWithNewViewsInSchema_ThenAddNewViewsToStore()
            {
                var viewIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

                var infos = (ICollection<IViewInfo>)this.productInfo.Views;
                infos.Add(Mocks.Of<IViewInfo>().First(e => e.Id == viewIds[0]));
                infos.Add(Mocks.Of<IViewInfo>().First(e => e.Id == viewIds[1]));

                var target = this.DeserializeProduct();

                Assert.Equal(4, target.Views.Count);
                Assert.True(target.Views.Any(x => x.DefinitionId == viewIds[0]));
                Assert.True(target.Views.Any(x => x.DefinitionId == viewIds[1]));
            }

            [TestMethod]
            public void WhenDeserializingWithARemovedView_ThenRemovesViewFromStore()
            {
                var infos = (IList<IViewInfo>)this.productInfo.Views;
                var deletedId = infos[0].Id;
                infos.RemoveAt(0);

                var target = this.DeserializeProduct();

                Assert.Equal(1, target.Views.Count);
                Assert.False(target.Views.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod]
            public void WhenDeserializingWithNewProperties_ThenAddNewPropertiesToStore()
            {
                var propertyIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

                var infos = (ICollection<IPropertyInfo>)this.productInfo.Properties;
                infos.Add(Mock.Of<IPropertyInfo>(e => e.Id == propertyIds[0] && e.Name == "Foo0" && e.Type == "System.String"));
                infos.Add(Mock.Of<IPropertyInfo>(e => e.Id == propertyIds[1] && e.Name == "Foo1" && e.Type == "System.String"));

                var target = this.DeserializeProduct();

                Assert.Equal(4, target.Properties.Count);
                Assert.True(target.Properties.Any(x => x.DefinitionId == propertyIds[0]));
                Assert.True(target.Properties.Any(x => x.DefinitionId == propertyIds[1]));
            }

            [TestMethod]
            public void WhenDeserializingWithNewPropertiesWithDefaultValues_ThenAddNewPropertiesWithValueToStore()
            {
                var propertyId = Guid.NewGuid();

                var infos = (ICollection<IPropertyInfo>)this.productInfo.Properties;
                infos.Add(Mocks.Of<IPropertyInfo>().First(p => p.Id == propertyId &&
                    p.Name == "FooBar" &&
                    p.Type == "System.String" &&
                    p.DefaultValue == Mock.Of<IPropertyBindingSettings>(s => s.Value == "Bar")));

                var target = this.DeserializeProduct();

                Assert.True(target.Properties.Any(p => p.DefinitionId == propertyId && p.RawValue == "Bar"));
            }

            [TestMethod]
            public void WhenDeserializingWithARemovedProperty_ThenRemovesPropertyFromStore()
            {
                var infos = (IList<IPropertyInfo>)this.productInfo.Properties;
                var deletedId = infos[0].Id;
                infos.RemoveAt(0);

                var target = this.DeserializeProduct();

                Assert.Equal(1, target.Properties.Count);
                Assert.False(target.Properties.Any(x => x.DefinitionId == deletedId));
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();

                if (File.Exists(this.storeFilePath))
                {
                    File.Delete(this.storeFilePath);
                }
            }

            private Product DeserializeProduct()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction("Loading", true))
                {
                    ProductStateStoreSerializationHelper.Instance.LoadModel(this.store, this.storeFilePath, null, null, null);
                    tx.Commit();
                }

                return (Product)this.store.ElementDirectory.FindElement(this.productId);
            }
        }

        [TestClass]
        public class GivenMultipleProducts
        {
            private Dsl.Store store;
            private ProductState productStore;
            private IView view;
            private string storeFilePath = Path.GetTempFileName();
            private IProduct rootProduct1;
            private IProduct rootProduct2;
            private IProduct rootProduct3;
            private IProduct extensionProduct1;
            private IProduct extensionProduct2;
            private IProduct extensionProduct4;
            private IExtensionPointInfo extensionPointInfo2;


            private static class Ids
            {
                public const string PatternToolkit1Id = "PatternToolkit1";
                public const string PatternToolkit2Id = "PatternToolkit2";
                public const string PatternToolkit3Id = "PatternToolkit3";
                public static readonly Guid ExtensionToolkitPatternSchema1Id = Guid.NewGuid();
                public static readonly Guid ExtensionToolkitPatternSchema2Id = Guid.NewGuid();

                public static readonly string ExtensionPointSchemaPath1 = Guid.NewGuid().ToString();
                public static readonly string ExtensionPointSchemaPath2 = Guid.NewGuid().ToString();

                public const string MainToolkitId = "MainToolkit";
                public const string SecondToolkitId = "SecondToolkit";
                public static readonly Guid MainPatternSchemaId = Guid.NewGuid();
                public static readonly Guid SecondPatternSchemaId = Guid.NewGuid();
                public static readonly Guid MainViewSchemaId = Guid.NewGuid();
                public static readonly Guid ExtensionPointSchema1Id = Guid.NewGuid();
                public static readonly Guid ExtensionPointSchema2Id = Guid.NewGuid();
            }

            [TestInitialize]
            public void InitializeContext()
            {
                var serviceProvider = Mocks.Of<IServiceProvider>().First(provider =>
                                        provider.GetService(typeof(IPatternManager)) == Mocks.Of<IPatternManager>().First(manager =>
                                            manager.InstalledToolkits == new IInstalledToolkitInfo[] 
						{
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.PatternToolkit1Id &&
								toolkitInfo.Schema.Pattern.Id == Ids.ExtensionToolkitPatternSchema1Id &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.PatternToolkit1Id &&
								toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
								{
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == Ids.ExtensionPointSchemaPath1),
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == Ids.ExtensionPointSchemaPath2)
								}),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.PatternToolkit2Id &&
								toolkitInfo.Schema.Pattern.Id == Ids.ExtensionToolkitPatternSchema2Id &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.PatternToolkit2Id &&
								toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
								{
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == Ids.ExtensionPointSchemaPath2)
								}),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.SecondToolkitId &&
								toolkitInfo.Schema.Pattern.Id == Ids.SecondPatternSchemaId &&
								toolkitInfo.Schema.Pattern.Name == "SecondPatternSchema" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.MainToolkitId),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.MainToolkitId &&
								toolkitInfo.Schema.Pattern.Id == Ids.MainPatternSchemaId &&
								toolkitInfo.Schema.Pattern.Name == "MainPatternSchema" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.MainToolkitId &&
                                toolkitInfo.Schema.Pattern.Views == new []
								{
                                    //  View
                                    //          ExtensionPoint1
                                    //          ExtensionPoint2
									Mocks.Of<IViewInfo>().First(view => 
										view.Id == Ids.MainViewSchemaId && 
										view.Name == "View" &&
										view.ExtensionPoints == new IExtensionPointInfo[] 
										{
											Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
												extensionPoint.Id == Ids.ExtensionPointSchema1Id && 
												extensionPoint.Name == "ExtensionPointSchema1" && 
                                                extensionPoint.OrderGroup == 2 &&
												extensionPoint.Cardinality == Cardinality.ZeroToMany && 
												extensionPoint.RequiredExtensionPointId == Ids.ExtensionPointSchemaPath1
												),
											Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
												extensionPoint.Id == Ids.ExtensionPointSchema2Id && 
												extensionPoint.Name == "ExtensionPointSchema2" && 
                                                extensionPoint.OrderGroup == 3 &&
												extensionPoint.Cardinality == Cardinality.ZeroToOne && 
												extensionPoint.RequiredExtensionPointId == Ids.ExtensionPointSchemaPath2
												),
										}), 
								}),
						})
                    );

                using (var store = new Dsl.Store(serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();
                    productStore
                        .CreateProduct(p => { p.ExtensionId = Ids.SecondToolkitId; p.DefinitionId = Ids.SecondPatternSchemaId; });
                    productStore
                        .CreateProduct(p => { p.ExtensionId = Ids.SecondToolkitId; p.DefinitionId = Ids.SecondPatternSchemaId; });
                    productStore
                        .CreateProduct(p => { p.ExtensionId = Ids.MainToolkitId; p.DefinitionId = Ids.MainPatternSchemaId; })
                        .CreateView(v => v.DefinitionId = Ids.MainViewSchemaId);

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

                this.rootProduct3 = this.store.ElementDirectory.FindElements<Product>(true).Where(p => p.DefinitionId == Ids.SecondPatternSchemaId).ToArray()[0];
                this.rootProduct2 = this.store.ElementDirectory.FindElements<Product>(true).Where(p => p.DefinitionId == Ids.SecondPatternSchemaId).ToArray()[1];
                this.rootProduct1 = this.store.ElementDirectory.FindElements<Product>(true).First(p => p.DefinitionId == Ids.MainPatternSchemaId);
                this.view = rootProduct1.Views.First();

                this.extensionPointInfo2 = this.view.Info.ExtensionPoints.First(ep => ep.Id == Ids.ExtensionPointSchema2Id);

                this.extensionProduct1 = this.view.CreateExtension(prod => { prod.ProductState = this.productStore; prod.ExtensionId = Ids.PatternToolkit1Id; prod.DefinitionId = Ids.ExtensionToolkitPatternSchema1Id; });
                this.extensionProduct2 = this.view.CreateExtension(prod => { prod.ProductState = this.productStore; prod.ExtensionId = Ids.PatternToolkit1Id; prod.DefinitionId = Ids.ExtensionToolkitPatternSchema1Id; });
                this.extensionProduct4 = this.view.CreateExtension(prod => { prod.ProductState = this.productStore; prod.ExtensionId = Ids.PatternToolkit2Id; prod.DefinitionId = Ids.ExtensionToolkitPatternSchema2Id; });
            }

            [TestMethod]
            public void ThenRootProductsFromDiffToolkitHaveDifferentInfo()
            {
                Assert.NotEqual(rootProduct1.Info, rootProduct2.Info);
            }

            [TestMethod]
            public void ThenRootProductsFromSameToolkitHaveSameInfo()
            {
                Assert.Equal(rootProduct2.Info, rootProduct3.Info);
            }

            [TestMethod]
            public void ThenExtendedProductsFromSameToolkitAndSameExtensionPointHaveSameInfo()
            {
                Assert.Equal(extensionProduct1.Info, extensionProduct2.Info);
            }

            [TestMethod]
            public void ThenExtendedProductsFromDiffToolkitAndDiffExtensionPointsHaveDiffInfo()
            {
                Assert.NotEqual(extensionProduct1.Info, extensionProduct4.Info);
            }
        }
    }
}