using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
    public class TypeExtensionsSpec
    {
        [TestClass]
        public class GivenAnyType
        {
            [TestMethod]
            public void WhenTypeIsString_ThenIsClsCompliant()
            {
                var valueType = typeof(string);

                Assert.IsTrue(valueType.IsClsCompliant());
            }

            [TestMethod]
            public void WhenTypeIsUInt64_ThenNotIsClsCompliant()
            {
                var valueType = typeof(UInt64);

                Assert.IsFalse(valueType.IsClsCompliant());
            }

            [TestMethod]
            public void WhenTypeIstest_ThenNotIsClsCompliant()
            {
                var valueType = typeof(NonClsCompliantType);

                Assert.IsFalse(valueType.IsClsCompliant());
            }
        }


        private class NonClsCompliantType
        {
        }
    }
}
