using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Modeling.Immutability;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
    public class ViewSchemaChangeRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANewView
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternModelSchema patternModel;
            private PatternSchema product;
            private ViewSchema view;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.product = this.patternModel.Create<PatternSchema>();
                    this.view = this.product.Create<ViewSchema>();
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsDefaultIsTrue_ThenOtherViewsAreSetToFalseAndNotLocked()
            {
                this.view.WithTransaction(vw => vw.IsDefault = true);

                var view2 = this.product.Create<ViewSchema>();
                view2.WithTransaction(vw => vw.IsDefault = true);

                Assert.False(this.view.IsDefault);
                Assert.False(this.view.IsLocked(Locks.Delete));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSecondViewIsDefaultIsTrue_ThenSecondViewIsLocked()
            {
                var view2 = this.product.Create<ViewSchema>();
                view2.WithTransaction(vw => vw.IsDefault = true);

                Assert.True(view2.IsLocked(Locks.Delete));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsDefaultIsFalse_ThenIsDefaultIsTrue()
            {
                this.view.WithTransaction(vw => vw.IsDefault = false);

                Assert.True(this.view.IsDefault);
                Assert.True(this.view.IsLocked(Locks.Delete));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSecondViewIsDefaultIsFalse_ThenFirstViewIsDefaultIsTrue()
            {
                var view2 = this.product.Create<ViewSchema>();
                view2.WithTransaction(vw => vw.IsDefault = false);

                this.view.WithTransaction(vw => vw.IsDefault = false);

                Assert.True(this.view.IsDefault);
                Assert.True(this.view.IsLocked(Locks.Delete));
            }
        }
    }
}