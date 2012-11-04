using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
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
