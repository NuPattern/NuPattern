using System.IO;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
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
