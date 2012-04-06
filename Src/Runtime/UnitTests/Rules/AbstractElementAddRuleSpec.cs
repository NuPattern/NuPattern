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
    public class AbstractElementAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenAnElementInfo
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private Mock<IElementInfo> elementInfo;
            private View view;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                var viewInfo = new Mock<IViewInfo>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.view = this.store.ElementFactory.CreateElement<View>();
                    this.view.Info = viewInfo.Object;
                    tx.Commit();
                }

                this.elementInfo = new Mock<IElementInfo>();
                this.elementInfo.Setup(e => e.Id).Returns(Guid.NewGuid());
                this.elementInfo.Setup(e => e.Parent).Returns(this.view.Info);

                viewInfo.Setup(p => p.Elements).Returns(new[] { this.elementInfo.Object });
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();
            }

            [TestMethod]
            public void WhenCreatingNewElementWithoutDefinitionId_ThenInfoTurnsNull()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Element>();
                    tx.Commit();

                    Assert.Null(target.Info);
                }
            }

            [TestMethod]
            public void WhenCreatingNewElementWithDefinitionId_ThenSetsSchemaInfo()
            {
                var target = this.CreateElement();

                Assert.Equal(this.elementInfo.Object, target.Info);
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenSetProperties()
            {
                this.elementInfo.Setup(v => v.Name).Returns("bar");
                this.elementInfo.Setup(v => v.DisplayName).Returns("bar");

                var target = this.CreateElement();

                Assert.Equal(this.elementInfo.Object.Name, target.Info.Name);
                Assert.Equal(this.elementInfo.Object.Name, target.InstanceName);
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenAddSchemaElementsWithOneToOneCardinalityAndAutoCreateOption()
            {
                this.elementInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
					{
						Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
						Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
						Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
						Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
					});

                var target = this.CreateElement();

                Assert.Equal(2, target.Elements.Count());
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenDoesntAddSchemaElementsWithOneToOneCardinalityAutomatically()
            {
                this.elementInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
					{
						Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
						Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
						Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
						Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
					});

                var target = this.CreateElement();

                Assert.Equal(0, target.Elements.Count());
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenSetsDefinitionIdInAddedElements()
            {
                var elementId = Guid.NewGuid();
                var collectionId = Guid.NewGuid();

                this.elementInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
					{
						Mocks.Of<IElementInfo>().First(e => e.Id == elementId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
						Mocks.Of<ICollectionInfo>().First(e => e.Id == collectionId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true)
					});

                var target = this.CreateElement();

                Assert.True(target.Elements.Any(e => e.DefinitionId == elementId));
                Assert.True(target.Elements.Any(e => e.DefinitionId == collectionId));
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenAddSchemaDynamicProperties()
            {
                this.elementInfo.Setup(e => e.Properties)
                    .Returns(Mocks.Of<IPropertyInfo>().Where(v =>
                        v.Id == Guid.NewGuid() &&
                        v.Type == "System.String" &&
                        v.Name == "Foo" + Guid.NewGuid().ToString()).Take(4).ToArray());

                var target = this.CreateElement();

                Assert.Equal(4, target.Properties.Count());
            }

            [TestMethod]
            public void WhenCreatingNewElement_ThenSetsDefinitionIdInAddedProperties()
            {
                var propertyId = Guid.NewGuid();
                this.elementInfo.Setup(e => e.Properties)
                    .Returns(new[] { Mock.Of<IPropertyInfo>(p => p.Id == propertyId && p.Name == "Foo" && p.Type == "System.String") });

                var target = this.CreateElement();

                Assert.Equal(propertyId, target.Properties.First().DefinitionId);
            }

            [TestMethod]
            public void WhenCreatingNewElementWithPropertiesWithDefaultValues_ThenSetsDefinitionIdInAddedProperties()
            {
                this.elementInfo.Setup(e => e.Properties)
                    .Returns(new[] { Mocks.Of<IPropertyInfo>().First(p => p.Id == Guid.NewGuid() && 
						p.Name == "FooBar" &&
						p.Type == "System.String" &&
						p.DefaultValue == Mock.Of<IPropertyBindingSettings>(s => s.Value == "Foo") )});

                var target = this.CreateElement();

                Assert.Equal("Foo", target.Properties.First().RawValue);
            }

            private Element CreateElement()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Element>();
                    target.DefinitionId = this.elementInfo.Object.Id;
                    target.View = this.view;
                    tx.Commit();
                    return target;
                }
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenACollectionInfo
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private Mock<ICollectionInfo> collectionInfo;
            private View view;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                var viewInfo = new Mock<IViewInfo>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.view = this.store.ElementFactory.CreateElement<View>();
                    this.view.Info = viewInfo.Object;
                    tx.Commit();
                }

                this.collectionInfo = new Mock<ICollectionInfo>();
                this.collectionInfo.Setup(c => c.Id).Returns(Guid.NewGuid());
                this.collectionInfo.Setup(c => c.Parent).Returns(this.view.Info);

                viewInfo.Setup(p => p.Elements).Returns(new[] { this.collectionInfo.Object });
            }

            [TestCleanup]
            public void CleanUp()
            {
                this.store.Dispose();
            }

            [TestMethod]
            public void WhenCreatingNewCollectionWithoutDefinitionId_ThenInfoTurnsNull()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Collection>();
                    tx.Commit();

                    Assert.Null(target.Info);
                }
            }

            [TestMethod]
            public void WhenCreatingNewCollectionWithDefinitionId_ThenSetsSchemaInfo()
            {
                var target = this.CreateCollection();

                Assert.Equal(this.collectionInfo.Object, target.Info);
            }

            [TestMethod]
            public void WhenCreatingNewCollection_ThenSetProperties()
            {
                this.collectionInfo.Setup(v => v.Name).Returns("bar");
                this.collectionInfo.Setup(v => v.DisplayName).Returns("bar");

                var target = this.CreateCollection();

                Assert.Equal(this.collectionInfo.Object.Name, target.Info.DisplayName);
                Assert.Equal(this.collectionInfo.Object.Name, target.InstanceName);
            }

            [TestMethod]
            public void WhenCreatingNewCollection_ThenAddSchemaElementsWithOneToOneCardinalityAndAutoCreateOption()
            {
                this.collectionInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
				{
					Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
					Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
					Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
					Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
				});

                var target = this.CreateCollection();

                Assert.Equal(2, target.Elements.Count());
            }


            [TestMethod]
            public void WhenCreatingNewCollection_ThenDoesntAddSchemaElementsWithOneToOneCardinalityAutomatically()
            {
                this.collectionInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
				{
					Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
					Mocks.Of<IElementInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany),
					Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.OneToOne),
					Mocks.Of<ICollectionInfo>().First(e => e.Cardinality == Cardinality.ZeroToMany)
				});

                var target = this.CreateCollection();

                Assert.Equal(0, target.Elements.Count());
            }

            [TestMethod]
            public void WhenCreatingNewCollection_ThenSetsDefinitionIdInAddedElements()
            {
                var elementId = Guid.NewGuid();
                var collectionId = Guid.NewGuid();

                this.collectionInfo.Setup(e => e.Elements)
                    .Returns(new IAbstractElementInfo[]
				{
					Mocks.Of<IElementInfo>().First(e => e.Id == elementId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
					Mocks.Of<ICollectionInfo>().First(e => e.Id == collectionId && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true)
				});

                var target = this.CreateCollection();

                Assert.True(target.Elements.Any(e => e.DefinitionId == elementId));
                Assert.True(target.Elements.Any(e => e.DefinitionId == collectionId));
            }

            [TestMethod]
            public void WhenCreatingNewCollection_ThenAddSchemaDynamicProperties()
            {
                this.collectionInfo.Setup(e => e.Properties)
                    .Returns(Mocks.Of<IPropertyInfo>().Where(v =>
                        v.Id == Guid.NewGuid() &&
                        v.Type == "System.String" &&
                        v.Name == "Foo" + Guid.NewGuid().ToString()).Take(4).ToArray());

                var target = this.CreateCollection();

                Assert.Equal(4, target.Properties.Count());
            }

            [TestMethod]
            public void WhenCreatingNewCollection_ThenSetsDefinitionIdInAddedProperties()
            {
                var propertyId = Guid.NewGuid();
                this.collectionInfo.Setup(e => e.Properties)
                    .Returns(new[] { Mock.Of<IPropertyInfo>(p => p.Id == propertyId && p.Name == "Foo" && p.Type == "System.String") });

                var target = this.CreateCollection();

                Assert.Equal(propertyId, target.Properties.First().DefinitionId);
            }

            private Collection CreateCollection()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    var target = this.store.ElementFactory.CreateElement<Collection>();
                    target.DefinitionId = this.collectionInfo.Object.Id;
                    target.View = this.view;
                    tx.Commit();
                    return target;
                }
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
        public class GivenASerializedProductStore
        {
            private IElementInfo elementInfo;
            private Guid elementId;
            private string storeFilePath;
            private Dsl.Store store;

            [TestInitialize]
            [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
            public void Initialize()
            {
                this.storeFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());

                var viewInfo = Mocks.Of<IViewInfo>().First(x => x.Id == Guid.NewGuid());

                this.elementInfo = Mock.Of<IElementInfo>(x =>
                    x.Id == Guid.NewGuid() &&
                    x.Cardinality == Cardinality.ZeroToMany &&
                    x.Elements == new List<IAbstractElementInfo>
					{
						Mock.Of<IElementInfo>(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
						Mock.Of<ICollectionInfo>(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true),
						Mock.Of<ICollectionInfo>(e => e.Id == Guid.NewGuid() && e.Cardinality == Cardinality.ZeroToMany)
					} &&
                    x.Properties == new List<IPropertyInfo>
					{ 
						Mock.Of<IPropertyInfo>(p => p.Id == Guid.NewGuid() && p.Name == "Foo" + Guid.NewGuid() && p.Type == "System.String"),
						Mock.Of<IPropertyInfo>(p => p.Id == Guid.NewGuid() && p.Name == "Foo" + Guid.NewGuid() && p.Type == "System.String")
					} &&
                    x.ExtensionPoints == new List<IExtensionPointInfo>
					{
						Mock.Of<IExtensionPointInfo>(ep => ep.Id == Guid.NewGuid() && ep.Cardinality == Cardinality.OneToOne && ep.RequiredExtensionPointId == "ext_1"),
						Mock.Of<IExtensionPointInfo>(ep => ep.Id == Guid.NewGuid() && ep.Cardinality == Cardinality.ZeroToMany && ep.RequiredExtensionPointId == "ext_2"),
					});

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

                Mock.Get(viewInfo).Setup(x => x.Elements).Returns(new[] { elementInfo });

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

                using (var store = new Dsl.Store(serviceProvider.Object, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();

                    var element = productStore
                        .CreateProduct(x =>
                        {
                            x.ExtensionId = "test_toolkit";
                            x.DefinitionId = toolkit.Schema.Pattern.Id;
                        })
                        .CreateView(x => x.DefinitionId = viewInfo.Id)
                        .CreateElement(x => x.DefinitionId = this.elementInfo.Id);

                    this.elementId = element.Id;

                    element.CreateCollection(x => x.DefinitionId = this.elementInfo.Elements.ElementAt(2).Id);
                    element.CreateCollection(x => x.DefinitionId = this.elementInfo.Elements.ElementAt(2).Id);

                    element.CreateExtension(x =>
                    {
                        x.DefinitionId = extensionPoint1.Schema.Pattern.Id;
                        x.ExtensionId = extensionPoint1.Schema.Pattern.ExtensionId;
                    });
                    element.CreateExtension(x =>
                    {
                        x.DefinitionId = extensionPoint2.Schema.Pattern.Id;
                        x.ExtensionId = extensionPoint2.Schema.Pattern.ExtensionId;
                    });
                    element.CreateExtension(x =>
                    {
                        x.DefinitionId = extensionPoint2.Schema.Pattern.Id;
                        x.ExtensionId = extensionPoint2.Schema.Pattern.ExtensionId;
                    });

                    ProductStateStoreSerializationHelper.Instance.SaveModel(new Dsl.SerializationResult(), productStore, this.storeFilePath);
                    tx.Commit();
                }

                this.store = new Dsl.Store(serviceProvider.Object, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel));
            }

            [TestMethod]
            public void WhenDeserializingWithNewOneToOneCardinalityElementsInSchema_ThenAddNewElementsToRuntimeStore()
            {
                var singleIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();
                var mutipleIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

                var infos = (ICollection<IAbstractElementInfo>)this.elementInfo.Elements;
                infos.Add(Mock.Of<IElementInfo>(e => e.Id == singleIds[0] && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true));
                infos.Add(Mock.Of<IElementInfo>(e => e.Id == mutipleIds[0] && e.Cardinality == Cardinality.ZeroToMany));
                infos.Add(Mock.Of<ICollectionInfo>(e => e.Id == singleIds[1] && e.Cardinality == Cardinality.OneToOne && e.AutoCreate == true));
                infos.Add(Mock.Of<ICollectionInfo>(e => e.Id == mutipleIds[1] && e.Cardinality == Cardinality.ZeroToMany));

                var target = DeserializeElement();

                Assert.Equal(6, target.Elements.Count);
                Assert.True(target.Elements.Any(x => x.DefinitionId == singleIds[0]));
                Assert.True(target.Elements.Any(x => x.DefinitionId == singleIds[1]));
                Assert.False(target.Elements.Any(x => x.DefinitionId == mutipleIds[0]));
                Assert.False(target.Elements.Any(x => x.DefinitionId == mutipleIds[1]));
            }

            [TestMethod]
            public void WhenDeserializingWithDeletedElementsInSchema_ThenDeleteElementsFromStore()
            {
                var infos = (IList<IAbstractElementInfo>)this.elementInfo.Elements;
                var deletedId = infos[0].Id;
                infos.RemoveAt(0);

                var target = this.DeserializeElement();

                Assert.Equal(3, target.Elements.Count);
                Assert.False(target.Elements.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod]
            public void WhenDeserializingWithDeletedZeroToManyCardinalityElementsInSchema_ThenDeleteElementsFromStore()
            {
                var infos = (IList<IAbstractElementInfo>)this.elementInfo.Elements;
                var info = infos.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var deletedId = info.Id;
                infos.Remove(info);

                var target = this.DeserializeElement();

                Assert.Equal(2, target.Elements.Count);
                Assert.False(target.Elements.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod]
            public void WhenDeserializingWithNewProperties_ThenAddNewPropertiesToRuntimeStore()
            {
                var propertyIds = Enumerable.Range(0, 2).Select(x => Guid.NewGuid()).ToArray();

                var infos = (ICollection<IPropertyInfo>)this.elementInfo.Properties;
                infos.Add(Mock.Of<IPropertyInfo>(e => e.Id == propertyIds[0] && e.Name == "Foo" + Guid.NewGuid() && e.Type == "System.String"));
                infos.Add(Mock.Of<IPropertyInfo>(e => e.Id == propertyIds[1] && e.Name == "Foo" + Guid.NewGuid() && e.Type == "System.String"));

                var target = this.DeserializeElement();

                Assert.Equal(4, target.Properties.Count);
                Assert.True(target.Properties.Any(x => x.DefinitionId == propertyIds[0]));
                Assert.True(target.Properties.Any(x => x.DefinitionId == propertyIds[1]));
            }

            [TestMethod]
            public void WhenDeserializingWithNewPropertiesWithDefaultValues_ThenAddNewPropertiesWithValueToStore()
            {
                var propertyId = Guid.NewGuid();

                var infos = (ICollection<IPropertyInfo>)this.elementInfo.Properties;
                infos.Add(Mocks.Of<IPropertyInfo>().First(p => p.Id == propertyId &&
                    p.Name == "FooBar" &&
                    p.Type == "System.String" &&
                    p.DefaultValue == Mock.Of<IPropertyBindingSettings>(s => s.Value == "Bar")));

                var target = this.DeserializeElement();

                Assert.True(target.Properties.Any(p => p.DefinitionId == propertyId && p.RawValue == "Bar"));
            }

            [TestMethod]
            public void WhenDeserializingWithADeletedProperty_ThenRemovesPropertyFromStore()
            {
                var infos = (IList<IPropertyInfo>)this.elementInfo.Properties;
                var deletedId = infos[0].Id;
                infos.RemoveAt(0);

                var target = this.DeserializeElement();

                Assert.Equal(1, target.Properties.Count);
                Assert.False(target.Properties.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod]
            public void WhenDeserializingWithDeletedExtensionPointInSchema_ThenDeleteExtensionsPointsFromStore()
            {
                var infos = (IList<IExtensionPointInfo>)this.elementInfo.ExtensionPoints;
                var deletedId = infos[1].Id;
                infos.RemoveAt(1);

                var target = this.DeserializeElement();

                Assert.Equal(1, target.ExtensionProducts.Count);
                Assert.False(target.ExtensionProducts.Any(x => x.DefinitionId == deletedId));
            }

            [TestMethod]
            public void WhenDeserializingWithElementMutiplicityChangedFromMultipleToSingle_ThenDeletesAnyExtraElement()
            {
                var info = this.elementInfo.Elements.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var infoMock = Mock.Get((ICollectionInfo)info);
                infoMock.Setup(x => x.Cardinality).Returns(Cardinality.OneToOne);

                var target = this.DeserializeElement();

                Assert.Equal(3, target.Elements.Count);
            }

            [TestMethod]
            public void WhenDeserializingWithExtensionPointMutiplicityChangedFromMultipleToSingle_ThenDeletesAnyExtraExtensionPoint()
            {
                var info = this.elementInfo.ExtensionPoints.First(x => x.Cardinality == Cardinality.ZeroToMany);
                var infoMock = Mock.Get(info);
                infoMock.Setup(x => x.Cardinality).Returns(Cardinality.OneToOne);

                var target = this.DeserializeElement();

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

            private Element DeserializeElement()
            {
                using (var tx = this.store.TransactionManager.BeginTransaction("Loading", true))
                {
                    ProductStateStoreSerializationHelper.Instance.LoadModel(this.store, this.storeFilePath, null, null, null);
                    tx.Commit();
                }

                return (Element)this.store.ElementDirectory.FindElement(this.elementId);
            }
        }
    }
}