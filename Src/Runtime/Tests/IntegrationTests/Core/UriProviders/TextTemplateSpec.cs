using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Moq;
using NuPattern.Runtime.UriProviders;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests.UriProviders
{
    [TestClass]
    public class TextTemplateSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestMethod, TestCategory("Integration")]
        public void WhenTemplateFileDoesNotExist_ThenThrowsFileNotFoundException()
        {
            Assert.Throws<FileNotFoundException>(() => new TextTemplate(
                new Mock<ITextTemplating>().Object,
                new Mock<IModelBus>().Object,
                Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())));
        }

        [TestClass]
        public class GivenATextTemplate : IntegrationTest
        {
            private ITextTemplating templating;
            private IModelBus modelBus;
            private ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                this.templating = VsIdeTestHostContext.ServiceProvider.GetService<STextTemplating, ITextTemplating>();
                this.modelBus = VsIdeTestHostContext.ServiceProvider.GetService<SModelBus, IModelBus>();
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            }

            [TestMethod, TestCategory("Integration")]
            [HostType("VS IDE")]
            [DeploymentItem("Runtime.IntegrationTests.Content\\Sample", "Runtime.IntegrationTests\\TextTemplateSpec\\WhenUnfolding_ThenTransformsText")]
            [DeploymentItem("Runtime.IntegrationTests.Content\\TextTemplate\\TestTextTemplate.t4", "Runtime.IntegrationTests\\TextTemplateSpec\\WhenUnfolding_ThenTransformsText")]
            public void WhenUnfolding_ThenTransformsText()
            {
                var solution = Path.Combine(this.TestContext.DeploymentDirectory, "Runtime.IntegrationTests\\TextTemplateSpec\\WhenUnfolding_ThenTransformsText\\Sample.sln");
                this.solution.Open(solution);
                var file = Path.Combine(this.TestContext.DeploymentDirectory, "Runtime.IntegrationTests\\TextTemplateSpec\\WhenUnfolding_ThenTransformsText\\TestTextTemplate.t4");
                var template = new TextTemplate(this.templating, this.modelBus, file);
                var parent = this.solution.Find<ISolutionFolder>().First();

                Console.WriteLine(parent.Name);

                var count = parent.Items.Count();

                var item = template.Unfold("Foo", parent);

                Assert.NotNull(item);
                Assert.Equal("Foo.txt", item.Name);
                Assert.Equal(count + 1, parent.Items.Count());
                Assert.True(File.ReadAllText(item.PhysicalPath).Contains("Hello"));
            }
        }
    }
}
