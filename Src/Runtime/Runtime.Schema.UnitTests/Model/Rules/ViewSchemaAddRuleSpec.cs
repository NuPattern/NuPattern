using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Immutability;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class ViewSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewView
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternModelSchema patternModel;
            private PatternSchema product;
            private ViewSchema view;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = this.patternModel.Create<PatternSchema>();
                    this.view = this.product.Create<ViewSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFirstViewInAProduct_ThenIsDefaultAndIsLocked()
            {
                Assert.True(this.view.IsDefault);
                Assert.True(this.view.IsLocked(Locks.Delete));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingTheSecondViewInAProduct_ThenIsNotDefaultAndIsNotLocked()
            {
                var view2 = this.product.Create<ViewSchema>();

                Assert.False(view2.IsDefault);
                Assert.False(view2.IsLocked(Locks.Delete));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingANewView_ThenCustomizationPolicyIsNotNull()
            {
                Assert.NotNull(this.view.Policy);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingANewView_ThenCustomizationPolicyHasRequiredSettings()
            {
                IEnumerable<CustomizableSettingSchema> settings = this.view.Policy.Settings;

                Assert.Equal(4, settings.Count());
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.view.GetPropertyName(element => element.IsVisible)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.view.GetPropertyName(element => element.IsDefault)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.view.GetPropertyName(element => element.Description)));
                Assert.NotNull(settings.FirstOrDefault(setting => setting.PropertyId == this.view.GetPropertyName(element => element.DisplayName)));
            }
        }
    }
}