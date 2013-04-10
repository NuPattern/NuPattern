using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.UnitTests.Automation.Event
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
        }
    }
}