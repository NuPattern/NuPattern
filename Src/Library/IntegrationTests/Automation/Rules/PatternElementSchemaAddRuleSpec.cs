using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Library.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class PatternElementSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private DslTestStore<PatternModelDomainModel> store =
            new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));

        [TestMethod]
        public void WhenCreatingAnPatternElementSchema_ThenInjectsExtensionElements()
        {
            ElementSchema element = null;

            this.store.TransactionManager.DoWithinTransaction(() =>
            {
                element = this.store.ElementFactory.CreateElement<ElementSchema>();
            });

            Assert.True(element.GetExtensions<IGuidanceExtension>().Any());
            Assert.True(element.GetExtensions<IArtifactExtension>().Any());
        }

        [TestMethod]
        public void WhenLoadingAnPatternElementSchema_ThenInjectsExtensionElements()
        {
            ElementSchema element = null;

            this.store.TransactionManager.DoWithinTransaction(() =>
            {
                element = this.store.ElementFactory.CreateElement<ElementSchema>();
            },
            true);

            Assert.True(element.GetExtensions<IGuidanceExtension>().Any());
            Assert.True(element.GetExtensions<IArtifactExtension>().Any());
        }
    }
}