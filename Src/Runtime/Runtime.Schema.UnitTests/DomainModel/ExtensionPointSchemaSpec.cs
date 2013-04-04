using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class ExtensionPointSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredView
        {
            private ViewSchema view;
            private ExtensionPointSchema extensionPoint;

            [TestInitialize]
            public void InitializeContext()
            {
                var store = new DslTestStore<PatternModelDomainModel>();
                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                this.view = patternSchema.Create<ViewSchema>();
                this.view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");
                        this.view.ExtensionPoints.Add(this.extensionPoint);
                    });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.view.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.ExtensionPoint1",
                    this.extensionPoint.RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                var expected = string.Concat(this.view.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.extensionPoint.Name);
                Assert.Equal(expected, this.extensionPoint.SchemaPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingName_ThenExtensionPointIdReturnsNewValue()
            {
                this.extensionPoint.WithTransaction(extension => extension.Name = "ExtensionPoint2");

                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.ExtensionPoint2",
                    this.extensionPoint.RequiredExtensionPointId);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenATailoredView
        {
            private ViewSchema view;
            private ExtensionPointSchema authoredExtensionPoint;
            private ExtensionPointSchema tailoredExtensionPoint;

            [TestInitialize]
            public void InitializeContext()
            {
                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema authoredPatternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    authoredPatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var patternSchema = authoredPatternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                this.view = patternSchema.Create<ViewSchema>();
                this.view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.authoredExtensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");
                    this.view.ExtensionPoints.Add(this.authoredExtensionPoint);

                });

                // Customize Toolkit
                var tailoredUriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                        p.ToolkitInfo.Identifier == "TailoredToolkit"));

                var tailoredPatternModel = PatternModelSpec.TailorPatternModel(authoredPatternModel, new Version("1.0.0.0"), "AuthoredToolkit");

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    tailoredPatternModel.Pattern.UriService = tailoredUriService;
                    tailoredPatternModel.BaseId = "AuthoredToolkit";
                });

                this.tailoredExtensionPoint = tailoredPatternModel.Pattern.Views[0].ExtensionPoints[0];
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsBaseExtensionPointIdValue()
            {
                Assert.NotNull(this.tailoredExtensionPoint);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.ExtensionPoint1",
                    this.tailoredExtensionPoint.RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAddingANewProperty_ThenRequiredExtensionPointIdReturnsTailoredExtensionPointIdValue()
            {
                this.tailoredExtensionPoint.WithTransaction(extension => extension.CreatePropertySchema());

                Assert.NotNull(this.tailoredExtensionPoint);
                Assert.Equal(
                    "TailoredToolkit.FooPattern.View1.ExtensionPoint1",
                    this.tailoredExtensionPoint.RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenRemovingAProperty_ThenRequiredExtensionPointIdReturnsBaseExtensionPointIdValue()
            {
                this.tailoredExtensionPoint.WithTransaction(extension => extension.CreatePropertySchema());

                Assert.NotNull(this.tailoredExtensionPoint);
                Assert.Equal(
                    "TailoredToolkit.FooPattern.View1.ExtensionPoint1",
                    this.tailoredExtensionPoint.RequiredExtensionPointId);

                this.tailoredExtensionPoint.WithTransaction(extension => extension.Properties.RemoveAt(0));

                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.ExtensionPoint1",
                    this.tailoredExtensionPoint.RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingName_ThenExtensionPointIdReturnsAuthoredValue()
            {
                this.tailoredExtensionPoint.WithTransaction(extension => extension.Name = "ExtensionPoint2");

                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.ExtensionPoint2",
                    this.tailoredExtensionPoint.RequiredExtensionPointId);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElement
        {
            private ElementSchema element;
            private ExtensionPointSchema extensionPoint;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.element = store.ElementFactory.CreateElement<ElementSchema>("Element1");
                        this.extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");
                        this.element.ExtensionPoints.Add(this.extensionPoint);
                        view.Elements.Add(element);
                    });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.element.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Element1.ExtensionPoint1",
                    this.element.ExtensionPoints[0].RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                var expected = string.Concat(this.element.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.extensionPoint.Name);
                Assert.Equal(expected, this.extensionPoint.SchemaPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingName_TheExtensionPointIdReturnsNewValue()
            {
                this.extensionPoint.WithTransaction(extension => extension.Name = "ExtensionPoint2");

                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Element1.ExtensionPoint2",
                    this.extensionPoint.RequiredExtensionPointId);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElementWithinAnElement
        {
            private ElementSchema element;
            private ExtensionPointSchema extensionPoint;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    var parentElement = store.ElementFactory.CreateElement<ElementSchema>("Element1");
                    this.element = store.ElementFactory.CreateElement<ElementSchema>("Element1");
                    this.extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");
                    this.element.ExtensionPoints.Add(this.extensionPoint);
                    parentElement.Elements.Add(element);
                    view.Elements.Add(parentElement);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.element.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Element1.Element1.ExtensionPoint1",
                    this.extensionPoint.RequiredExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                var expected = string.Concat(this.element.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.extensionPoint.Name);
                Assert.Equal(expected, this.extensionPoint.SchemaPath);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test Code"), TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACollection
        {
            private CollectionSchema collection;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.collection = store.ElementFactory.CreateElement<CollectionSchema>("Collection1");

                    var extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");

                    this.collection.ExtensionPoints.Add(extensionPoint);
                    view.Elements.Add(collection);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.collection.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Collection1.ExtensionPoint1",
                    this.collection.ExtensionPoints[0].RequiredExtensionPointId);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test Code"), TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenACollectionWithinACollection
        {
            private CollectionSchema collection;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    var parentCollection = store.ElementFactory.CreateElement<CollectionSchema>("Collection1");
                    this.collection = store.ElementFactory.CreateElement<CollectionSchema>("Collection1");

                    var extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");

                    this.collection.ExtensionPoints.Add(extensionPoint);

                    parentCollection.Elements.Add(collection);
                    view.Elements.Add(parentCollection);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.collection.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Collection1.Collection1.ExtensionPoint1",
                    this.collection.ExtensionPoints[0].RequiredExtensionPointId);
            }
        }

        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Test Code"), TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnElementWithinACollection
        {
            private ElementSchema element;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    var parentCollection = store.ElementFactory.CreateElement<CollectionSchema>("Collection1");
                    this.element = store.ElementFactory.CreateElement<ElementSchema>("Element1");

                    var extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");

                    this.element.ExtensionPoints.Add(extensionPoint);

                    parentCollection.Elements.Add(element);
                    view.Elements.Add(parentCollection);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.element.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Collection1.Element1.ExtensionPoint1",
                    this.element.ExtensionPoints[0].RequiredExtensionPointId);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnCollectionWithinAnElement
        {
            private CollectionSchema collection;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "AuthoredToolkit"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                });

                var patternSchema = patternModel.Create<PatternSchema>();
                patternSchema.Name = "FooPattern";
                patternSchema.PatternLink = "patternmanager://foo";
                patternSchema.UriService = uriService;

                var view = patternSchema.Create<ViewSchema>();
                view.Name = "View1";

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    var parentElement = store.ElementFactory.CreateElement<ElementSchema>("Element1");
                    this.collection = store.ElementFactory.CreateElement<CollectionSchema>("Collection1");

                    var extensionPoint = store.ElementFactory.CreateElement<ExtensionPointSchema>("ExtensionPoint1");

                    this.collection.ExtensionPoints.Add(extensionPoint);

                    parentElement.Elements.Add(collection);
                    view.Elements.Add(parentElement);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenRequiredExtensionPointIdReturnsValue()
            {
                Assert.NotNull(this.collection.ExtensionPoints[0]);
                Assert.Equal(
                    "AuthoredToolkit.FooPattern.View1.Element1.Collection1.ExtensionPoint1",
                    this.collection.ExtensionPoints[0].RequiredExtensionPointId);
            }
        }
    }
}