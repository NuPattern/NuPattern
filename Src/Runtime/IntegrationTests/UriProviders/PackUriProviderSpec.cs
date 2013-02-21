using System;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.UriProviders;

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

		[TestInitialize]
		public void Initialize()
		{
			VsIdeTestHostContext.Dte.Solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\PackUriProvider\TestProviders.sln"));

			this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
			this.project = this.solution.Find<IProject>().First();
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
			var resolver = new PackUriProvider();

			var item = this.project.Find<IItem>(i => i.Name == "Bitmap1.bmp").First();
			var createdUri = resolver.CreateUri(new ResourcePack(item));

			Assert.Equal("pack://application:,,,/TestProviders;component/Resources/Bitmap1.bmp", createdUri.AbsoluteUri);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenProjectItemIsLocalWithSpacesInName_ThenUriCreated()
		{
			var resolver = new PackUriProvider();

			var item = this.project.Find<IItem>(i => i.Name == "Bitmap2 With Spaces.bmp").First();
			var createdUri = resolver.CreateUri(new ResourcePack(item));

			Assert.Equal("pack://application:,,,/TestProviders;component/Resources/Bitmap2%20With%20Spaces.bmp", createdUri.AbsoluteUri);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenProjectItemIsLinked_ThenUriCreated()
		{
			var resolver = new PackUriProvider();

			var item = this.project.Find<IItem>(i => i.Name == "LinkedBitmap1.bmp").First();
			var createdUri = resolver.CreateUri(new ResourcePack(item));

			Assert.Equal("pack://application:,,,/TestProviders;component/Resources/LinkedBitmap1.bmp", createdUri.AbsoluteUri);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsProjectItem_ThenResolvesToItem()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			var item = this.project.Find<IItem>(i => i.Name == "Bitmap1.bmp").First();

			var resolved = resolver.ResolveUri(new Uri("pack://application:,,,/TestProviders;component/Resources/Bitmap1.bmp"));
			Assert.Equal(item, resolved.GetItem());
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsProjectItemWithSpacesInName_ThenResolvesToItem()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			var item = this.project.Find<IItem>(i => i.Name == "Bitmap2 With Spaces.bmp").First();

			var resolved = resolver.ResolveUri(new Uri("pack://application:,,,/TestProviders;component/Resources/Bitmap2%20With%20Spaces.bmp"));
			Assert.Equal(item, resolved.GetItem());
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsLinkedProjectItem_ThenResolvesToItem()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			var item = this.project.Find<IItem>(i => i.Name == "LinkedBitmap1.bmp").First();
			var resolved = resolver.ResolveUri(new Uri("pack://application:,,,/TestProviders;component/Resources/LinkedBitmap1.bmp"));
			Assert.Equal(item, resolved.GetItem());
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsNotItem_ReturnsNull()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			Assert.Null(resolver.ResolveUri(new Uri("pack://application:,,,/TestProviders;component/foo.bmp")));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsNotProject_ReturnsNull()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			Assert.Null(resolver.ResolveUri(new Uri("pack://application:,,,/foo;component/Bitmap1.bmp")));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsNotReferencedAssemblyResource_ReturnsNull()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			Assert.Null(resolver.ResolveUri(new Uri("pack://application:,,,/System;component/foo.bmp")));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenUriIsReferencedAssemblyResource_ResolvesToResource()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = solution;

			var resolved = resolver.ResolveUri(new Uri("pack://application:,,,/NuPattern.Runtime.IntegrationTests;component/Runtime.IntegrationTests.Content/PackUriProvider/TestIcon.bmp"));

			Assert.Equal("NuPattern.Runtime.IntegrationTests", resolved.AssemblyName);
			Assert.Equal("runtime.integrationtests.content/packuriprovider/testicon.bmp", resolved.ResourcePath);
		}
	}
}
