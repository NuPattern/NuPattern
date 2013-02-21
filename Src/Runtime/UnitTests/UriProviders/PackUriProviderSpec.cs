using System;
using System.Linq;
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

		private IItem item1;
		private IItem item2;
		private Mock<IServiceProvider> serviceProvider;
		private Solution solution = new Solution
		{
			Id = Guid.NewGuid().ToString(),
			Items =
			{
				new Project
				{
					Name = "project",
					PhysicalPath = @"c:\projects\solution\project\project.csproj",
					Data = 
					{
						AssemblyName = "project",
					},
					Id = Guid.NewGuid().ToString(),
					Items = 
					{
						new Folder
						{
							Name = "assets",
							Items = 
							{
								new Item
								{
									Name = "icon.ico",
									PhysicalPath = @"c:\projects\solution\project\assets\icon.ico",
									Data = 
									{
										CustomTool = "",
										IncludeInVSIX = "false",
										CopyToOutputDirectory = (int)CopyToOutput.DoNotCopy,
										ItemType = "None",
									},
								},
								new Item
								{
									Name = "icon with spaces.ico",
									PhysicalPath = @"c:\projects\solution\project\assets\icon with spaces.ico",
								}
							}
						},
					}
				},

			}
		};

		[TestInitialize]
		public void Initialize()
		{
			this.item1 = this.solution.Find<IItem>().First();
			this.item2 = this.solution.Find<IItem>().Last();
			this.serviceProvider = new Mock<IServiceProvider>();
			this.serviceProvider.Setup(s => s.GetService(typeof(IFxrUriReferenceService))).Returns(new UriReferenceService(new[] { new SolutionUriProvider() }));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenProjectItem_CreateUriForItem()
		{
			var resolver = new PackUriProvider();

			Assert.Equal("pack://application:,,,/project;component/assets/icon.ico", resolver.CreateUri(new ResourcePack(this.item1)).AbsoluteUri);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenProjectItemWithSpacesInName_CreateUriForItem()
		{
			var resolver = new PackUriProvider();

			Assert.Equal("pack://application:,,,/project;component/assets/icon%20with%20spaces.ico", resolver.CreateUri(new ResourcePack(this.item2)).AbsoluteUri);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenPackIsItem_ResolveItem()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = this.solution;

			Assert.Equal(this.item1, resolver.ResolveUri(new Uri("pack://application:,,,/project;component/assets/icon.ico")).GetItem());
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenPackIsItemWithSpacesInName_ResolveItem()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = this.solution;

			Assert.Equal(this.item2, resolver.ResolveUri(new Uri("pack://application:,,,/project;component/assets/icon%20with%20spaces.ico")).GetItem());
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenPackIsNotItem_ResolveNull()
		{
			var resolver = new PackUriProvider();
			resolver.Solution = this.solution;

			Assert.Null(resolver.ResolveUri(new Uri("pack://application:,,,/referencedproject;component/foo.ico")));
		}
	}
}
