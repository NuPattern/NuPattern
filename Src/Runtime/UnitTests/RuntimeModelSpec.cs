using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;
using NuPattern.Runtime.Store;

namespace NuPattern.Runtime.UnitTests
{
    [TestClass]
    public class RuntimeModelSpec
    {
        [TestMethod, TestCategory("Unit")]
        public void WhenChangingProperty_ThenNotifiesSubscriber()
        {
            var store = new DslTestStore<ProductStateStoreDomainModel>();
            IProduct product = null;

            store.TransactionManager.DoWithinTransaction(() =>
            {
                var productStore = store.ElementFactory.CreateElement<ProductState>();
                product = productStore.Create<Product>();
            });

            var notified = false;

            product.SubscribeChanged(x => x.InstanceName, p => notified = true);

            product.InstanceName = "Hello";

            Assert.IsTrue(notified);
        }
    }
}
