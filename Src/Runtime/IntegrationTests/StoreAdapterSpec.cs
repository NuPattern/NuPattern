using System;
using System.IO;
using System.Linq;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
    [TestClass]
    [CLSCompliant(false)]
    [DeploymentItem(@"Runtime.IntegrationTests.Content\\Projects\\Sample", "Runtime.IntegrationTests.Content\\StoreAdapterSpec")]
    public class StoreAdapterSpec : IntegrationTest
    {
        internal static readonly IAssertion Assert = new Assertion();

        private ISolution solution;
        private IModelBus modelBus;

        [TestInitialize]
        public void Initialize()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.modelBus = VsIdeTestHostContext.ServiceProvider.GetService<SModelBus, IModelBus>();

            var solution = Path.Combine(this.TestContext.DeploymentDirectory, "Runtime.IntegrationTests.Content\\StoreAdapterSpec\\Sample.sln");
            VsIdeTestHostContext.Dte.Solution.Open(solution);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingReference_ThenUsesAdapter()
        {
            var stateFile = this.solution.Find<IItem>(item => item.Name.EndsWith(Constants.RuntimeStoreExtension)).First();

            var manager = this.modelBus.FindAdapterManagers(new object[] { stateFile.PhysicalPath }).FirstOrDefault();

            Assert.NotNull(manager);

            Assert.True(manager.CanCreateReference(new object[] { stateFile.PhysicalPath }));

            var mar = new ModelingAdapterReference(null, null, stateFile.PhysicalPath);
            var mbr = new ModelBusReference(this.modelBus, Adapter.AdapterId,
                Path.GetFileNameWithoutExtension(stateFile.Name),
                mar);

            using (var adapter = this.modelBus.CreateAdapter(mbr, VsIdeTestHostContext.ServiceProvider))
            {
                var withRoot = adapter as IModelingAdapterWithRootedModel;
                var state = ((IProductState)withRoot.ModelRoot).Products.First();
                var elementRef = adapter.GetElementReference(state);

                Assert.NotNull(elementRef);
            }
        }
    }
}
