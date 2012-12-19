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
            solution = new Solution();
            project = new Project { Name = "project", PhysicalPath = @"c:\projects\solution\project\project.csproj" };
            folder = new Folder { PhysicalPath = @"c:\projects\solution\project\assets", Name = "assets" };
            item = new Item { Data = { CustomTool = "", IncludeInVSIX = "false", CopyToOutputDirectory = (int)CopyToOutput.DoNotCopy, ItemType = "None" }, PhysicalPath = @"c:\projects\solution\project\assets\icon.ico", Name = "icon.ico" };
            folder.Items.Add(item);
            project.Items.Add(folder);
            project.Data.AssemblyName = "project";
            project.Id = Guid.NewGuid().ToString();
            solution.Items.Add(project);
            solution.Id = Guid.NewGuid().ToString();

            serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(IFxrUriReferenceService))).Returns(new UriReferenceService(new[] { new SolutionUriProvider() }));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenItemExists_ReturnPackForItem()
        {
            var resolver = new PackUriProvider();

            Assert.Equal("pack://application:,,,/project;component/assets/icon.ico", resolver.CreateUri(new ResourcePack(item)).AbsoluteUri);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenPackIsValid_ReturnItem()
        {
            var resolver = new PackUriProvider();
            resolver.Solution = solution;

            Assert.Equal(item, resolver.ResolveUri(new Uri("pack://application:,,,/project;component/assets/icon.ico")).Item);
        }
    }
}
