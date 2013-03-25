using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Modeling;
using NuPattern.Runtime.Extensibility;
using NuPattern.Runtime.Store;

namespace NuPattern.Runtime.UnitTests.Store
{
    [TestClass]
    public class InstanceBaseSpecs
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenARuntimeStore
        {
            internal ProductState ProductStore { get; private set; }
            internal DslTestStore<ProductStateStoreDomainModel> DslStore { get; private set; }

            [TestInitialize]
            public virtual void Initialize()
            {
                this.DslStore = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.DslStore.TransactionManager.BeginTransaction())
                {
                    this.ProductStore = this.DslStore.ElementFactory.CreateElement<ProductState>();
                    tx.Commit();
                }
            }

            [TestCleanup]
            public void CleanUp()
            {
                if (!this.DslStore.Store.Disposed)
                {
                    this.DslStore.Dispose();
                }
            }
        }

        [TestClass]
        public class GivenAProductWithElements : GivenARuntimeStore
        {
            private IProduct product;
            private IElement element;

            [TestInitialize]
            public override void Initialize()
            {
                base.Initialize();

                using (var tx = this.DslStore.TransactionManager.BeginTransaction())
                {
                    this.product = this.ProductStore.CreateProduct(p =>
                        {
                            p.CreateView(v =>
                                {
                                    v.CreateCollection(c =>
                                        {
                                            c.CreateElement(e => element = e.CreateElement());
                                            c.CreateElement();
                                        });
                                });
                        });

                    tx.Commit();
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTraversingRoot_ThenGetsOwningProduct()
            {
                var root = this.element.Root;

                Assert.NotNull(root);
                Assert.Same(this.product, root);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDeletingElement_ThenRaisesDeleteEvents()
            {
                var element = this.element.Root.Views.First().Elements.First();
                var deletingCalled = false;
                var deleteCalled = false;

                element.Deleting += (sender, args) => deletingCalled = true;
                element.Deleted += (sender, args) => deleteCalled = true;

                element.Delete();

                Assert.True(deleteCalled);
                Assert.True(deletingCalled);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "extension", Justification = "Test Code")]
            [TestMethod, TestCategory("Unit")]
            public void WhenTraversingFromExtensionPoint_ThenGetsOwningProduct()
            {
                ((Element)this.element).Info = Mock.Of<IElementInfo>(elei =>
                    elei.ExtensionPoints == new[] 
                    { 
                        Mock.Of<IExtensionPointInfo>(epi => epi.RequiredExtensionPointId == "Foo") 
                    });

                IElement extElement = null;
                var extension = this.element.CreateExtension(p =>
                {
                    p.CreateView(v =>
                        v.CreateCollection(c =>
                        {
                            c.CreateElement(e => extElement = e.CreateElement());
                            c.CreateElement();
                        })
                    );
                    ((Product)p).Info = Mock.Of<IPatternInfo>(pei =>
                        pei.ProvidedExtensionPoints == new[] 
                        { 
                            Mock.Of<IProvidedExtensionPointInfo>(epi => epi.ExtensionPointId == "Foo") 
                        });
                });

                var root = extElement.Root;

                Assert.NotNull(root);
                Assert.Same(this.product, root);
            }
        }
    }
}
