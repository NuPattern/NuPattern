using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class AutomationSettingsSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAutomation
        {
            private AutomationSettingsSchema automation;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public virtual void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.automation = this.store.ElementFactory.CreateElement<AutomationSettingsSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenAutomationTypeNameIsEmpty()
            {
                Assert.Equal(string.Empty, this.automation.AutomationType);
            }
        }
    }
}
