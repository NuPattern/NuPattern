using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class PatternSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAPattern
        {
            private PatternSchema pattern;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.pattern = this.store.ElementFactory.CreateElement<PatternSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                Assert.Equal(this.pattern.Name, this.pattern.SchemaPath);
            }
        }
    }
}
