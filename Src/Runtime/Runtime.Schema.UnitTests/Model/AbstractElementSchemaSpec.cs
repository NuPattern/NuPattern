using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class AbstractElementSchemaSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenAnElementAndCollection
        {
            private PatternSchema product;
            private ViewSchema view;
            private ElementSchema element;
            private CollectionSchema collection;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = patternModel.Create<PatternSchema>();
                    this.view = this.product.Create<ViewSchema>();
                    this.element = this.view.Create<ElementSchema>();
                    this.collection = this.view.Create<CollectionSchema>();
                });
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSchemaPathIsValid()
            {
                var expectedElement = string.Concat(this.view.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.element.Name);
                var expectedCollection = string.Concat(this.view.SchemaPath, NamedElementSchema.SchemaPathDelimiter, this.collection.Name);
                Assert.Equal(expectedElement, this.element.SchemaPath);
                Assert.Equal(expectedCollection, this.collection.SchemaPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenOrderIsDefault()
            {
                Assert.Equal(1d, this.element.OrderGroup);
            }
        }
    }
}