using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Modeling;
using NuPattern.Runtime.Store;

namespace NuPattern.Runtime.UnitTests.Rules
{
    public class ViewAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAViewInfo
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private Product product;
            private Mock<IViewInfo> viewInfo;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                var productInfo = new Mock<IPatternInfo>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.product = this.store.ElementFactory.CreateElement<Product>();
                    this.product.Info = productInfo.Object;
                    tx.Commit();
                }

                this.viewInfo = new Mock<IViewInfo>();
                this.viewInfo.Setup(v => v.Id).Returns(Guid.NewGuid());
                this.viewInfo.Setup(v => v.Pattern).Returns(this.product.Info);

                productInfo.Setup(p => p.Views).Returns(new[] { this.viewInfo.Object });
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewViewWithoutDefinitionId_ThenInfoTurnsNull()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<View>();
                    tx.Commit();

                    Assert.Null(target.Info);
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewViewWithDefinitionId_ThenSetsSchemaInfo()
            {
                var target = this.CreateView();

                Assert.Equal(this.viewInfo.Object, target.Info);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewView_ThenSetProperties()
            {
                this.viewInfo.Setup(v => v.Name).Returns("bar");
                this.viewInfo.Setup(v => v.DisplayName).Returns("bar display");

                var target = this.CreateView();

                Assert.Equal(this.viewInfo.Object.Name, target.Info.Name);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewView_ThenAddSchemaElementsWithOneToOneCardinalityAndAutoCreateOption()
            {
                this.viewInfo.Setup(v => v.Elements)
                    .Returns(new IAbstractElementInfo[]
                    {
                        Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
                        Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
                        Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
                        Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
                    });

                var target = this.CreateView();

                Assert.Equal(2, target.Elements.Count());
            }


            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewView_ThenDoesntAddSchemaElementsWithOneToOneCardinalityAutomatically()
            {
                this.viewInfo.Setup(v => v.Elements)
                    .Returns(new IAbstractElementInfo[]
                    {
                        Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
                        Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
                        Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
                        Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
                    });

                var target = this.CreateView();

                Assert.Equal(0, target.Elements.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewView_ThenSetsDefinitionIdInAddedElements()
            {
                var elementId = Guid.NewGuid();
                var collectionId = Guid.NewGuid();

                this.viewInfo.Setup(v => v.Elements)
                    .Returns(new IAbstractElementInfo[]
                {
                    Mocks.Of<IElementInfo>().First(e => e.Id == elementId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
                    Mocks.Of<ICollectionInfo>().First(e => e.Id == collectionId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true)
                });

                var target = this.CreateView();

                Assert.True(target.Elements.Any(e => e.DefinitionId == elementId));
                Assert.True(target.Elements.Any(e => e.DefinitionId == collectionId));
            }

            private View CreateView()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<View>();
                    target.DefinitionId = this.viewInfo.Object.Id;
                    target.Product = this.product;
                    tx.Commit();
                    return target;
                }
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenASerializedProductStore
        {
            private IViewInfo viewInfo;
            private Guid viewId;
            private string storeFilePath;
            private Microsoft.VisualStudio.Modeling.Store store;

            [TestInitialize]
            [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
            public void Initialize()
            {
                this.storeFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

                var viewInfoMock = new Mock<IViewInfo>();
                viewInfoMock.Setup(x => x.Id).Returns(Guid.NewGuid());
                viewInfoMock.Setup(e => e.Elements)
                    .Returns(new List<IAbstractElementInfo>
                    {
                        Mocks.Of<IElementInfo>().First(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
                        Mocks.Of<ICollectionInfo>().First(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
                        Mocks.Of<IElementInfo>().First(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.ZeroToMany)
                    });
                viewInfoMock.Setup(x => x.ExtensionPoints)
                    .Returns(new List<IExtensionPointInfo>
                    {
                        Mocks.Of<IExtensionPointInfo>().First(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true && e.RequiredExtensionPointId == "ext_1"),
                        Mocks.Of<IExtensionPointInfo>().First(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.ZeroToMany && e.RequiredExtensionPointId == "ext_2"),
                    });

                this.viewInfo = viewInfoMock.Object;

                var providedExtension1 = Mocks.Of<IProvidedExtensionPointInfo>().First(x => x.ExtensionPointId == "ext_1");
                var extensionPoint1 = Mocks.Of<IInstalledToolkitInfo>().First(x =>
                    x.Id == "ext_toolkit_1" &&
                    x.Schema.Pattern.Id == Guid.NewGuid() &&
                    x.Schema.Pattern.ExtensionId == "ext_toolkit_1" &&
                    x.Schema.Pattern.ProvidedExtensionPoints == new[] { providedExtension1 });

                var providedExtension2 = Mocks.Of<IProvidedExtensionPointInfo>().First(x => x.ExtensionPointId == "ext_2");
                var extensionPoint2 = Mocks.Of<IInstalledToolkitInfo>().First(x =>
                    x.Id == "ext_toolkit_2" &&
                    x.Schema.Pattern.Id == Guid.NewGuid() &&
                    x.Schema.Pattern.ExtensionId == "ext_toolkit_2" &&
                    x.Schema.Pattern.ProvidedExtensionPoints == new[] { providedExtension2 });

                var toolkit = Mocks.Of<IInstalledToolkitInfo>().First(x =>
                    x.Id == "test_toolkit" &&
                    x.Schema.Pattern.Id == Guid.NewGuid() &&
                    x.Schema.Pattern.Views == new[] { viewInfo } &&
                    x.Schema.Pattern.ProvidedExtensionPoints == new[] { providedExtension1, providedExtension2 });

                var patternManager = new Mock<IPatternManager>();
                patternManager.Setup(x => x.InstalledToolkits)
                    .Returns(new[] { toolkit, extensionPoint1, extensionPoint2 });

                var serviceProvider = new Mock<IServiceProvider>();
                serviceProvider.Setup(x => x.GetService(typeof(IPatternManager)))
                    .Returns(patternManager.Object);

                using (var store = new Microsoft.VisualStudio.Modeling.Store(serviceProvider.Object, typeof(Microsoft.VisualStudio.Modeling.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();

                    var view = productStore
                        .CreateProduct(x => { x.ExtensionId = "test_toolkit"; x.DefinitionId = toolkit.Schema.Pattern.Id; })
                        .CreateView(x => x.DefinitionId = this.viewInfo.Id);

                    this.viewId = view.Id;

                    view.CreateElement(x => x.DefinitionId = this.viewInfo.Elements.ElementAt(2).Id);
                    view.CreateElement(x => x.DefinitionId = this.viewInfo.Elements.ElementAt(2).Id);

                    view.CreateExtension(x => { x.DefinitionId = extensionPoint1.Schema.Pattern.Id; x.ExtensionId = extensionPoint1.Schema.Pattern.ExtensionId; });
                    view.CreateExtension(x => { x.DefinitionId = extensionPoint2.Schema.Pattern.Id; x.ExtensionId = extensionPoint2.Schema.Pattern.ExtensionId; });
                    view.CreateExtension(x => { x.DefinitionId = extensionPoint2.Schema.Pattern.Id; x.ExtensionId = extensionPoint2.Schema.Pattern.ExtensionId; });

                    ProductStateStoreSerializationHelper.Instance.SaveModel(new Microsoft.VisualStudio.Modeling.SerializationResult(), productStore, this.storeFilePath);
                    tx.Commit();
                }

                this.store = new Microsoft.VisualStudio.Modeling.Store(serviceProvider.Object, typeof(Microsoft.VisualStudio.Modeling.CoreDomainModel), typeof(ProductStateStoreDomainModel));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithNewOneToOneCardinalityElementsInSchema_ThenAddNewElementsToStore()
            {
                var singleIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();
                var mutipleIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

                var infos = (ICollection<IAbstractElementInfo>)this.viewInfo.Elements;
                infos.Add(Mocks.Of<IElementInfo>().First(e => e.Id == singleIds[0] && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true));
                infos.Add(Mocks.Of<IElementInfo>().First(e => e.Id == mutipleIds[0] && e.Cardinality == Cardinality.ZeroToMany));
                infos.Add(Mocks.Of<ICollectionInfo>().First(e => e.Id == singleIds[1] && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true));
                infos.Add(Mocks.Of<ICollectionInfo>().First(e => e.Id == mutipleIds[1] && e.Cardinality == Cardinality.ZeroToMany));

                var target = this.DeserializeView();

                Assert.Equal(6, target.Elements.Count);
                Assert.True(target.Elements.Any(x => x.DefinitionId == singleIds[0]));
                Assert.True(target.Elements.Any(x => x.DefinitionId == singleIds[1]));
                Assert.False(target.Elements.Any(x => x.DefinitionId == mutipleIds[0]));
                Assert.False(target.Elements.Any(x => x.DefinitionId == mutipleIds[1]));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithDeletedElementsInSchema_ThenDeleteElementsFromStore()
            {
                var infos = (IList<IAbstractElementInfo>)this.viewInfo.Elements;
                var deletedId = infos[0].Id;
                infos.RemoveAt(0);

                var target = this.DeserializeView();

                Assert.Equal(3, target.Elements.Count);
                Assert.False(target.Elements.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithDeletedZeroToManyCardinalityElementsInSchema_ThenDeleteElementsFromStore()
            {
                var infos = (IList<IAbstractElementInfo>)this.viewInfo.Elements;
                var info = infos.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var deletedId = info.Id;
                infos.Remove(info);

                var target = this.DeserializeView();

                Assert.Equal(2, target.Elements.Count);
                Assert.False(target.Elements.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithDeletedExtensionPointInSchema_ThenDeleteExtensionsPointsFromStore()
            {
                var infos = (IList<IExtensionPointInfo>)this.viewInfo.ExtensionPoints;
                var deletedId = infos[1].Id;
                infos.RemoveAt(1);

                var target = this.DeserializeView();

                Assert.Equal(1, target.ExtensionProducts.Count);
                Assert.False(target.ExtensionProducts.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithElementMutiplicityChangedFromMultipleToSingle_ThenDeletesAnyExtraElement()
            {
                var info = this.viewInfo.Elements.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var infoMock = Mock.Get((IElementInfo)info);
                infoMock.Setup(x => x.Cardinality).Returns(Cardinality.OneToOne);

                var target = this.DeserializeView();

                Assert.Equal(3, target.Elements.Count);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeserializingWithExtensionPointMutiplicityChangedFromMultipleToSingle_ThenDeletesAnyExtraExtensionPoint()
            {
                var info = this.viewInfo.ExtensionPoints.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var infoMock = Mock.Get(info);
                infoMock.Setup(x => x.Cardinality).Returns(Cardinality.OneToOne);

                var target = this.DeserializeView();

                Assert.Equal(2, target.ExtensionProducts.Count);
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

            private View DeserializeView()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction("Loading", true))
                {
                    ProductStateStoreSerializationHelper.Instance.LoadModel(this.store, this.storeFilePath, null, null, null);
                    tx.Commit();
                }

                return (View)this.store.ElementDirectory.FindElement(this.viewId);
            }
        }
    }
}