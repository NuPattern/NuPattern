using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class PatternModelClonerSpec
    {

        [TestClass]
        public class GivenANullPatternModel
        {
            internal static readonly IAssertion Assert = new Assertion();

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingACloner_ThenNullExceptionIsThrown()
            {
                Assert.Throws<ArgumentNullException>(() => new PatternModelCloner(null, null, null));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAPatternModel
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var store = new DslTestStore<PatternModelDomainModel>();

                store.TransactionManager.DoWithinTransaction(
                    () => this.sourcePatternModel = store.ElementFactory.CreateElement<PatternModelSchema>());

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingAClonerWithNullTargetPatternModel_ThenNullExceptionIsThrown()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), null));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAPatternModelWithAPattern
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAPatternModelWithAPatternAndDesign
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternWithProperties
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                patternSchema.Create<PropertySchema>().WithTransaction(
                    property =>
                    {
                        property.Name = "Property1";
                        property.Type = "MyType";
                        property.RawDefaultValue = "Foo";
                    });

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(
                    () => this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
                Assert.Equal(this.sourcePatternModel.Pattern.Properties.Count, this.targetPatternModel.Pattern.Properties.Count);
                Assert.Equal(this.sourcePatternModel.Pattern.Properties[0].Name, this.targetPatternModel.Pattern.Properties[0].Name);
                Assert.Equal(this.sourcePatternModel.Pattern.Properties[0].Type, this.targetPatternModel.Pattern.Properties[0].Type);
                Assert.Equal(this.sourcePatternModel.Pattern.Properties[0].RawDefaultValue, this.targetPatternModel.Pattern.Properties[0].RawDefaultValue);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternAndView
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                patternSchema.Create<ViewSchema>();

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternAndViewWithExtensionPoint
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                patternSchema.Create<ViewSchema>()
                    .Create<ExtensionPointSchema>();

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].ExtensionPoints[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].ExtensionPoints[0].Name, this.targetPatternModel.Pattern.Views[0].ExtensionPoints[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternViewsAndElements
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                var element = view.Create<ElementSchema>();
                element.Create<ElementSchema>();

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements.Count, this.targetPatternModel.Pattern.Views[0].Elements.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].Name);

                Assert.Equal(((ElementSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements.Count,
                    ((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements.Count);
                Assert.NotNull(((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0]);
                Assert.Equal(((ElementSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name,
                    ((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternViewsAndElementsWithExtensionPoint
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                var element = view.Create<ElementSchema>();

                element.Create<ElementSchema>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                    element.ExtensionPoints.Add(sourceStore.ElementFactory.CreateElement<ExtensionPointSchema>()));

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements.Count, this.targetPatternModel.Pattern.Views[0].Elements.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].Name);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0].Name);

                Assert.Equal(((ElementSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements.Count,
                    ((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements.Count);
                Assert.NotNull(((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0]);
                Assert.Equal(((ElementSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name,
                    ((ElementSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternViewsAndCollections
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                var collection = view.Create<CollectionSchema>();
                collection.Create<CollectionSchema>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                    collection.ExtensionPoints.Add(sourceStore.ElementFactory.CreateElement<ExtensionPointSchema>()));

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements.Count, this.targetPatternModel.Pattern.Views[0].Elements.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].Name);

                Assert.Equal(
                    ((CollectionSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements.Count,
                    ((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements.Count);
                Assert.NotNull(((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0]);
                Assert.Equal(
                    ((CollectionSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name,
                    ((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithAPatternViewsAndCollectionsWithExtensionPoint
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var patternSchema = this.sourcePatternModel.Create<PatternSchema>();
                patternSchema.Name = "Pattern1";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                var collection = view.Create<CollectionSchema>();
                collection.Create<CollectionSchema>();

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                    collection.ExtensionPoints.Add(
                        sourceStore.ElementFactory.CreateElement<ExtensionPointSchema>()));

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views.Count, this.targetPatternModel.Pattern.Views.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Name, this.targetPatternModel.Pattern.Views[0].Name);

                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements.Count, this.targetPatternModel.Pattern.Views[0].Elements.Count);
                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].Name);

                Assert.NotNull(this.targetPatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0]);
                Assert.Equal(this.sourcePatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0].Name, this.targetPatternModel.Pattern.Views[0].Elements[0].ExtensionPoints[0].Name);

                Assert.Equal(
                    ((CollectionSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements.Count,
                    ((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements.Count);
                Assert.NotNull(((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0]);
                Assert.Equal(
                    ((CollectionSchema)this.sourcePatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name,
                    ((CollectionSchema)this.targetPatternModel.Pattern.Views[0].Elements[0]).Elements[0].Name);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnPatternModelWithRegistration
        {
            internal static readonly IAssertion Assert = new Assertion();

            private PatternModelSchema sourcePatternModel;
            private PatternModelSchema targetPatternModel;

            [TestInitialize]
            public void InitializeContext()
            {
                var sourceStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(sourceStore.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                sourceStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.sourcePatternModel = sourceStore.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "MyToolkit"));

                var pattern = this.sourcePatternModel.Create<PatternSchema>();
                pattern.Name = "Pattern1";
                pattern.PatternLink = "patternmanager://foo";
                pattern.UriService = uriService;

                var targetStore = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider1 = Mock.Get(targetStore.ServiceProvider);
                serviceProvider1.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                targetStore.TransactionManager.DoWithinTransaction(() =>
                {
                    this.targetPatternModel = targetStore.ElementFactory.CreateElement<PatternModelSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCloning_ThenTargetPatternIsCloned()
            {
                new PatternModelCloner(this.sourcePatternModel, new Version("1.0.0.0"), this.targetPatternModel).Clone();

                Assert.NotNull(this.targetPatternModel.Pattern);
                Assert.Equal(this.sourcePatternModel.Pattern.Name, this.targetPatternModel.Pattern.Name);
            }
        }
    }
}