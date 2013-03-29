using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NuPattern.Runtime.UnitTests
{
    [TestClass]
    public class PatternManagerExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingWithNullManager_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PatternManagerExtensions.Find(null, "foo"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingWithNullProductName_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PatternManagerExtensions.Find(new Mock<IPatternManager>().Object, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingWithEmptyProductName_ThenThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PatternManagerExtensions.Find(new Mock<IPatternManager>().Object, string.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingWithAClosedManager_ThenDoesNotFindElements()
        {
            var manager = new Mock<IPatternManager>();

            PatternManagerExtensions.Find(manager.Object, "foo");

            manager.Verify(x => x.Products, Times.Never());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingExistingProduct_ThenFindsIt()
        {
            var product = Mocks.Of<IProduct>().First(x => x.InstanceName == "Foo");
            var manager = Mocks.Of<IPatternManager>().First(x => x.Products == new[] { product } && x.IsOpen == true);

            var found = PatternManagerExtensions.Find(manager, "Foo");

            Assert.NotNull(found);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenFindingExistingProduct_ThenFindsItCaseInsensitively()
        {
            var product = Mocks.Of<IProduct>().First(x => x.InstanceName == "Foo");
            var manager = Mocks.Of<IPatternManager>().First(x => x.Products == new[] { product } && x.IsOpen == true);

            var found = PatternManagerExtensions.Find(manager, "foo");

            Assert.NotNull(found);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDeletingWithNullManager_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PatternManagerExtensions.Delete(null, "foo"));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDeletingWithNullProductName_ThenThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => PatternManagerExtensions.Delete(new Mock<IPatternManager>().Object, null));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDeleteWithEmptyProductName_ThenThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => PatternManagerExtensions.Delete(new Mock<IPatternManager>().Object, string.Empty));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDeletingNonExistingProduct_ThenReturnsFalse()
        {
            var manager = Mocks.Of<IPatternManager>().First(x => x.Products == new IProduct[0] && x.IsOpen == true);

            var result = PatternManagerExtensions.Delete(manager, "foo");

            Assert.False(result);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenDeletingExistingProduct_ThenDeletesInstanceFromManager()
        {
            var product = Mocks.Of<IProduct>().First(x => x.InstanceName == "Foo");
            var manager = Mocks.Of<IPatternManager>().First(x => x.Products == new[] { product } && x.IsOpen == true);

            PatternManagerExtensions.Delete(manager, "Foo");

            Mock.Get(manager).Verify(x => x.DeleteProduct(product));
        }
    }
}
