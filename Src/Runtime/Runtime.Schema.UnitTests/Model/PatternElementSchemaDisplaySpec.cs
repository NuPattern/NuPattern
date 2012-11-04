using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    [TestClass]
    public class PatternElementSchemaDisplaySpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public abstract class GivenAPatternModel
        {
            protected PatternModelSchema PatternModel { get; set; }

            protected ElementSchema Element { get; set; }

            [TestInitialize]
            public virtual void InitializeContext()
            {
                var store = new DslTestStore<PatternModelDomainModel>();

                store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.PatternModel = store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = this.PatternModel.Create<PatternSchema>();
                    this.Element = pattern.Create<ElementSchema>();
                });
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAnAuthoredPattern : GivenAPatternModel
        {
            [TestMethod, TestCategory("Unit")]
            public void TheValidationRulesPropertyIsBrowsableAndNotReadOnly()
            {
                var descriptor = TypedDescriptor.GetProperty(this.Element, element => element.ValidationRules);

                Assert.True(descriptor.IsBrowsable);
                Assert.False(descriptor.IsReadOnly);
            }
        }
    }
}