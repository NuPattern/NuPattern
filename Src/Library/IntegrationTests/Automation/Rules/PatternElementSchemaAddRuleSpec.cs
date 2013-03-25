using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Automation;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Schema;

namespace NuPattern.Library.IntegrationTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class PatternElementSchemaAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private DslTestStore<PatternModelDomainModel> store =
            new DslTestStore<PatternModelDomainModel>(typeof(LibraryDomainModel));

        [TestMethod, TestCategory("Integration")]
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

        [TestMethod, TestCategory("Integration")]
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