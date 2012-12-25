using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class CustomizableSettingSchemaSpec
    {
        private const string TestPropertyId = "TestPropertyId";

        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenASetting
        {
            private CustomizableSettingSchema setting;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.setting = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsEnabledIsTrue()
            {
                Assert.True(this.setting.IsEnabled);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenPropertyIdReturnsEmpty()
            {
                Assert.Equal(string.Empty, this.setting.PropertyId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenCaptionReturnsDefaultNameCaption()
            {
                Assert.Equal(this.setting.Caption,
                    string.Format(CultureInfo.CurrentCulture, this.setting.CaptionFormatter, Properties.Resources.CustomizableSetting_DefaultCaption));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenDescriptionReturnsDefaultNameDescription()
            {
                Assert.Equal(this.setting.Description,
                    string.Format(CultureInfo.CurrentCulture, this.setting.DescriptionFormatter, Properties.Resources.CustomizableSetting_DefaultCaption));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenDefaultValueIsTrue()
            {
                Assert.True(this.setting.DefaultValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenValueIsDefaultValue()
            {
                Assert.Equal(this.setting.Value, this.setting.DefaultValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsModifiedIsFalse()
            {
                Assert.False(this.setting.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenValueChanges_ThenIsModifiedIsTrue()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = !this.setting.DefaultValue);

                Assert.True(this.setting.IsModified);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenADisabledSetting
        {
            private CustomizableSettingSchema setting;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.setting = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.setting.Disable();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisabled_ThenIsEnabledIsFalse()
            {
                Assert.False(this.setting.IsEnabled);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisabled_ThenIsModifiedIsFalse()
            {
                Assert.False(this.setting.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisabled_ThenValueIsFalse()
            {
                Assert.False(this.setting.Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenChangingValueFromDefault_ThenIsModifiedRemainsFalse()
            {
                this.setting.WithTransaction(s => s.Value = !s.DefaultValue);

                Assert.False(this.setting.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisabledAndValueChanged_ThenValueIsSameValue()
            {
                bool orignalValue = this.setting.Value;
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = !orignalValue);

                Assert.Equal(this.setting.Value, orignalValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDisabledAndDefaultValueChanged_TheDefaultValueIsSameDefaultValue()
            {
                bool orignalDefaultValue = this.setting.DefaultValue;
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.DefaultValue = !orignalDefaultValue);

                Assert.Equal(this.setting.DefaultValue, orignalDefaultValue);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenASettingWithAPropertyId
        {
            private CustomizableSettingSchema setting;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.setting = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.setting.PropertyId = TestPropertyId;
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenPropertyIdReturnsPropertyId()
            {
                Assert.Equal(TestPropertyId, this.setting.PropertyId);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCaptionReturnsCaption()
            {
                Assert.Equal(this.setting.Caption,
                    string.Format(CultureInfo.CurrentCulture, this.setting.CaptionFormatter, this.setting.GetPropertyIdDisplayName()));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDescriptionReturnsDescription()
            {
                Assert.Equal(this.setting.Description,
                    string.Format(CultureInfo.CurrentCulture, this.setting.DescriptionFormatter, this.setting.GetPropertyIdDisplayName()));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenASettingWithDefaultValueIsFalse
        {
            private CustomizableSettingSchema setting;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.setting = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.setting.DefaultValue = false;
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDefaultValueIsFalse_ThenDefaultValueIsFalse()
            {
                Assert.False(this.setting.DefaultValue);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDefaultValueIsFalse_ThenIsModifiedIsTrue()
            {
                Assert.True(this.setting.IsModified);
            }
        }
    }
}