using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;
using NuPattern.VisualStudio.Solution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class CompositionServiceSpec
    {
        [HostType("VS IDE")]
        [TestMethod]
        public void When_Retieving_Composition_Service_Then_Succeeds()
        {
            var components = NuPatternCompositionService.Instance;

			var composition = components.GetExportedValue<INuPatternCompositionService>();

            Assert.IsNotNull(composition);
        }

		[HostType("VS IDE")]
		[TestMethod]
		public void When_Retieving_Pattern_Manager_Service_Then_Succeeds()
		{
			var components = NuPatternCompositionService.Instance;

			var manager = components.GetExportedValue<IPatternManager>();

			Assert.IsNotNull(manager);
		}

		[HostType("VS IDE")]
		[TestMethod]
		public void When_Retieving_Binding_Factory_Then_Succeeds()
		{
			var components = NuPatternCompositionService.Instance;

			var manager = components.GetExportedValue<IBindingFactory>();

			Assert.IsNotNull(manager);
		}

		[HostType("VS IDE")]
		[TestMethod]
		public void When_Retieving_Binding_Composition_Service_Then_Succeeds()
		{
			var components = NuPatternCompositionService.Instance;

			var manager = components.GetExportedValue<IBindingCompositionService>();

			Assert.IsNotNull(manager);
		}

		[HostType("VS IDE")]
		[TestMethod]
		public void When_Retieving_ISolution_Then_Succeeds()
		{
			var components = NuPatternCompositionService.Instance;

			var solution = components.GetExportedValue<ISolution>();

			Assert.IsNotNull(solution);
		}

	}
}
