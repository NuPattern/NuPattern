using System;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.VsIde;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
    public class CreateOnUnfoldSpec : IntegrationTest
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

        public void SolutionItemExists()
        {
            Assert.True(solution.Items.Any(p => p.Name == instanceName));
        }

        public void MainPatternExists()
        {
            Assert.NotNull(pattern);
        }

        public void ReferenceLinkIsAdded()
        {
            var uriProvider = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
            var project = solution.Items.OfType<IProject>().First().Id;
            Assert.True(pattern.References.Any(r => r.Value == string.Format("solution://{0}/", project)));
        }

        public void SyncNamesWorks()
        {
            var newName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            var handle = UIThreadInvoker.Invoke(new Action(() => pattern.InstanceName = newName));
            Assert.Equal(newName, solution.Items.OfType<IProject>().First().Name);
        }
    }

    [TestClass]
    public class FromTemplate : CreateOnUnfoldSpec
    {
        [TestInitialize]
        public void InitializeContext()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.instanceName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            this.patternManager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.solution.CreateInstance(this.DeploymentDirectory, instanceName);

            var toolkit = this.patternManager.InstalledToolkits.Single(f => f.Id == MainToolkitId); //TestFactory
            UIThreadInvoker.Invoke(new Action(() => this.product = this.patternManager.CreateProduct(toolkit, instanceName)));

            pattern = this.patternManager.Products.FirstOrDefault(p => p.InstanceName == instanceName);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_SolutionItemExists()
        {
            this.SolutionItemExists();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_ReferenceLinkIsAdded()
        {
            this.ReferenceLinkIsAdded();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_SyncNamesWork()
        {
            this.SyncNamesWorks();
        }
    }

    [TestClass]
    public class SanitizeNames : CreateOnUnfoldSpec
    {
        [TestInitialize]
        public void InitializeContext()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.instanceName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            this.instanceName = instanceName.Substring(0, 2) + " " + instanceName.Substring(2);
            this.patternManager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.solution.CreateInstance(this.DeploymentDirectory, instanceName);

            var toolkit = this.patternManager.InstalledToolkits.Single(f => f.Id == MainToolkitId); //TestFactory
            UIThreadInvoker.Invoke(new Action(() => this.product = this.patternManager.CreateProduct(toolkit, instanceName)));

            pattern = this.patternManager.Products.FirstOrDefault(p => p.InstanceName == instanceName);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_SolutionItemExists()
        {
            Assert.True(solution.Items.Any(p => p.Name == instanceName.Replace(" ", "")));
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_MainPatternExists()
        {
            this.MainPatternExists();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_ReferenceLinkIsAdded()
        {
            this.ReferenceLinkIsAdded();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainToolkit_SanitizeNamesWork()
        {
            this.SyncNamesWorks();
        }
    }

    [TestClass]
    public class FromAutomation : CreateOnUnfoldSpec
    {
        [TestInitialize]
        public void InitializeContext()
        {
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.instanceName = "a" + Guid.NewGuid().ToString().Replace("-", "");
            this.patternManager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();

            this.solution.CreateInstance(this.DeploymentDirectory, instanceName);

            this.dte = VsIdeTestHostContext.ServiceProvider.GetService<EnvDTE.DTE>();

            var templatePath = ((Solution2)dte.Solution).GetProjectTemplate(MainToolkitVsTemplate, "CSharp");
            var template = new VsProjectTemplate(templatePath);
            template.Unfold(instanceName, this.solution);

            pattern = this.patternManager.Products.FirstOrDefault(p => p.InstanceName == instanceName);
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainProject_SolutionItemExists()
        {
            this.SolutionItemExists();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainProject_MainPatternExists()
        {
            this.MainPatternExists();
        }

        [HostType("VS IDE")]
        [TestMethod]
        public void WhenCreatingMainProject_ReferenceLinkIsAdded()
        {
            this.ReferenceLinkIsAdded();
        }
    }
}
