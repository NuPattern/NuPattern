using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.ComponentModel.Design;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema.Design;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Schema.UnitTests.Design
{
    public class ProvidedExtensionPointsPropertyDescriptorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenAnAuthoredPattern
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema pattern;
            private ProvidedExtensionPointsPropertyDescriptor descriptor;
            private IEnumerable<IExtensionPointSchema> allExtensionPoints;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<IUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "ToolkitId"));

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.pattern = patternModel.CreatePatternSchema() as PatternSchema;
                    pattern.PatternLink = "patternmanager://foo";
                });

                var globalExtension = new Mock<IExtensionPointSchema>();
                globalExtension.Setup(g => g.RequiredExtensionPointId).Returns("testExtensionPoint");
                this.allExtensionPoints = new[] { globalExtension.Object };

                this.descriptor = new ProvidedExtensionPointsPropertyDescriptor(
                                this.pattern,
                                allExtensionPoints,
                                new Mock<IUserMessageService>().Object,
                                "ImplementedExtensionPointsRaw",
                                string.Empty);

                this.descriptor.UriService = uriService;
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingConverter_ThenReturnsExtensionPointsConverter()
            {
                Assert.True(this.descriptor.Converter.GetType() == typeof(ExtensionPointsConverter));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingEditor_ThenReturnsCollectionConverter()
            {
                Assert.True(this.descriptor.GetEditor(typeof(UITypeEditor)).GetType() == typeof(CollectionDropDownEditor<IExtensionPointSchema>));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingPropertyType_ThenReturnsStringType()
            {
                Assert.True(this.descriptor.PropertyType == typeof(string));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingReadOnlyProperty_ThenReturnsFalse()
            {
                Assert.False(this.descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingCanResetValue_ThenReturnsTrue()
            {
                Assert.True(this.descriptor.CanResetValue(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenReturnsValue()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var e = this.pattern.Create<ProvidedExtensionPointSchema>();
                    e.ExtensionPointId = this.allExtensionPoints.First().RequiredExtensionPointId;
                });

                CollectionAssert.AreEqual(this.allExtensionPoints.ToList(), (System.Collections.ICollection)this.descriptor.GetValue(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingValue_ThenDeletesAllProvidedExtensionsAndContractProperties()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var e = this.pattern.Create<ProvidedExtensionPointSchema>();
                    e.ExtensionPointId = this.allExtensionPoints.First().RequiredExtensionPointId;

                    // Two ordinary properties
                    this.pattern.Create<PropertySchema>();
                    this.pattern.Create<PropertySchema>();
                });

                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    // Make two contract properties
                    this.pattern.Properties.Add(this.pattern.Properties[0].CloneAsExtensionContractProperty(this.pattern));
                    this.pattern.Properties.Add(this.pattern.Properties[1].CloneAsExtensionContractProperty(this.pattern));
                });

                this.descriptor.ResetValue(null);

                Assert.True(this.pattern.ProvidedExtensionPoints.Count == 0);
                Assert.Equal(2, this.pattern.Properties.Count());
            }
        }

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenARegisteredExtension
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema patternSchema;
            private ProvidedExtensionPointsPropertyDescriptor descriptor;
            private List<IExtensionPointSchema> extensionPoints;

            [TestInitialize]
            public void InitializeContext()
            {
                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                ExtensionPointSchema extensionPoint = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();

                    var uriService = Mock.Of<IUriReferenceService>(
                        u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                            p.ToolkitInfo.Identifier == "ToolkitId"));

                    this.patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    this.patternSchema.PatternLink = "patternmanager://foo";
                    this.patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";
                    var e = extensionPoint = view.Create<ExtensionPointSchema>();
                    e.Name = "ep1";

                    var property = extensionPoint.Create<PropertySchema>();
                    property.Name = "ContractProperty1";
                    property.Type = typeof(string).Name;
                });

                this.extensionPoints = new List<IExtensionPointSchema> { extensionPoint };

                this.descriptor = new ProvidedExtensionPointsPropertyDescriptor(
                                this.patternSchema,
                                this.extensionPoints,
                                new Mock<IUserMessageService>().Object,
                                "ImplementedExtensionPointsRaw",
                                string.Empty);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValue_ThenSetsImplementedExtensionPointsAndVariableProperties()
            {
                this.descriptor.SetValue(null, this.extensionPoints);

                Assert.Equal(1, this.patternSchema.ProvidedExtensionPoints.Count());

                var extensionPoint = this.patternSchema.ProvidedExtensionPoints[0];
                Assert.Equal("ToolkitId.Pattern1.View1.ep1", extensionPoint.ExtensionPointId);

                Assert.Equal(1, this.patternSchema.Properties.Count());

                var property1 = this.patternSchema.Properties[0];
                Assert.Equal("ContractProperty1", property1.Name);
                Assert.Equal(typeof(string).Name, property1.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenReturnsValue()
            {
                this.descriptor.SetValue(null, this.extensionPoints);
                Assert.NotNull(this.descriptor.GetValue(null));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenTwoRegisteredExtensionsWithDifferentVariableProperties
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema patternSchema;
            private ProvidedExtensionPointsPropertyDescriptor descriptor;
            private List<IExtensionPointSchema> extensionPoints;

            [TestInitialize]
            public void InitializeContext()
            {
                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                ExtensionPointSchema extensionPoint1 = null;
                ExtensionPointSchema extensionPoint2 = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();

                    var uriService = Mock.Of<IUriReferenceService>(
                        u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                            p.ToolkitInfo.Identifier == "ToolkitId"));

                    this.patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    this.patternSchema.PatternLink = "patternmanager://foo";
                    this.patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";
                    extensionPoint1 = view.Create<ExtensionPointSchema>();
                    extensionPoint1.Name = "ep1";

                    var prop1 = extensionPoint1.Create<PropertySchema>();
                    prop1.Name = "ContractProperty1";
                    prop1.Type = typeof(string).Name;

                    extensionPoint2 = view.Create<ExtensionPointSchema>();
                    extensionPoint2.Name = "ep2";

                    var prop2 = extensionPoint2.Create<PropertySchema>();
                    prop2.Name = "ContractProperty2";
                    prop2.Type = typeof(string).Name;
                });

                this.extensionPoints = new List<IExtensionPointSchema> { extensionPoint1, extensionPoint2 };

                this.descriptor = new ProvidedExtensionPointsPropertyDescriptor(
                                this.patternSchema,
                                extensionPoints,
                                new Mock<IUserMessageService>().Object,
                                "ImplementedExtensionPointsRaw",
                                string.Empty);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValue_ThenSetsImplementedExtensionPointsAndVariableProperties()
            {
                this.descriptor.SetValue(null, this.extensionPoints);

                Assert.Equal(2, this.patternSchema.ProvidedExtensionPoints.Count());

                var extensionPoint1 = this.patternSchema.ProvidedExtensionPoints[0];
                Assert.Equal("ToolkitId.Pattern1.View1.ep1", extensionPoint1.ExtensionPointId);

                var extensionPoint2 = this.patternSchema.ProvidedExtensionPoints[1];
                Assert.Equal("ToolkitId.Pattern1.View1.ep2", extensionPoint2.ExtensionPointId);

                Assert.Equal(2, this.patternSchema.Properties.Count());

                var property1 = this.patternSchema.Properties[0];
                Assert.Equal("ContractProperty1", property1.Name);
                Assert.Equal(typeof(string).Name, property1.Type);

                var property2 = this.patternSchema.Properties[1];
                Assert.Equal("ContractProperty2", property2.Name);
                Assert.Equal(typeof(string).Name, property2.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenReturnsValue()
            {
                this.descriptor.SetValue(null, this.extensionPoints);
                Assert.NotNull(this.descriptor.GetValue(null));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenTwoRegisteredExtensionsWithEqualVariablePropertyNamesAndTypes
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema patternSchema;
            private ProvidedExtensionPointsPropertyDescriptor descriptor;
            private List<IExtensionPointSchema> extensionPoints;

            [TestInitialize]
            public void InitializeContext()
            {
                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                ExtensionPointSchema extensionPoint1 = null;
                ExtensionPointSchema extensionPoint2 = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();

                    var uriService = Mock.Of<IUriReferenceService>(
                        u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                            p.ToolkitInfo.Identifier == "ToolkitId"));

                    this.patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    this.patternSchema.PatternLink = "patternmanager://foo";
                    this.patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";

                    extensionPoint1 = view.Create<ExtensionPointSchema>();
                    extensionPoint1.Name = "ep1";

                    var prop1 = extensionPoint1.Create<PropertySchema>();
                    prop1.Name = "ContractProperty1";
                    prop1.Type = typeof(string).Name;

                    extensionPoint2 = view.Create<ExtensionPointSchema>();
                    extensionPoint2.Name = "ep2";

                    var prop2 = extensionPoint2.Create<PropertySchema>();
                    prop2.Name = "ContractProperty1";
                    prop2.Type = typeof(string).Name;
                });

                this.extensionPoints = new List<IExtensionPointSchema> { extensionPoint1, extensionPoint2 };

                this.descriptor = new ProvidedExtensionPointsPropertyDescriptor(
                                this.patternSchema,
                                extensionPoints,
                                new Mock<IUserMessageService>().Object,
                                "ImplementedExtensionPointsRaw",
                                string.Empty);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValue_ThenSetsImplementedExtensionPointsAndVariableProperties()
            {
                this.descriptor.SetValue(null, this.extensionPoints);

                Assert.Equal(2, this.patternSchema.ProvidedExtensionPoints.Count());

                var extensionPoint1 = this.patternSchema.ProvidedExtensionPoints[0];
                Assert.Equal("ToolkitId.Pattern1.View1.ep1", extensionPoint1.ExtensionPointId);

                var extensionPoint2 = this.patternSchema.ProvidedExtensionPoints[1];
                Assert.Equal("ToolkitId.Pattern1.View1.ep2", extensionPoint2.ExtensionPointId);

                Assert.Equal(1, this.patternSchema.Properties.Count());

                var property1 = this.patternSchema.Properties[0];
                Assert.Equal("ContractProperty1", property1.Name);
                Assert.Equal(typeof(string).Name, property1.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenReturnsValue()
            {
                this.descriptor.SetValue(null, this.extensionPoints);
                Assert.NotNull(this.descriptor.GetValue(null));
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        [TestClass]
        public class GivenTwoRegisteredExtensionsWithEqualVariablePropertyNamesAndDifferentTypes
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema patternSchema;
            private ProvidedExtensionPointsPropertyDescriptor descriptor;
            private List<IExtensionPointSchema> extensionPoints;

            [TestInitialize]
            public void InitializeContext()
            {
                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                ExtensionPointSchema extensionPoint1 = null;
                ExtensionPointSchema extensionPoint2 = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();

                    var uriService = Mock.Of<IUriReferenceService>(
                        u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                            p.ToolkitInfo.Identifier == "ToolkitId"));

                    this.patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    this.patternSchema.PatternLink = "patternmanager://foo";
                    this.patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";

                    extensionPoint1 = view.Create<ExtensionPointSchema>();
                    extensionPoint1.Name = "ep1";

                    var prop1 = extensionPoint1.Create<PropertySchema>();
                    prop1.Name = "ContractProperty1";
                    prop1.Type = typeof(string).Name;

                    extensionPoint2 = view.Create<ExtensionPointSchema>();
                    extensionPoint2.Name = "ep2";

                    var prop2 = extensionPoint2.Create<PropertySchema>();
                    prop2.Name = "ContractProperty1";
                    prop2.Type = typeof(double).Name;
                });

                this.extensionPoints = new List<IExtensionPointSchema> { extensionPoint1, extensionPoint2 };

                this.descriptor = new ProvidedExtensionPointsPropertyDescriptor(
                                this.patternSchema,
                                extensionPoints,
                                new Mock<IUserMessageService>().Object,
                                "ImplementedExtensionPointsRaw",
                                string.Empty);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValue_ThenSetsImplementedExtensionPointsAndVariableProperties()
            {
                this.descriptor.SetValue(null, this.extensionPoints);

                Assert.Equal(1, this.patternSchema.ProvidedExtensionPoints.Count());

                var extensionPoint1 = this.patternSchema.ProvidedExtensionPoints[0];
                Assert.Equal("ToolkitId.Pattern1.View1.ep1", extensionPoint1.ExtensionPointId);

                Assert.Equal(1, this.patternSchema.Properties.Count());

                var property1 = this.patternSchema.Properties[0];
                Assert.Equal("ContractProperty1", property1.Name);
                Assert.Equal(typeof(string).Name, property1.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingValue_ThenReturnsValue()
            {
                this.descriptor.SetValue(null, this.extensionPoints);
                Assert.NotNull(this.descriptor.GetValue(null));
            }
        }
    }
}