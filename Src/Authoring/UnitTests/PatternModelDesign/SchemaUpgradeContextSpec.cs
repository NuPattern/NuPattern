using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Runtime.Schema;

namespace NuPattern.Authoring.UnitTests.PatternModelDesign
{
    [TestClass]
    public class SchemaUpgradeContextSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestInitialize]
            public void InitializeContext()
            {
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNullFilePath_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    new SchemaUpgradeContext(null, null));
            }
        }
    }
}
