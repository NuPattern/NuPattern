using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
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

            [TestMethod]
            public void WhenConstructed_ThenIsEnabledIsTrue()
            {
                Assert.True(this.setting.IsEnabled);
            }

            [TestMethod]
            public void WhenConstructed_ThenPropertyIdReturnsEmpty()
            {
                Assert.Equal(string.Empty, this.setting.PropertyId);
            }

            [TestMethod]
            public void WhenConstructed_ThenCaptionReturnsDefaultNameCaption()
            {
                Assert.Equal(this.setting.Caption,
                    string.Format(CultureInfo.CurrentCulture, this.setting.CaptionFormatter, Properties.Resources.CustomizableSetting_DefaultCaption));
            }

            [TestMethod]
            public void WhenConstructed_ThenDescriptionReturnsDefaultNameDescription()
            {
                Assert.Equal(this.setting.Description,
                    string.Format(CultureInfo.CurrentCulture, this.setting.DescriptionFormatter, Properties.Resources.CustomizableSetting_DefaultCaption));
            }

            [TestMethod]
            public void WhenConstructed_ThenDefaultValueIsTrue()
            {
                Assert.True(this.setting.DefaultValue);
            }

            [TestMethod]
            public void WhenConstructed_ThenValueIsDefaultValue()
            {
                Assert.Equal(this.setting.Value, this.setting.DefaultValue);
            }

            [TestMethod]
            public void WhenConstructed_ThenIsModifiedIsFalse()
            {
                Assert.False(this.setting.IsModified);
            }

            [TestMethod]
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

            [TestMethod]
            public void WhenDisabled_ThenIsEnabledIsFalse()
            {
                Assert.False(this.setting.IsEnabled);
            }

            [TestMethod]
            public void WhenDisabled_ThenIsModifiedIsFalse()
            {
                Assert.False(this.setting.IsModified);
            }

            [TestMethod]
            public void WhenDisabled_ThenValueIsFalse()
            {
                Assert.False(this.setting.Value);
            }

            [TestMethod]
            public void WhenChangingValueFromDefault_ThenIsModifiedRemainsFalse()
            {
                this.setting.WithTransaction(s => s.Value = !s.DefaultValue);

                Assert.False(this.setting.IsModified);
            }

            [TestMethod]
            public void WhenDisabledAndValueChanged_ThenValueIsSameValue()
            {
                bool orignalValue = this.setting.Value;
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = !orignalValue);

                Assert.Equal(this.setting.Value, orignalValue);
            }

            [TestMethod]
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

            [TestMethod]
            public void ThenPropertyIdReturnsPropertyId()
            {
                Assert.Equal(TestPropertyId, this.setting.PropertyId);
            }

            [TestMethod]
            public void ThenCaptionReturnsCaption()
            {
                Assert.Equal(this.setting.Caption,
                    string.Format(CultureInfo.CurrentCulture, this.setting.CaptionFormatter, this.setting.GetPropertyIdDisplayName()));
            }

            [TestMethod]
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

            [TestMethod]
            public void WhenDefaultValueIsFalse_ThenDefaultValueIsFalse()
            {
                Assert.False(this.setting.DefaultValue);
            }

            [TestMethod]
            public void WhenDefaultValueIsFalse_ThenIsModifiedIsTrue()
            {
                Assert.True(this.setting.IsModified);
            }
        }
    }
}