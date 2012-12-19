using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Runtime.Store;

namespace NuPattern.Runtime.UnitTests
{
    [TestClass]
    public class ProductStateSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenProductInstantiated_ThenRaisesInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            IInstanceBase instantiated = null;
            IProduct product = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product = productState.CreateProduct();
            });

            Assert.NotNull(instantiated);
            Assert.Same(product, instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenEventsAreRaised_ThenStoreIsInATransaction()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            IProduct product = null;
            bool inTransaction = false;

            productState.ElementInstantiated += (sender, args) => inTransaction = store.Store.TransactionActive;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product = productState.CreateProduct();
            });

            Assert.True(inTransaction);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenNestedTransactionNotCommited_ThenNestedChangesNotCommited()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IProduct product = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                product = productState.CreateProduct();
            });

            var committed = false;
            store.Store.EventManagerDirectory.TransactionCommitted.Add(new EventHandler<TransactionCommitEventArgs>(
                (sender, args) => committed = true));

            var parentTx = store.TransactionManager.BeginTransaction();

            product.DefinitionName = "foo";

            var nestedTx = store.TransactionManager.BeginTransaction();

            product.InstanceName = "bar";

            nestedTx.Dispose();

            Assert.NotEqual("bar", product.InstanceName);

            parentTx.Commit();

            Assert.True(committed);
            Assert.Equal("foo", product.DefinitionName);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProductInstantiated_ThenRetrievesProductInstance()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IProduct product = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                product = productState.CreateProduct();
            });

            Assert.Equal(1, productState.FindAll<Product>().Count());
            Assert.Equal(product.Id, productState.FindAll<Product>().ElementAt(0).Id);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenViewInstantiated_ThenRaisesInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IProduct product = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                product = productState.CreateProduct();
            });

            IInstanceBase instantiated = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product.CreateView();
            });

            Assert.NotNull(instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenElementInstantiated_ThenRaisesInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IView view = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                var product = productState.CreateProduct();
                view = product.CreateView();
            });

            IInstanceBase instantiated = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                view.CreateElement();
            });

            Assert.NotNull(instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCollectionInstantiated_ThenRaisesInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IView view = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                var product = productState.CreateProduct();
                view = product.CreateView();
            });

            IInstanceBase instantiated = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                view.CreateCollection();
            });

            Assert.NotNull(instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPropertyInstantiated_ThenRaisesInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IProduct product = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                product = productState.CreateProduct();
            });

            IInstanceBase instantiated = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product.CreateProperty();
            });

            Assert.NotNull(instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProductInstantiatedWhileSerializing_ThenDoesNotRaiseInstantiatedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            store.Store.PropertyBag[ProductState.IsSerializingKey] = true;
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            IInstanceBase instantiated = null;
            IProduct product = null;

            productState.ElementInstantiated += (sender, args) => instantiated = args.Value;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product = productState.CreateProduct();
            });

            Assert.Null(instantiated);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProductInstantiatedWhileSerializing_ThenDoesNotRaisePropertyChangeEvents()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            store.Store.PropertyBag[ProductState.IsSerializingKey] = true;
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            bool changed = false;
            IProduct product = null;

            productState.PropertyChanged += (sender, args) => changed = true;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                product = productState.CreateProduct();
            });

            Assert.False(changed);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenElementInstantiated_ThenDoesNotRaisePropertyChangedEventsFromInitializerSetters()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;
            IView view = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
                var product = productState.CreateProduct();
                view = product.CreateView();
            });

            bool changed = false;
            productState.PropertyChanged += (sender, args) => changed = true;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                view.CreateElement(e => e.InstanceName = "Foo");
            });

            Assert.False(changed);
        }


        [TestMethod, TestCategory("Unit")]
        public void WhenTransactionCommited_ThenRaisesTransactionCommitedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            var fired = false;
            productState.TransactionCommited += (sender, args) => fired = true;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState.CreateProduct();
            });

            Assert.True(fired);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTransactionCommitedWhileSerializing_ThenDoesNotRaiseTransactionCommitedEvent()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            store.Store.PropertyBag[ProductState.IsSerializingKey] = true;
            IProductState productState = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState = store.ElementFactory.CreateElement<ProductState>();
            });

            var fired = false;
            productState.TransactionCommited += (sender, args) => fired = true;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                productState.CreateProduct();
            });

            Assert.False(fired);
        }
    }
}