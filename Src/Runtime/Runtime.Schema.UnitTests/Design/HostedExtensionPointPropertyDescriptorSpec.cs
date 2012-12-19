using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Immutability;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class HostedExtensionPointPropertyDescriptorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenAnAuthoredExtensionPoint
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private ExtensionPointSchema extensionPointSchema;
            private IExtensionPointSchema hostedExtensionPoint;
            private HostedExtensionPointPropertyDescriptor descriptor;
            private Mock<IUserMessageService> userMessageService;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "ToolkitId"));

                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                ExtensionPointSchema hosted = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    patternSchema.PatternLink = "patternmanager://foo";
                    patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";
                    this.extensionPointSchema = view.Create<ExtensionPointSchema>();
                    this.extensionPointSchema.Name = "ep1";

                    hosted = view.Create<ExtensionPointSchema>();
                    hosted.Name = "hostedExtensionPoint";

                    var prop1 = hosted.Create<PropertySchema>();
                    prop1.Name = "PartiallyCustomizable";
                    prop1.Type = "System.String";
                    prop1.IsCustomizable = CustomizationState.True;
                    prop1.EnsurePolicyAndDefaultSettings();
                    prop1.Policy.Settings[0].Value = false;

                    var prop2 = hosted.Create<PropertySchema>();
                    prop2.Name = "FullyCustomizable";
                    prop2.Type = "System.String";
                    prop2.IsCustomizable = CustomizationState.False;
                    prop2.EnsurePolicyAndDefaultSettings();
                });

                this.hostedExtensionPoint = hosted as IExtensionPointSchema;

                this.userMessageService = new Mock<IUserMessageService>();

                var defaultDescriptor = TypeDescriptor.CreateProperty(typeof(ExtensionPointSchema),
                    Reflector<ExtensionPointSchema>.GetPropertyName(e => e.RepresentedExtensionPointId), typeof(string), null);
                this.descriptor = new HostedExtensionPointPropertyDescriptor(
                                defaultDescriptor,
                                this.extensionPointSchema,
                                this.userMessageService.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingConverter_ThenReturnsExtensionPointsConverter()
            {
                Assert.True(this.descriptor.Converter.GetType() == typeof(ExtensionPointConverter));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingEditor_ThenReturnsMultipleValuesEditor()
            {
                Assert.True(this.descriptor.GetEditor(typeof(UITypeEditor)).GetType() == typeof(StandardValuesEditor));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingComponentType_ThenReturnsExtensionPointType()
            {
                Assert.True(this.descriptor.ComponentType == typeof(ExtensionPointSchema));
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
                this.extensionPointSchema.RepresentedExtensionPointId = "foo";

                Assert.Equal("foo", this.descriptor.GetValue(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingValue_ThenSetsRepresentationOfToNull()
            {
                this.extensionPointSchema.RepresentedExtensionPointId = "foo";

                this.descriptor.ResetValue(null);

                Assert.Equal(string.Empty, this.extensionPointSchema.RepresentedExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResettingValue_ThenDeletesAllProperties()
            {
                this.extensionPointSchema.RepresentedExtensionPointId = "foo";
                this.extensionPointSchema.Create<PropertySchema>();
                this.extensionPointSchema.Create<PropertySchema>();

                this.descriptor.ResetValue(null);

                Assert.Equal(0, this.extensionPointSchema.Properties.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueWithNull_ThenThrows()
            {
                Assert.Throws<ArgumentException>(() =>
                    this.descriptor.SetValue(null, null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueWithSameExtensionPointIdAsSelf_ShowWarning()
            {
                this.hostedExtensionPoint.Name = this.extensionPointSchema.Name;

                this.descriptor.SetValue(null, this.hostedExtensionPoint);

                this.userMessageService.Verify(msg => msg.ShowWarning(Properties.Resources.HostedExtensionPointPropertyDescriptor_CantSelfHost), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueWithNoExistingProperties_ThenSetsRepresentationOfToValue()
            {
                this.descriptor.SetValue(null, this.hostedExtensionPoint);

                this.userMessageService.Verify(msg => msg.PromptWarning(It.IsAny<string>()), Times.Never());

                Assert.Equal(this.hostedExtensionPoint.RequiredExtensionPointId, this.extensionPointSchema.RepresentedExtensionPointId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueWithNoExistingProperties_ThenAddsContractProperties()
            {
                this.descriptor.SetValue(null, this.hostedExtensionPoint);

                this.userMessageService.Verify(msg => msg.PromptWarning(It.IsAny<string>()), Times.Never());

                Assert.Equal(2, this.extensionPointSchema.Properties.Count());

                var property1 = this.extensionPointSchema.Properties[0];
                Assert.Equal(this.hostedExtensionPoint.Properties.First().Name, property1.Name);
                Assert.Equal(CustomizationState.True, property1.IsCustomizable);
                Assert.True(property1.IsCustomizationEnabled);
                Assert.True(property1.IsInheritedFromBase);
                Assert.True(property1.IsUsageExtensionPoint());
                Assert.Equal(Locks.Delete, property1.GetLocks());
                Assert.False(property1.Policy.Settings[0].Value);
                Assert.False(property1.Policy.Settings[0].IsEnabled);

                var property2 = this.extensionPointSchema.Properties[1];
                Assert.Equal(CustomizationState.False, property2.IsCustomizable);
                Assert.False(property2.IsCustomizationEnabled);
                Assert.False(property2.IsCustomizationEnabled);
                Assert.Equal(Locks.Delete, property2.GetLocks());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueWithExistingProperties_ThenDeletesAllProperties()
            {
                this.extensionPointSchema.Create<PropertySchema>();
                this.userMessageService.Setup(msg => msg.PromptWarning(Properties.Resources.HostedExtensionPointPropertyDescriptor_PropertiesWillBeDeleted))
                    .Returns(true);

                this.descriptor.SetValue(null, this.hostedExtensionPoint);

                Assert.Equal(2, this.extensionPointSchema.Properties.Count());

                this.userMessageService.Verify(msg => msg.PromptWarning(Properties.Resources.HostedExtensionPointPropertyDescriptor_PropertiesWillBeDeleted), Times.Once());
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenATailoredHostedExtensionPoint
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private ExtensionPointSchema extensionPointSchema;
            private IExtensionPointSchema hostedExtensionPoint;
            private HostedExtensionPointPropertyDescriptor descriptor;
            private Mock<IUserMessageService> userMessageService;

            [TestInitialize]
            public void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "ToolkitId"));

                var serviceProvider = Mock.Get(this.store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                PatternModelSchema patternModel = null;
                ExtensionPointSchema hosted = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                    patternSchema.PatternLink = "patternmanager://foo";
                    patternSchema.UriService = uriService;

                    var view = patternSchema.Create<ViewSchema>();
                    view.Name = "View1";
                    this.extensionPointSchema = view.Create<ExtensionPointSchema>();
                    this.extensionPointSchema.Name = "ep1";

                    hosted = view.Create<ExtensionPointSchema>();
                    hosted.Name = "hostedExtensionPoint";

                    var prop1 = hosted.Create<PropertySchema>();
                    prop1.Name = "PartiallyCustomizable";
                    prop1.Type = "System.String";
                    prop1.IsCustomizable = CustomizationState.True;
                    prop1.EnsurePolicyAndDefaultSettings();
                    prop1.Policy.Settings[0].Value = false;

                    var prop2 = hosted.Create<PropertySchema>();
                    prop2.Name = "FullyCustomizable";
                    prop2.Type = "System.String";
                    prop2.IsCustomizable = CustomizationState.False;
                    prop2.EnsurePolicyAndDefaultSettings();

                    this.hostedExtensionPoint = hosted as IExtensionPointSchema;
                });

                //Host the extension Point
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    // Add contract properties
                    this.extensionPointSchema.Store.TransactionManager.DoWithinTransaction(() =>
                    {
                        this.extensionPointSchema.ClearAllProperties();
                        this.extensionPointSchema.CopyHostedContractProperties(this.hostedExtensionPoint);
                        this.extensionPointSchema.RepresentedExtensionPointId = this.hostedExtensionPoint.RequiredExtensionPointId;
                    });
                });

                // Tailor Toolkit
                PatternModelSchema tailoredPatternModel = null;
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    tailoredPatternModel = PatternModelSpec.TailorPatternModel(patternModel, new Version("1.0.0.0"));
                });

                // Re-assign local variables
                this.extensionPointSchema = tailoredPatternModel.Store.ElementDirectory.AllElements.OfType<ExtensionPointSchema>().First(e => e.Name == "ep1");
                this.hostedExtensionPoint = tailoredPatternModel.Store.ElementDirectory.AllElements.OfType<ExtensionPointSchema>().First(e => e.Name == "hostedExtensionPoint");

                this.userMessageService = new Mock<IUserMessageService>();

                var defaultDescriptor = TypeDescriptor.CreateProperty(typeof(ExtensionPointSchema),
                    Reflector<ExtensionPointSchema>.GetPropertyName(e => e.RepresentedExtensionPointId), typeof(string), null);
                this.descriptor = new HostedExtensionPointPropertyDescriptor(
                                defaultDescriptor,
                                this.extensionPointSchema,
                                this.userMessageService.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingReadOnlyProperty_ThenReturnsTrue()
            {
                Assert.True(this.descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingCanResetValue_ThenReturnsFalse()
            {
                Assert.False(this.descriptor.CanResetValue(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingReadOnlyProperty_ThenReturnsFalse()
            {
                Assert.True(this.descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenContractPropertiesAreStillInheritedAndMaintainCustomization()
            {
                Assert.Equal(2, this.extensionPointSchema.Properties.Count());

                var property1 = this.extensionPointSchema.Properties[0];
                Assert.Equal(this.hostedExtensionPoint.Properties.First().Name, property1.Name);
                Assert.Equal(CustomizationState.Inherited, property1.IsCustomizable); // Fixed up by tailoring rules.
                Assert.True(property1.IsCustomizationEnabled);
                Assert.True(property1.IsInheritedFromBase);
                Assert.True(property1.IsUsageExtensionPoint());
                //Assert.Equal(Locks.Delete, property1.GetLocks());
                Assert.False(property1.Policy.Settings[0].Value);
                Assert.False(property1.Policy.Settings[0].IsEnabled);

                var property2 = this.extensionPointSchema.Properties[1];
                Assert.Equal(CustomizationState.False, property2.IsCustomizable);
                Assert.False(property2.IsCustomizationEnabled);
                Assert.False(property2.IsCustomizationEnabled);
                //Assert.Equal(Locks.Delete, property2.GetLocks());
            }
        }
    }
}