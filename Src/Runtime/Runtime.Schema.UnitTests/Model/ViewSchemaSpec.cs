using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public class ViewSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAView
        {
            private PatternSchema product;
            private ViewSchema view;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = patternModel.Create<PatternSchema>();
                    this.view = this.product.Create<ViewSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                var expected = string.Concat(this.product.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.view.Name);
                Assert.Equal(expected, this.view.SchemaPath);
            }
        }
    }
}
