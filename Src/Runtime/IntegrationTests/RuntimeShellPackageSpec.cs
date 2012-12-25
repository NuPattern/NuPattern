using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class RuntimeShellPackageSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext : IntegrationTest
        {
            [TestInitialize]
            public void InitializeContext()
            {
            }
        }
    }
}
