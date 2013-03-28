using System;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.VsIde;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.IntegrationTests
{
    public abstract class CreateOnUnfoldSpec : IntegrationTest
    {
        protected readonly string MainToolkitId = "cb6a7b60-ec95-42dd-8f01-28ff11dcf800";
        protected readonly string MainToolkitVsTemplate = "b1707c79361e-MainTest";

        internal static readonly IAssertion Assert = new Assertion();
        protected ISolution solution;
        protected IPatternManager patternManager;
        protected IProduct product;
        protected EnvDTE.DTE dte;
        protected string instanceName = string.Empty;
        protected IProduct pattern;

        public virtual void InitializeContext()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.dte = VsIdeTestHostContext.ServiceProvider.GetService<EnvDTE.DTE>();
            this.patternManager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();

            this.instanceName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            this.solution.CreateInstance(this.DeploymentDirectory, this.instanceName);
        }

        public void ReferenceLinkIsAdded()
        {
            var uriProvider = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
            var project = solution.Items.OfType<IProject>().First().Id;
            Assert.True(this.pattern.References.Any(r => r.Value == string.Format("solution://{0}/", project)));
        }

        public void SyncNamesWorks()
        {
            var newName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            var handle = UIThreadInvoker.Invoke(new Action(() => pattern.InstanceName = newName));
            Assert.Equal(newName, solution.Items.OfType<IProject>().First().Name);
        }
    }

    [TestClass]
    public class GivenAnAutomatedCreatedPattern : CreateOnUnfoldSpec
    {
        [TestInitialize]
        public override void InitializeContext()
        {
            //this.instanceName = instanceName.Substring(0, 2) + " " + instanceName.Substring(2);
            base.InitializeContext();

            var toolkit = this.patternManager.InstalledToolkits.Single(f => f.Id == MainToolkitId);
            UIThreadInvoker.Invoke(new Action(() => this.product = this.patternManager.CreateProduct(toolkit, instanceName)));

            this.pattern = this.patternManager.Products.FirstOrDefault(p => p.InstanceName == instanceName);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenSolutionItemExists()
        {
            Assert.True(solution.Items.Any(p => p.Name == instanceName));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenPatternInstanceExists()
        {
            Assert.NotNull(pattern);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenReferenceLinkIsAdded()
        {
            base.ReferenceLinkIsAdded();
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenSyncNamesWork()
        {
            base.SyncNamesWorks();
        }
    }

    [TestClass]
    public class GivenUnfoldedTemplatePattern : CreateOnUnfoldSpec
    {
        [TestInitialize]
        public override void InitializeContext()
        {
            base.InitializeContext();

            var templatePath = ((Solution2)dte.Solution).GetProjectTemplate(MainToolkitVsTemplate, "CSharp");
            var template = new VsProjectTemplate(templatePath);
            template.Unfold(instanceName, this.solution);

            this.pattern = this.patternManager.Products.FirstOrDefault(p => p.InstanceName == instanceName);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenSolutionItemExists()
        {
            Assert.True(solution.Items.Any(p => p.Name == instanceName));
        }

        public void ThenPatternInstanceExists()
        {
            Assert.NotNull(pattern);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void ThenReferenceLinkIsAdded()
        {
            base.ReferenceLinkIsAdded();
        }
    }
}
