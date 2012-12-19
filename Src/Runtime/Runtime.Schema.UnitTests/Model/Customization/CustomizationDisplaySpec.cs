using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class CustomizationDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public abstract class GivenAPatternModelWithAProperty
        {
            protected PropertySchema Property { get; set; }

            protected PatternModelSchema PatternModel { get; set; }

            [TestInitialize]
            public virtual void InitializeContext()
            {
                var uriService = Mock.Of<Microsoft.VisualStudio.TeamArchitect.PowerTools.IFxrUriReferenceService>(
                    u => u.ResolveUri<IInstanceBase>(It.IsAny<Uri>()) == Mock.Of<IProduct>(p =>
                    p.ToolkitInfo.Identifier == "ToolkitId"));

                var store = new DslTestStore<PatternModelDomainModel>();

                var serviceProvider = Mock.Get(store.ServiceProvider);
                serviceProvider.Setup(s => s.GetService(typeof(IPatternManager))).Returns(
                    Mock.Of<IPatternManager>(p => p.InstalledToolkits == new List<IInstalledToolkitInfo>()));

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.PatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = this.PatternModel.Create<PatternSchema>();
                    pattern.Name = "WebService";
                    pattern.PatternLink = "patternmanager://foo";
                    pattern.UriService = uriService;

                    this.Property = pattern.Create<PropertySchema>();
                    this.Property.Name = "Namespace";
                });
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredPattern : GivenAPatternModelWithAProperty
        {
            [TestMethod, TestCategory("Unit")]
            public void ThenIsCustomizableIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, pattern => pattern.IsCustomizable);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCustomizationPolicyIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, element => element.Policy);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCustomizationPolicyDisplaysCorrectly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, element => element.Policy);
                string category = this.PatternModel.Pattern.Policy.GetDomainClass().ImplementationClass.Category(true);

                Assert.Equal(category, descriptor.Category);
                Assert.Equal(descriptor.Converter.ConvertToString(this.PatternModel.Pattern.Policy),
                    string.Format(CultureInfo.CurrentCulture, CustomizableElementTypeDescriptor.PolicyDisplayedValueFormatter,
                        this.PatternModel.Pattern.Policy.CustomizationLevel));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAllSettingsAreBrowsableAndNotReadOnly()
            {
                var settingsDescriptors = TypeDescriptor.GetProperties(this.PatternModel.Pattern.Policy)
                    .Cast<PropertyDescriptor>()
                    .Where(descriptor => descriptor.Name != this.PatternModel.Pattern.Policy.GetPropertyName(policy => policy.Settings));

                Assert.True(settingsDescriptors.All(descr => descr.IsBrowsable));
                Assert.False(settingsDescriptors.All(descr => descr.IsReadOnly));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableChangedToFalse_ThenAllSettingsBecomesReadOnly()
            {
                this.PatternModel.Pattern.WithTransaction(pattern => pattern.IsCustomizable = CustomizationState.False);

                var settingsDescriptors = TypeDescriptor.GetProperties(this.PatternModel.Pattern.Policy)
                    .Cast<PropertyDescriptor>()
                    .Where(descriptor => descriptor.Name != this.PatternModel.Pattern.Policy.GetPropertyName(policy => policy.Settings));

                Assert.True(settingsDescriptors.All(descriptor => descriptor.IsBrowsable));
                Assert.True(settingsDescriptors.All(descriptor => descriptor.IsReadOnly));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenATailoredPatternWithAProperty : GivenAPatternModelWithAProperty
        {
            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel = PatternModelSpec.TailorPatternModel(this.PatternModel, new Version("1.0.0.0"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingIsDisabled_ThenReferredToElementPropertyIsReadOnly()
            {
                var setting = this.PatternModel.Pattern.Policy.Settings.First();
                var descriptor = TypeDescriptor.GetProperties(this.PatternModel.Pattern)[setting.PropertyId];

                Assert.False(descriptor.IsReadOnly);

                setting.WithTransaction(set => set.Disable());

                descriptor = TypeDescriptor.GetProperties(this.PatternModel.Pattern)[setting.PropertyId];
                Assert.True(descriptor.IsReadOnly);
            }
        }

        [TestClass]
        public class GivenATailoredPatternWithADisabledCustomizedSetting : GivenAPatternModelWithAProperty
        {
            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel = PatternModelSpec.TailorPatternModel(this.PatternModel, new Version("1.0.0.0"));

                var setting = this.Property.Policy.Settings
                    .First(s => s.PropertyId == Reflector<PropertySchema>.GetProperty(prop => prop.RawDefaultValue).Name);

                setting.WithTransaction(s => s.Disable());
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSettingIsReadOnlyForTailor()
            {
                var policyDescriptor = TypedDescriptor.GetProperty(this.Property, pattern => pattern.Policy);
                var policySettings = policyDescriptor.GetChildProperties(this.Property.Policy);
                var settingDescriptor = policySettings.Cast<PropertyDescriptor>().First(descriptor => descriptor.DisplayName.Contains("Default"));

                Assert.True(settingDescriptor.IsReadOnly);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenATailoredPatternWithCustomizationDisabled : GivenAPatternModelWithAProperty
        {
            [TestInitialize]
            public override void InitializeContext()
            {
                base.InitializeContext();

                this.PatternModel = PatternModelSpec.TailorPatternModel(this.PatternModel, new Version("1.0.0.0"));
                this.PatternModel.Pattern.WithTransaction(pattern => pattern.DisableCustomization());
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsCustomizableIsReadOnlyAndFalse()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, pattern => pattern.IsCustomizable);

                Assert.True(descriptor.IsReadOnly);
                Assert.True(this.PatternModel.Pattern.IsCustomizable == CustomizationState.False);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCustomizationPolicyIsNotAvailable()
            {
                var descriptor = TypedDescriptor.GetProperty(this.PatternModel.Pattern, element => element.Policy);

                Assert.NotNull(descriptor);
                Assert.False(descriptor.IsBrowsable);
            }
        }
    }
}