using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Conditions;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Library.UnitTests.Automation.Event
{
    public class EventSettingsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        public class GivenEventSettings
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));
            private EventSettings settings;

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var automationSettings = pattern.Create<AutomationSettingsSchema>();
                    this.settings = automationSettings.AddExtension<EventSettings>();
                });
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenConditionAreEmpty()
            {
                Assert.True(String.IsNullOrEmpty(this.settings.Conditions));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenFilterForCurrentElementIsFalse()
            {
                Assert.False(this.settings.FilterForCurrentElement);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFilterForCurrentElementIsTrue_ThenAddsFilteringCondition()
            {
                this.settings.WithTransaction(setting => setting.FilterForCurrentElement = true);

                var result = BindingSerializer.Deserialize<List<ConditionBindingSettings>>(this.settings.Conditions);

                Assert.Equal(1, result.Count);
                Assert.Equal(typeof(EventSenderMatchesElementCondition).FullName, result[0].TypeId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFilteringConditionExistsAndFilterForCurrentElementIsTrue_ThenDoesNotAddFilteringCondition()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.settings.Conditions = BindingSerializer.Serialize(
                        new[]
						{
							new ConditionBindingSettings
							{
								TypeId = typeof(EventSenderMatchesElementCondition).FullName
							}
						});
                    this.settings.FilterForCurrentElement = true;
                });

                var result = BindingSerializer.Deserialize<List<ConditionBindingSettings>>(this.settings.Conditions);

                Assert.Equal(1, result.Count);
                Assert.Equal(typeof(EventSenderMatchesElementCondition).FullName, result[0].TypeId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFilteringConditionExistsAndFilterForCurrentElementIsChangedToFalse_ThenRemovesFilteringCondition()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var condition = new ConditionBindingSettings
                    {
                        TypeId = typeof(EventSenderMatchesElementCondition).FullName
                    };

                    this.settings.Conditions = BindingSerializer.Serialize(new List<ConditionBindingSettings> { condition });
                    this.settings.FilterForCurrentElement = true;
                    this.settings.FilterForCurrentElement = false;
                });

                var result = BindingSerializer.Deserialize<List<ConditionBindingSettings>>(this.settings.Conditions);

                Assert.Equal(0, result.Count);
            }
        }
    }
}