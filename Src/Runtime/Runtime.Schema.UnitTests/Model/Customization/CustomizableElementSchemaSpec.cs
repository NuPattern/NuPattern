using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    public partial class CustomizableElementSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();
        private const string TestCaption = "TestCaption";

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public partial class GivenACustomizableElement
        {
            private CustomizableElementSchema element = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.IsCustomizable = CustomizationState.True;
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsEnabledIsTrue()
            {
                Assert.True(this.element.IsCustomizationEnabled);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenPolicyIsNotNull()
            {
                Assert.NotNull(this.element.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsPolicyModifyableIsTrue()
            {
                Assert.True(this.element.IsCustomizationPolicyModifyable);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public partial class GivenADisabledCustomizableElementWithSettings
        {
            private CustomizableElementSchema element = null;
            private CustomizableSettingSchema setting;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.setting = this.element.Policy.Create<CustomizableSettingSchema>();
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.IsCustomizable = CustomizationState.True;
                    this.element.DisableCustomization();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsEnabledIsFalse()
            {
                Assert.False(this.element.IsCustomizationEnabled);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsCustomizableIsFalse()
            {
                Assert.Equal(this.element.IsCustomizable, CustomizationState.False);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsPolicyModifyableIsFalse()
            {
                Assert.False(this.element.IsCustomizationPolicyModifyable);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableInherited_ThenIsCustomizableIsFalse()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.element.IsCustomizable = CustomizationState.Inherited);

                Assert.Equal(this.element.IsCustomizable, CustomizationState.False);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableTrue_ThenIsCustomizableIsFalse()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.element.IsCustomizable = CustomizationState.True);

                Assert.Equal(this.element.IsCustomizable, CustomizationState.False);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAllSettingsDisabled()
            {
                Assert.False(this.setting.IsEnabled);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public partial class GivenACustomizableElementWithNoParent
        {
            private CustomizableElementSchema element = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.IsCustomizable = CustomizationState.True;
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsCustomizableIsTrue()
            {
                Assert.Equal(this.element.IsCustomizable, CustomizationState.True);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsCustomizableInherited_ThenIsCustomizableIsTrue()
            {
                this.store.TransactionManager.DoWithinTransaction(
                    () => this.element.IsCustomizable = CustomizationState.Inherited);

                Assert.Equal(this.element.IsCustomizable, CustomizationState.True);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public partial class GivenADelegatingElementWithParentWhereParentIsPolicyIsTrue
        {
            private ElementSchema childElement = null;
            private ElementSchema parentElement;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.IsCustomizable = CustomizationState.True;
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.childElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.Elements.Add(this.childElement);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsCustomizableIsInherit()
            {
                Assert.Equal(this.childElement.IsCustomizable, CustomizationState.Inherited);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenParentReturnsParentElement()
            {
                Assert.NotNull(this.childElement.Owner);
                Assert.Equal(this.childElement.Owner, this.parentElement);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenPolicyModifyableIsTrue()
            {
                Assert.True(this.childElement.IsCustomizationPolicyModifyable);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public partial class GivenADelegatingElementWithParentWhereParentIsPolicyIsFalse
        {
            private ElementSchema childElement = null;
            private ElementSchema parentElement;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.IsCustomizable = CustomizationState.False;
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.childElement = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.parentElement.Elements.Add(this.childElement);
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsCustomizableIsInherit()
            {
                Assert.Equal(this.childElement.IsCustomizable, CustomizationState.Inherited);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenParentReturnsParentElement()
            {
                Assert.NotNull(this.childElement.Owner);
                Assert.Equal(this.childElement.Owner, this.parentElement);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenPolicyModifyableIsFalse()
            {
                Assert.False(this.childElement.IsCustomizationPolicyModifyable);
            }
        }
    }
}