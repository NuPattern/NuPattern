using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Library.Automation;
using NuPattern.Runtime;
using NuPattern.VisualStudio;

namespace NuPattern.Library.IntegrationTests
{
    [TestClass]
    [DeploymentItem("Library.IntegrationTests.Content\\MyTemplate1.gen.vstemplate")]
    [DeploymentItem("Library.IntegrationTests.Content\\MyTemplate2.gen.vstemplate")]
    [CLSCompliant(false)]
    public class InstantiationTemplateWizardSpec : IntegrationTest
    {
        private const string TestToolkitTemplateCacheFormat = @"%localappdata%\Microsoft\VisualStudio\{0}Exp\VTC\Library.IntegrationTests";
        private const string TestToolkitId = "NuPattern.Runtime.IntegrationTests.TestToolkit";
        private static readonly IAssertion Assert = new Assertion();

        private IPatternManager manager;
        private IInstalledToolkitInfo toolkit;
        private EnvDTE.DTE dte;
#if VSVER11
        private string testToolkitTemplatePath;
#endif

        [TestInitialize]
        public void InitializeContext()
        {
            this.dte = VsIdeTestHostContext.ServiceProvider.GetService<EnvDTE.DTE>();
            this.manager = VsIdeTestHostContext.ServiceProvider.GetService<IPatternManager>();
            var componentModel = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
            var installedToolkits = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

            this.toolkit = installedToolkits.SingleOrDefault(t => t.Id == TestToolkitId);

#if VSVER11
            //Copy TestToolkit template to VSExp template cache
            this.testToolkitTemplatePath = string.Format(CultureInfo.InvariantCulture, TestToolkitTemplateCacheFormat, dte.Version);
            Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate1.zip"));
            Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate2.zip"));
            File.Copy(Path.Combine(this.TestContext.DeploymentDirectory, @"MyTemplate1.gen.vstemplate"),
                Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate1.zip\MyTemplate1.gen.vstemplate"), true);
            File.Copy(Path.Combine(this.TestContext.DeploymentDirectory, @"MyTemplate2.gen.vstemplate"),
                Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate2.zip\MyTemplate2.gen.vstemplate"), true);
#endif
        }

#if VSVER11

        [TestCleanup]
        public void CleanUp()
        {
            if (Directory.Exists(Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath)))
            {
                Directory.Delete(Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath), true);
            }
        }
#endif

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenFindToolkitOrThrowWithUnknownTemplate_ThenThrows()
        {
            Assert.Throws<InvalidOperationException>(() =>
                InstantiationTemplateWizard.FindToolkitOrThrow(this.manager, @"C:\undefined.vstemplate"));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenFindToolkitOrThrowWithNonToolkitTemplate_ThenThrows()
        {
            var dtePath = Path.GetDirectoryName(this.dte.Application.FullName);
            Assert.Throws<InvalidOperationException>(() =>
                InstantiationTemplateWizard.FindToolkitOrThrow(this.manager,
                Path.Combine(dtePath, @"\ProjectTemplates\CSharp\Windows\1033\ClassLibrary\csClassLibrary.vstemplate")));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenFindToolkitOrThrowWithToolkitTemplateVs2010_ThenReturnsToolkit()
        {
            var result = InstantiationTemplateWizard.FindToolkitOrThrow(this.manager,
                Path.Combine(this.toolkit.Extension.InstallPath,
                @"Templates\Projects\~PC\MyTemplate1.gen.zip\MyTemplate1.gen.vstemplate"));

            Assert.Equal(result.Id, TestToolkitId);
        }

#if VSVER11
        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenFindToolkitOrThrowWithToolkitTemplateVs2012_ThenReturnsToolkit()
        {
            var result = InstantiationTemplateWizard.FindToolkitOrThrow(this.manager,
                Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate1.zip\MyTemplate1.gen.vstemplate"));

            Assert.Equal(result.Id, TestToolkitId);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenFindToolkitOrThrowWithToolkitTemplateAndTemplateIdVs2012_ThenReturnsToolkit()
        {
            var result = InstantiationTemplateWizard.FindToolkitOrThrow(this.manager,
                Environment.ExpandEnvironmentVariables(this.testToolkitTemplatePath + @"\~PC\Projects\MyTemplate2.zip\MyTemplate2.gen.vstemplate"));

            Assert.Equal(result.Id, TestToolkitId);
        }
#endif
    }
}
