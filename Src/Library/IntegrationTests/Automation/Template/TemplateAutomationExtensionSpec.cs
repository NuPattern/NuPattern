using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Runtime;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.IntegrationTests
{
    [TestClass]
    [CLSCompliant(false)]
    [DeploymentItem(@"Library.IntegrationTests.Content\\EmptySolution.sln")]
    public class TemplateAutomationExtensionSpec : IntegrationTest
    {
        private static readonly IAssertion Assert = new Assertion();

        private IPatternManager manager;
        private IInstalledToolkitInfo toolkit;

        [TestInitialize]
        public void InitializeContext()
        {
            var solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();

            solution.Open(PathTo(@"EmptySolution.sln"));

            this.manager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();
            var componentModel = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
            var installedToolkits = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

            this.toolkit = installedToolkits
                .SingleOrDefault(toolkit => toolkit.Id.Equals("1530D736-97EA-4B4A-8A25-C6C7A089D1BB", StringComparison.OrdinalIgnoreCase));
        }

        [HostType("VS IDE")]
        [Ignore]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreatingAProduct_ThenSetsArtifactLinks()
        {
            IProduct product = null;

            UIThreadInvoker.Invoke((Action)(() =>
                {
                    DoActionWithWait(4000, () => product = this.manager.CreateProduct(toolkit, "Foo"));
                }));

            Assert.NotNull(product);
            Assert.NotNull(product.TryGetReference(ReferenceKindConstants.ArtifactLink));

            Assert.True(product.Views.Count() > 0);
            var view = product.Views.ElementAt(0);

            Assert.True(view.Elements.Count() > 0);
            var element = view.Elements.ElementAt(0);

            Assert.NotNull(element.TryGetReference(ReferenceKindConstants.ArtifactLink));
        }
    }
}
