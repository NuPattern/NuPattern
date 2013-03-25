using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
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
