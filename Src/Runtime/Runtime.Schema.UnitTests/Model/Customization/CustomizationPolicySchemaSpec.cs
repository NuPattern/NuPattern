using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class CustomizationPolicySchemaSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenNoSettings
        {
            private CustomizationPolicySchema policy = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.policy = this.store.ElementFactory.CreateElement<CustomizationPolicySchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenNoSettings()
            {
                Assert.True(this.policy.Settings.Count() == 0);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsModifiedIsFalse()
            {
                Assert.False(this.policy.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenCustomizedLevelIsNone()
            {
                Assert.Equal(this.policy.CustomizationLevel, CustomizedLevel.None);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenASingleSetting
        {
            private CustomizableSettingSchema setting = null;
            private CustomizationPolicySchema policy = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.policy = this.store.ElementFactory.CreateElement<CustomizationPolicySchema>();
                    this.setting = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.policy.Settings.Add(this.setting);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenSingleSetting()
            {
                Assert.NotNull(this.policy.Settings);
                Assert.True(this.policy.Settings.Count() == 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueChanged_ThenIsModifiedIsTrue()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = !this.setting.DefaultValue);

                Assert.True(this.policy.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueReset_ThenIsModifiedIsFalse()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = this.setting.DefaultValue);

                Assert.False(this.policy.IsModified);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueIsFalse_ThenCustomizedLevelIsNone()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = false);

                Assert.Equal(CustomizedLevel.None, this.policy.CustomizationLevel);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingIsDisabled_ThenCustomizedLevelIsNone()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = false);
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Disable());

                Assert.Equal(CustomizedLevel.None, this.policy.CustomizationLevel);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingValueIsTrue_ThenCustomizedLevelIsAll()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting.Value = true);

                Assert.Equal(CustomizedLevel.All, this.policy.CustomizationLevel);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenMultipleSettings
        {
            private CustomizableSettingSchema setting1 = null;
            private CustomizableSettingSchema setting2 = null;
            private CustomizationPolicySchema policy = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.policy = this.store.ElementFactory.CreateElement<CustomizationPolicySchema>();
                    this.setting1 = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.setting2 = this.store.ElementFactory.CreateElement<CustomizableSettingSchema>();
                    this.policy.Settings.Add(this.setting1);
                    this.policy.Settings.Add(this.setting2);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllSettingsAreReset_ThenCustomizedLevelIsNone()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting1.Value = false);
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting2.Value = false);

                Assert.Equal(this.policy.CustomizationLevel, CustomizedLevel.None);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAllSettingsAreSet_ThenCustomizedLevelIsAll()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting1.Value = true);
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting2.Value = true);

                Assert.Equal(this.policy.CustomizationLevel, CustomizedLevel.All);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSomeSettingsAreSet_ThenCustomizedLevelIsPartially()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting1.Value = true);
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.setting2.Value = false);

                Assert.Equal(this.policy.CustomizationLevel, CustomizedLevel.Partially);
            }
        }
    }
}