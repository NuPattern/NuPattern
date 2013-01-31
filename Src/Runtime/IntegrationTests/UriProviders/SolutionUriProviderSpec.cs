using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.Runtime.UriProviders;

namespace NuPattern.Runtime.IntegrationTests
{
	[TestClass]
	[CLSCompliant(false)]
    [DeploymentItem(@"Runtime.IntegrationTests.Content\SolutionUriProvider", @"Runtime.IntegrationTests.Content\SolutionUriProvider")]
	public class SolutionUriProviderSpec : IntegrationTest
	{
		private static readonly IAssertion Assert = new Assertion();

		private ISolution solution;
		private IFxrUriReferenceProvider<IItemContainer> provider;

		[TestInitialize]
		public void Initialize()
		{
            VsIdeTestHostContext.Dte.Solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\SolutionUriProvider\TestProviders.sln"));

			this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
			this.provider = new SolutionUriProvider { Solution = this.solution };
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithNullUri_ThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => this.provider.ResolveUri(null));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithInvalidUriScheme_ThenThrows()
		{
			Assert.Throws<UriFormatException>(() => this.provider.ResolveUri(new Uri("foo://")));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithNullInstance_ThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => this.provider.CreateUri(null));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenOpenWithNullInstance_ThenThrows()
		{
			Assert.Throws<ArgumentNullException>(() => this.provider.Open(null));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithNoHost_ThenReturnsSolutionUri()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://"));
			Assert.Equal(this.solution, expected);

			expected = this.provider.ResolveUri(new Uri("solution:///"));
			Assert.Equal(this.solution, expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithSolutionFolders_ThenReturnsSolutionFolders()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/"));
			Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithSolutionItems_ThenReturnsSolutionItems()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/TopLevelSolutionItem.txt"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\TopLevelSolutionItem.txt").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/TopLevelSolutionItem.txt/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\TopLevelSolutionItem.txt").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/SecondLevelSolutionItem.txt"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\SecondLevelSolutionItem.txt").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/SecondLevelSolutionItem.txt/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\SecondLevelSolutionItem.txt").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelSolutionItem.txt"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelSolutionItem.txt").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelSolutionItem.txt/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelSolutionItem.txt").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithProjectsByPath_ThenReturnsProjects()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelProject"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelProject/"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelProject"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelProject/"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelProject").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithProjectsById_ThenReturnsProjects()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelProject").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/"));
			Assert.Equal(this.solution.Find<IProject>(@"TopLevelProject").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithSolutionProjectItems_ThenReturnsProjectItems()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelProject/TopLevelProjectItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelProject/TopLevelProjectItem.cs/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject\TopLevelProjectItem.cs").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithProjectFolders_ThenReturnsProjectFolders()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/ThirdLevelProjectFolder"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/ThirdLevelProjectFolder/"));
			Assert.Equal(this.solution.Find<IFolder>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithProjectItemByName_ThenReturnsProjectItems()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectItem.cs/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/TopLevelProjectFolderItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\TopLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/TopLevelProjectFolderItem.cs/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\TopLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/SecondLevelProjectFolderItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\SecondLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/SecondLevelProjectFolderItem.cs/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\SecondLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/ThirdLevelProjectFolder/ThirdLevelProjectFolderItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder\ThirdLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/ThirdLevelProjectFolder/ThirdLevelProjectFolderItem.cs/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder\ThirdLevelProjectFolderItem.cs").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenResolveUriWithProjectItemById_ThenReturnsProjectItems()
		{
			var expected = this.provider.ResolveUri(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/TopLevelProjectItem.cs"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\TopLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\TopLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\SecondLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\SecondLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder\ThirdLevelProjectFolderItem.cs").First(), expected);

			expected = this.provider.ResolveUri(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95/"));
			Assert.Equal(this.solution.Find<IItem>(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder\ThirdLevelProjectFolderItem.cs").First(), expected);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolution_TheReturnsUri()
		{
			var result = this.provider.CreateUri(this.solution);

			Assert.Equal(new Uri("solution://root/"), result);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolutionFolders_TheReturnsUris()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder"), result);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolutionItems_TheReturnsUris()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\TopLevelSolutionItem.txt").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder/TopLevelSolutionItem.txt"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\SecondLevelSolutionItem.txt").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/SecondLevelSolutionItem.txt"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelSolutionItem.txt").FirstOrDefault());
			Assert.Equal(new Uri("solution://root/TopLevelSolutionFolder/SecondLevelSolutionFolder/ThirdLevelSolutionFolder/ThirdLevelSolutionItem.txt"), result);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolutionProjectsByPath_TheReturnsUris()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject").FirstOrDefault());
			Assert.Equal(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55"), result);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolutionProjectItemWithItemGuid_TheReturnsResilientUri()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject\TopLevelProjectItemWithItemGuid.cs").FirstOrDefault());
			Assert.Equal(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/B3FA54F7-EF27-4EEB-A15A-05EF37EEB0CF"), result);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "What??")]
		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithSolutionProjectItemWithoutItemGuid_TheReturnsResilientUri()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelSolutionFolder\SecondLevelSolutionFolder\ThirdLevelSolutionFolder\ThirdLevelProject\TopLevelProjectItem.cs").FirstOrDefault());
			var itemGuidString = result.ToString().ToLower(CultureInfo.InvariantCulture).Replace("solution://b13b408f-4969-48c0-85c2-227461953fa7/", string.Empty);
			itemGuidString = itemGuidString.Trim(SolutionUriProvider.UriPathDelimiter);

			Guid itemGuid;
			Assert.True(Guid.TryParse(itemGuidString, out itemGuid));
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithProjectFolders_TheReturnsUris()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopLevelProjectFolder/SecondLevelProjectFolder/ThirdLevelProjectFolder"), result);
		}

		[HostType("VS IDE")]
		[TestMethod, TestCategory("Integration")]
		public void WhenCreateUriWithProjectItems_TheReturnsUris()
		{
			var result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectItem.cs").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder\TopLevelProjectFolderItem.cs").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\SecondLevelProjectFolderItem.cs").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB"), result);

			result = this.provider.CreateUri(this.solution.Find(@"TopLevelProject\TopLevelProjectFolder\SecondLevelProjectFolder\ThirdLevelProjectFolder\ThirdLevelProjectFolderItem.cs").FirstOrDefault());
			Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95"), result);
		}
	}
}