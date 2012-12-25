using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Runtime.Store;
using Toolkit14;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.UnitTests
{
    [TestClass]
    public class ToolkitInterfaceLayerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private static class Ids
        {
            public const string ExtensionToolkitId = "ToolkitExtension";
            public static readonly Guid ExtensionProductId = Guid.NewGuid();

            public const string WebServiceToolkitId = "MainToolkit";
            public static readonly Guid WebServiceProductId = new Guid("a7d76993-7a93-4bd1-b4f2-1e72af2796a2");
            public static readonly Guid ArchitectureViewId = new Guid("decb63d3-5dd6-488c-a606-65e01a232320");
            public static readonly Guid DataContractElementId = new Guid("85a60694-1cd9-42db-b531-102c987ab947");
            public static readonly Guid FolderCollectionId = new Guid("ccca4e03-00c1-49cf-bcb7-a5eef9243a71");
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenContainerDoesNotContainVariableProperty_ThenThrowsNotSupportedException()
        {
            IWebService ws = new WebService(new Mock<IProduct>().Object);

            Assert.Throws<NotSupportedException>(() => Console.WriteLine(ws.XmlNamespace));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsViewImplementsInterface_ThenReturnsSameInstance()
        {
            var view = new Mock<IArchitecture>().As<IView>().Object;

            var layer = view.As<IArchitecture>();

            Assert.Same(view, layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIAbstractElementImplementsInterface_ThenReturnsSameInstance()
        {
            var target = new Mock<IFolder>().As<IAbstractElement>().Object;

            var layer = target.As<IFolder>();

            Assert.Same(target, layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsProductNoStore_ThenReturnsNull()
        {
            var target = new Mock<object>().As<IProduct>().Object;

            var layer = target.As<IWebService>();

            Assert.Null(layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsProductImplementsInterface_ThenReturnsSameInstance()
        {
            var target = new Mock<IWebService>().As<IProduct>().Object;

            var layer = target.As<IWebService>();

            Assert.Same(target, layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIProductElementImplementsInterface_ThenReturnsSameInstance()
        {
            var target = new Mock<IDataContract>().As<IProductElement>().Object;

            var layer = target.As<IDataContract>();

            Assert.Same(target, layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIInstanceBaseImplementsInterface_ThenReturnsSameInstance()
        {
            var target = new Mock<IFolder>().As<IAbstractElement>().As<IInstanceBase>().Object;

            var layer = target.As<IFolder>();

            Assert.Same(target, layer);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsViewFindsCachedLayer_ThenReturnsSameInstance()
        {
            var definitionId = new Guid(ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(typeof(IArchitecture), true).DefinitionId);
            var bag = new Dictionary<object, object>();
            var cached = Mock.Of<IArchitecture>();
            var target = Mock.Of<IView>(x => x.Root.ProductState.PropertyBag == bag && x.DefinitionId == definitionId);
            bag[new ToolkitInterfaceLayerCacheKey(target, definitionId)] = cached;

            var layer = target.As<IArchitecture>();

            Assert.Same(layer, cached);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIAbstractElementFindsCachedLayer_ThenReturnsSameInstance()
        {
            var definitionId = new Guid(ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(typeof(IFolder), true).DefinitionId);
            var bag = new Dictionary<object, object>();
            var cached = Mock.Of<IFolder>();
            var target = Mock.Of<IAbstractElement>(x => x.Root.ProductState.PropertyBag == bag && x.DefinitionId == definitionId);
            bag[new ToolkitInterfaceLayerCacheKey(target, definitionId)] = cached;

            var layer = target.As<IFolder>();

            Assert.Same(layer, cached);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsProductFindsCachedLayer_ThenReturnsSameInstance()
        {
            var definitionId = new Guid(ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(typeof(IWebService), true).DefinitionId);
            var bag = new Dictionary<object, object>();
            var cached = Mock.Of<IWebService>();
            var target = Mock.Of<IProduct>(x => x.Root.ProductState.PropertyBag == bag && x.DefinitionId == definitionId);
            bag[new ToolkitInterfaceLayerCacheKey(target, definitionId)] = cached;

            var layer = target.As<IWebService>();

            Assert.Same(layer, cached);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIProductElementFindsCachedLayer_ThenReturnsSameInstance()
        {
            var definitionId = new Guid(ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(typeof(IDataContract), true).DefinitionId);
            var bag = new Dictionary<object, object>();
            var cached = Mock.Of<IDataContract>();
            var target = Mock.Of<IElement>(x => x.Root.ProductState.PropertyBag == bag && x.DefinitionId == definitionId);
            bag[new ToolkitInterfaceLayerCacheKey(target, definitionId)] = cached;

            var layer = ((IProductElement)target).As<IDataContract>();

            Assert.Same(layer, cached);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenAsIInstanceBaseFindsCachedLayer_ThenReturnsSameInstance()
        {
            var definitionId = new Guid(ReflectionExtensions.GetCustomAttribute<ToolkitInterfaceAttribute>(typeof(IFolder), true).DefinitionId);
            var bag = new Dictionary<object, object>();
            var cached = Mock.Of<IFolder>();
            var element = new Mock<IAbstractElement>();
            element.Setup(x => x.Root.ProductState.PropertyBag).Returns(bag);
            element.Setup(x => x.DefinitionId).Returns(definitionId);
            var target = element.As<IInstanceBase>().Object;

            bag[new ToolkitInterfaceLayerCacheKey(target, definitionId)] = cached;

            var layer = target.As<IFolder>();

            Assert.Same(layer, cached);
        }

        [TestClass]
        public class GivenAFullProduct
        {
            private IProduct product;

            [TestInitialize]
            public void Initialize()
            {
                this.product = Mocks.Of<IProduct>().First(prod =>
                    prod.InstanceName == "kzu" &&
                    prod.DefinitionId == new Guid("a7d76993-7a93-4bd1-b4f2-1e72af2796a2") &&
                    prod.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
                    prod.ProductState.PropertyBag == new Dictionary<object, object>() &&
                    prod.Properties == new[]
					{
						Mocks.Of<IProperty>().First(prop => 
							prop.DefinitionName == Reflector<IWebService>.GetPropertyName(x => x.XmlNamespace) && prop.Value == (object)"microsoft.com"),
						// This property is set to false and a *string* (not the boolean typed value), but the custom converter turns it into true always.
						Mocks.Of<IProperty>().First(prop => 
							prop.DefinitionName == Reflector<IWebService>.GetPropertyName(x => x.IsSecured) && prop.Value == (object)"false"), 				
					} &&
                    prod.Views == new[]
					{
						Mocks.Of<IView>().First(view => 
							view.DefinitionName == "Architecture" && 
							view.DefinitionId == Ids.ArchitectureViewId &&
							view.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
							view.Product.ProductState.PropertyBag == new Dictionary<object, object>() &&
							view.Elements == new[]
							{
								Mocks.Of<ICollection>().First(collection => 
									collection.DefinitionName == "Folder" && 
									collection.DefinitionId == Ids.FolderCollectionId &&
									collection.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
									collection.Product.ProductState.PropertyBag == new Dictionary<object, object>() &&
									collection.Elements == new []
									{
										Mocks.Of<IElement>().First(element => 
											element.DefinitionName == "DataContract" &&
											element.DefinitionId == Ids.DataContractElementId &&
											element.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
											element.Product.ProductState.PropertyBag == new Dictionary<object, object>() &&
											element.Properties == new []
											{
												Mocks.Of<IProperty>().First(prop => prop.DefinitionName == Reflector<IDataContract>.GetPropertyName(x => x.XsdFile) && prop.Value == (object)"schema1.xsd"),
											}),
										Mocks.Of<IElement>().First(element => 
											element.DefinitionName == "DataContract" &&
											element.DefinitionId == Ids.DataContractElementId &&
											element.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
											element.Product.ProductState.PropertyBag == new Dictionary<object, object>() &&
											element.Properties == new []
											{
												Mocks.Of<IProperty>().First(prop => prop.DefinitionName == Reflector<IDataContract>.GetPropertyName(x => x.XsdFile) && prop.Value == (object)"schema2.xsd"),
											}),
									}),
							}),
					});


                var collections = new Stack<ICollection>();
                var elements = new Stack<IElement>();

                Mock.Get(this.product.Views.First())
                    .Setup(x => x.CreateCollection(It.IsAny<Action<ICollection>>(), It.IsAny<bool>()))
                    .Returns(() =>
                    {
                        var collection = new Mock<ICollection>();
                        collection.SetupAllProperties();
                        collection.Setup(x => x.BeginTransaction()).Returns(new Mock<ITransaction>().Object);
                        collection.Setup(x => x.CreateElement(It.IsAny<Action<IElement>>(), It.IsAny<bool>()))
                            .Returns(() =>
                            {
                                var element = Mocks.Of<IElement>().First(x =>
                                    x.DefinitionName == "DataContract" &&
                                    x.BeginTransaction() == Mocks.Of<ITransaction>().First() &&
                                    x.Properties == new[]
									{
										Mocks.Of<IProperty>().First(prop => 
											prop.DefinitionName == Reflector<IDataContract>.GetPropertyName(c => c.XsdFile)),
									});

                                elements.Push(element);

                                return element;
                            })
                            .Callback<Action<IElement>, bool>((action, raise) =>
                            {
                                action(elements.Peek());
                                var current = collection.Object.Elements.ToList();
                                current.Add(elements.Peek());
                                collection.Setup(x => x.Elements)
                                    .Returns(current);
                            });

                        collections.Push(collection.Object);

                        return collection.Object;
                    })
                    .Callback<Action<ICollection>, bool>((action, raise) =>
                    {
                        action(collections.Peek());
                        var current = this.product.Views.First().Elements.ToList();
                        current.Add(collections.Peek());
                        Mock.Get(this.product.Views.First())
                            .Setup(x => x.Elements)
                            .Returns(current);
                    });

                // Set Root on everything.
                Mock.Get(this.product).Setup(x => x.Root).Returns(this.product);
                foreach (var view in this.product.Views)
                {
                    Mock.Get(view).Setup(x => x.Root).Returns(this.product);
                    foreach (var collection in view.Elements.OfType<ICollection>())
                    {
                        Mock.Get(collection).Setup(x => x.Root).Returns(this.product);
                        foreach (var element in collection.Elements.OfType<IElement>())
                        {
                            Mock.Get(element).Setup(x => x.Root).Returns(this.product);
                        }
                    }
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAccessingProperties_BuiltInConverterReturnsTypedValue()
            {
                IWebService ws = new WebService(this.product);

                Assert.Equal("microsoft.com", ws.XmlNamespace);
                Assert.Equal("kzu", ws.InstanceName);
                Assert.True(ws.IsSecured);

                Assert.NotNull(ws.Architecture);
                Assert.NotNull(ws.Architecture.Folders);
                Assert.Equal(1, ws.Architecture.Folders.Count());
                Assert.Equal(2, ws.Architecture.Folders.First().DataContracts.Count());
                Assert.Equal("schema1.xsd", ws.Architecture.Folders.First().DataContracts.First().XsdFile);
                Assert.Equal("schema2.xsd", ws.Architecture.Folders.First().DataContracts.Skip(1).First().XsdFile);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingProperties_BuiltInConverterIsUsed()
            {
                IWebService ws = new WebService(this.product);
                Assert.Equal("microsoft.com", ws.XmlNamespace);

                ws.XmlNamespace = "live.com";
                Assert.Equal("live.com", ws.XmlNamespace);

                ws.IsSecured = true;
                Assert.True(ws.IsSecured);
                ws.IsSecured = false;
                Assert.False(ws.IsSecured);

                ws.InstanceName = "foobar";

                Assert.Equal("foobar", ws.InstanceName);
                Assert.Equal("foobar", this.product.InstanceName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConvertingAsInterface_ThenFindsImplementationFromAttribute()
            {
                var ws = this.product.As<IWebService>();

                Assert.NotNull(ws);

                var view = this.product.Views.First().As<IArchitecture>();

                Assert.NotNull(view);

                var folder = this.product.Views.First().Elements.First().As<IFolder>();

                Assert.NotNull(folder);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingChild_ThenFindsImplementationFromAttribute()
            {
                var ws = this.product.As<IWebService>();
                var folder = ws.Architecture.CreateFolder("Folder");
                var contract = folder.CreateDataContract("Foo", c => c.XsdFile = "foo.xsd");

                Assert.NotNull(contract);

                Assert.Equal("foo.xsd", contract.XsdFile);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAccessingChild_ThenCachesInterface()
            {
                var ws = this.product.As<IWebService>();

                Assert.Same(ws.Architecture, ws.Architecture);
                Assert.Same(ws.Architecture.Folders.First(), ws.Architecture.Folders.First());
                Assert.Same(ws.Architecture.Folders.First().DataContracts.First(), ws.Architecture.Folders.First().DataContracts.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConvertingFromIInstanceBase_ThenFindsRightConvertOverload()
            {
                var ws = ((IInstanceBase)this.product).As<IWebService>();

                Assert.NotNull(ws);

                var view = ((IInstanceBase)this.product.Views.First()).As<IArchitecture>();

                Assert.NotNull(view);

                var folder = ((IInstanceBase)this.product.Views.First().Elements.First()).As<IFolder>();

                Assert.NotNull(folder);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConvertingFromInvalidInstanceBase_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() =>
                    ((IInstanceBase)this.product.Properties.First()).As<IWebService>());
            }
        }

        [TestClass]
        public class GivenAnInstalledToolkit
        {
            private IProduct product;
            private Dsl.Store store;
            private string storeFilePath = Path.GetTempFileName();

            [TestInitialize]
            [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
            public void Initialize()
            {
                var serviceProvider = Mocks.Of<IServiceProvider>().First(provider =>
                    provider.GetService(typeof(IPatternManager)) == Mocks.Of<IPatternManager>().First(manager =>
                        manager.InstalledToolkits == new IInstalledToolkitInfo[] 
						{
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.ExtensionToolkitId &&
								toolkitInfo.Schema.Pattern.Id == Ids.ExtensionProductId &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.ExtensionToolkitId &&
								toolkitInfo.Schema.Pattern.ProvidedExtensionPoints == new[]
								{
									Mocks.Of<IProvidedExtensionPointInfo>().First(extensionPoint => extensionPoint.ExtensionPointId == "ValidExtensionPointId")
								}),
							Mocks.Of<IInstalledToolkitInfo>().First(toolkitInfo => 
								toolkitInfo.Id == Ids.WebServiceToolkitId &&
								toolkitInfo.Schema.Pattern.Id == Ids.WebServiceProductId &&
								toolkitInfo.Schema.Pattern.Name == "Pattern" &&
								toolkitInfo.Schema.Pattern.ExtensionId == Ids.WebServiceToolkitId &&
								toolkitInfo.Schema.Pattern.Properties == new [] 
								{
									Mocks.Of<IPropertyInfo>().First(isSecured => 
										isSecured.Name == "IsSecured" && 
										isSecured.Type == typeof(bool).FullName &&
										isSecured.TypeConverterTypeName == "Toolkit14.TrueConverter"),
									Mocks.Of<IPropertyInfo>().First(xmlNamespac => 
										xmlNamespac.Name == "XmlNamespace" && 
										xmlNamespac.Type == typeof(string).FullName &&
										xmlNamespac.DefaultValue.Value == "microsoft.com"),
								} && 
								toolkitInfo.Schema.Pattern.Views == new []
								{
									Mocks.Of<IViewInfo>().First(view => 
										view.Id == Ids.ArchitectureViewId && 
										view.Name == "Architecture" &&
										view.Elements == new IAbstractElementInfo[]
										{
											Mocks.Of<ICollectionInfo>().First(folder =>
												folder.Id == Ids.FolderCollectionId && 
												folder.Name == "Folder" && 
												folder.Cardinality == Cardinality.ZeroToMany && 
												folder.Elements == new IAbstractElementInfo[] 
												{
													Mocks.Of<IElementInfo>().First(dataContract => 
														dataContract.Id == Ids.DataContractElementId && 
														dataContract.Name == "DataContract" && 
														dataContract.Cardinality == Cardinality.ZeroToMany && 
														dataContract.Properties == new [] 
														{
															Mocks.Of<IPropertyInfo>().First(xsdFile => 
																xsdFile.Id == Guid.NewGuid() && 
																xsdFile.Name == "XsdFile" && 
																xsdFile.Type == typeof(string).FullName
																),
														} && 
														dataContract.ExtensionPoints == new [] 
														{
															Mocks.Of<IExtensionPointInfo>().First(extensionPoint => 
																extensionPoint.Id == Guid.NewGuid() && 
																extensionPoint.Name == "Serializer" && 
																extensionPoint.Cardinality == Cardinality.ZeroToMany && 
																extensionPoint.RequiredExtensionPointId == "ValidExtensionPointId"
																),
														}), 
												}), 
										})
								}),
						})
                    );

                using (var store = new Dsl.Store(serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel)))
                using (var tx = store.TransactionManager.BeginTransaction())
                {
                    var productStore = store.ElementFactory.CreateElement<ProductState>();
                    productStore
                        .CreateProduct(p => { p.ExtensionId = Ids.WebServiceToolkitId; p.DefinitionId = Ids.WebServiceProductId; });
                    //.CreateView(v => v.DefinitionId = Ids.ArchitectureViewId);

                    ProductStateStoreSerializationHelper.Instance.SaveModel(new Dsl.SerializationResult(), productStore, this.storeFilePath);
                    tx.Commit();
                }

                this.store = new Dsl.Store(serviceProvider, typeof(Dsl.CoreDomainModel), typeof(ProductStateStoreDomainModel));
                using (var tx = this.store.TransactionManager.BeginTransaction("Loading", true))
                {
                    ProductStateStoreSerializationHelper.Instance.LoadModel(this.store, this.storeFilePath, null, null, null);
                    tx.Commit();
                }

                this.product = this.store.ElementDirectory.FindElements<Product>().First();
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAccessingView_ThenItIsNonNull()
            {
                var ws = this.product.As<IWebService>();

                Assert.NotNull(ws.Architecture);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingFolder_ThenAddsToCollection()
            {
                var view = this.product.As<IWebService>().Architecture;
                var folder = view.CreateFolder("Foo");

                Assert.NotNull(folder);
                Assert.Equal("Foo", folder.InstanceName);
                Assert.Equal(1, view.Folders.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultipleFolders_ThenAddsToCollection()
            {
                this.product.As<IWebService>().Architecture.CreateFolder("Foo");
                this.product.As<IWebService>().Architecture.CreateFolder("Bar");

                Assert.Equal(2, this.product.As<IWebService>().Architecture.Folders.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingDataContract_ThenAddsToCollection()
            {
                var folder = this.product.As<IWebService>().Architecture.CreateFolder("Foo");
                var dataContract = folder.CreateDataContract("Bar");

                Assert.Equal("Bar", dataContract.InstanceName);
                Assert.Equal(1, folder.DataContracts.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAsWrongInterfaceLayer_ThenReturnsNull()
            {
                var folder = this.product.As<IDataContract>();

                Assert.Null(folder);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAsWrongInterfaceLayer2_ThenReturnsNull()
            {
                var view = this.product.As<IWebService>().Architecture;
                var folder = view.CreateFolder("Foo");
                Assert.NotNull(folder);

                var element = folder.As<IProductElement>();
                Assert.NotNull(element);

                var contract = element.As<IDataContract>();

                Assert.Null(contract);
            }
        }
    }
}