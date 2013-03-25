using System;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.IntegrationTests.UriProviders
{
    [TestClass]
    [DeploymentItem(@"Runtime.IntegrationTests.dll")] // Assembly contains the TestIcon.bmp file
    [DeploymentItem(@"Runtime.IntegrationTests.Content\PackUriProvider", @"Runtime.IntegrationTests.Content\PackUriProvider")]
    public class PackUriProviderSpec : IntegrationTest
    {
        internal static readonly IAssertion Assert = new Assertion();
        private ISolution solution;
        private IProject project;
        private IFxrUriReferenceService service;

        [TestInitialize]
        public void Initialize()
        {
            VsIdeTestHostContext.Dte.Solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\PackUriProvider\TestProviders.sln"));

            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.project = this.solution.Find<IProject>().First();
            this.service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            VsIdeTestHostContext.Dte.Solution.Close(false);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenProjectItemIsLocal_ThenUriCreated()
        {
            var item = this.project.Find<IItem>(i => i.Name == "Bitmap1.bmp").First();
            var createdUri = this.service.CreateUri<ResourcePack>(new ResourcePack(item));

            Assert.Equal("pack://application:,,,/TestProviders;component/Resources/Bitmap1.bmp", createdUri.AbsoluteUri);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenProjectItemIsLocalWithSpacesInName_ThenUriCreated()
        {
            var item = this.project.Find<IItem>(i => i.Name == "Bitmap2 With Spaces.bmp").First();
            var createdUri = this.service.CreateUri<ResourcePack>(new ResourcePack(item));

            Assert.Equal("pack://application:,,,/TestProviders;component/Resources/Bitmap2%20With%20Spaces.bmp", createdUri.AbsoluteUri);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenProjectItemIsLinked_ThenUriCreated()
        {
            var item = this.project.Find<IItem>(i => i.Name == "LinkedBitmap1.bmp").First();
            var createdUri = this.service.CreateUri<ResourcePack>(new ResourcePack(item));

            Assert.Equal("pack://application:,,,/TestProviders;component/Resources/LinkedBitmap1.bmp", createdUri.AbsoluteUri);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsProjectItem_ThenResolvesToItem()
        {
            var item = this.project.Find<IItem>(i => i.Name == "Bitmap1.bmp").First();

            var resolved = this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/TestProviders;component/Resources/Bitmap1.bmp"));
            Assert.Equal(item, resolved.GetItem());
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsProjectItemWithSpacesInName_ThenResolvesToItem()
        {
            var item = this.project.Find<IItem>(i => i.Name == "Bitmap2 With Spaces.bmp").First();

            var resolved = this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/TestProviders;component/Resources/Bitmap2%20With%20Spaces.bmp"));
            Assert.Equal(item, resolved.GetItem());
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsLinkedProjectItem_ThenResolvesToItem()
        {
            var item = this.project.Find<IItem>(i => i.Name == "LinkedBitmap1.bmp").First();
            var resolved = this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/TestProviders;component/Resources/LinkedBitmap1.bmp"));
            Assert.Equal(item, resolved.GetItem());
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsNotItem_ReturnsNull()
        {
            Assert.Null(this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/TestProviders;component/foo.bmp")));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsNotProject_ReturnsNull()
        {
            Assert.Null(this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/foo;component/Bitmap1.bmp")));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsNotReferencedAssemblyResource_ReturnsNull()
        {
            Assert.Null(this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/System;component/foo.bmp")));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenUriIsReferencedAssemblyResource_ResolvesToResource()
        {
            var resolved = this.service.ResolveUri<ResourcePack>(new Uri("pack://application:,,,/NuPattern.Runtime.IntegrationTests;component/Runtime.IntegrationTests.Content/PackUriProvider/TestIcon.bmp"));

            Assert.Equal("NuPattern.Runtime.IntegrationTests", resolved.AssemblyName);
            Assert.Equal("runtime.integrationtests.content/packuriprovider/testicon.bmp", resolved.ResourcePath);
        }
    }
}
