using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.Runtime.UriProviders;

namespace NuPattern.Runtime.UnitTests.UriProviders
{
    [TestClass]
    public class PackUriProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private Solution solution;
        private Project project;
        private Folder folder;
        private Item item;
        private Mock<IServiceProvider> serviceProvider;

        [TestInitialize]
        public void Initialize()
        {
            this.solution = new Solution();
            this.project = new Project { Name = "project", PhysicalPath = @"c:\projects\solution\project\project.csproj" };
            this.folder = new Folder { PhysicalPath = @"c:\projects\solution\project\assets", Name = "assets" };
            this.item = new Item { Data = { CustomTool = "", IncludeInVSIX = "false", CopyToOutputDirectory = (int)CopyToOutput.DoNotCopy, ItemType = "None" }, PhysicalPath = @"c:\projects\solution\project\assets\icon.ico", Name = "icon.ico" };
            this.folder.Items.Add(item);
            this.project.Items.Add(folder);
            this.project.Data.AssemblyName = "project";
            this.project.Id = Guid.NewGuid().ToString();
            this.solution.Items.Add(project);
            this.solution.Id = Guid.NewGuid().ToString();

            this.serviceProvider = new Mock<IServiceProvider>();
            this.serviceProvider.Setup(s => s.GetService(typeof(IFxrUriReferenceService))).Returns(new UriReferenceService(new[] { new SolutionUriProvider() }));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenProjectItem_CreateUriForItem()
        {
            var resolver = new PackUriProvider();

            Assert.Equal("pack://application:,,,/project;component/assets/icon.ico", resolver.CreateUri(new ResourcePack(item)).AbsoluteUri);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPackIsItem_ReturnItem()
        {
            var resolver = new PackUriProvider();
            resolver.Solution = solution;

            Assert.Equal(item, resolver.ResolveUri(new Uri("pack://application:,,,/project;component/assets/icon.ico")).GetItem());
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPackIsNotItem_ReturnNull()
        {
            var resolver = new PackUriProvider();
            resolver.Solution = solution;

            Assert.Null(resolver.ResolveUri(new Uri("pack://application:,,,/referencedproject;component/foo.ico")));
        }
    }
}
