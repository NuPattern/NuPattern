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
        private IFxrUriReferenceService service;

        [TestInitialize]
        public void Initialize()
        {
            VsIdeTestHostContext.Dte.Solution.Open(this.PathTo(@"Runtime.IntegrationTests.Content\SolutionUriProvider\TestProviders.sln"));
            this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            this.service = VsIdeTestHostContext.ServiceProvider.GetService<IFxrUriReferenceService>();
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithNullUri_ThenThrows()
        {
            Assert.Throws<NullReferenceException>(() => this.service.ResolveUri<IItemContainer>(null));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithInvalidUriScheme_ThenThrows()
        {
            Assert.Throws<NotSupportedException>(() => this.service.ResolveUri<IItemContainer>(new Uri("foo://")));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithNullInstance_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(() => this.service.CreateUri<IItemContainer>(null));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenOpenWithNullInstance_ThenThrows()
        {
            Assert.Throws<NullReferenceException>(() => this.service.Open<IItemContainer>(null));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithNoHost_ThenReturnsSolutionUri()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://"));
            Assert.Equal(this.solution, expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution:///"));
            Assert.Equal(this.solution, expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithSolutionFolders_ThenReturnsSolutionFolders()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder\SecSolutionFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder\SecSolutionFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/"));
            Assert.Equal(this.solution.Find<ISolutionFolder>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithSolutionItems_ThenReturnsSolutionItems()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/TopSolutionItem.txt"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\TopSolutionItem.txt").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/TopSolutionItem.txt/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\TopSolutionItem.txt").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/SecSolutionItem.txt"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\SecSolutionItem.txt").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/SecSolutionItem.txt/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\SecSolutionItem.txt").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdSolutionItem.txt"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdSolutionItem.txt").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdSolutionItem.txt/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdSolutionItem.txt").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithProjectsByPath_ThenReturnsProjects()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdProject"));
            Assert.Equal(this.solution.Find<IProject>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdProject/"));
            Assert.Equal(this.solution.Find<IProject>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopProject"));
            Assert.Equal(this.solution.Find<IProject>(@"TopProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopProject/"));
            Assert.Equal(this.solution.Find<IProject>(@"TopProject").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithProjectsById_ThenReturnsProjects()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7"));
            Assert.Equal(this.solution.Find<IProject>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/"));
            Assert.Equal(this.solution.Find<IProject>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55"));
            Assert.Equal(this.solution.Find<IProject>(@"TopProject").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/"));
            Assert.Equal(this.solution.Find<IProject>(@"TopProject").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithSolutionProjectItems_ThenReturnsProjectItems()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdProject/TopProjectItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdProject/TopProjectItem.cs/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject\TopProjectItem.cs").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithProjectFolders_ThenReturnsProjectFolders()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder\SecProjectFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder\SecProjectFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/TrdProjectFolder"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/TrdProjectFolder/"));
            Assert.Equal(this.solution.Find<IFolder>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithProjectItemByName_ThenReturnsProjectItems()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectItem.cs/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/TopProjectFolderItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\TopProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/TopProjectFolderItem.cs/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\TopProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/SecProjectFolderItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\SecProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/SecProjectFolderItem.cs/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\SecProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/TrdProjectFolder/TrdProjectFolderItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder\TrdProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/TrdProjectFolder/TrdProjectFolderItem.cs/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder\TrdProjectFolderItem.cs").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolveUriWithProjectItemById_ThenReturnsProjectItems()
        {
            var expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/TopProjectItem.cs"));
            Assert.Equal(this.solution.Find<IItem>(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\TopProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\TopProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\SecProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\SecProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder\TrdProjectFolderItem.cs").First(), expected);

            expected = this.service.ResolveUri<IItemContainer>(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95/"));
            Assert.Equal(this.solution.Find<IItem>(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder\TrdProjectFolderItem.cs").First(), expected);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolution_TheReturnsUri()
        {
            var result = this.service.CreateUri(this.solution);

            Assert.Equal(new Uri("solution://root/"), result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolutionFolders_TheReturnsUris()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder"), result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolutionItems_TheReturnsUris()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\TopSolutionItem.txt").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder/TopSolutionItem.txt"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\SecSolutionItem.txt").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/SecSolutionItem.txt"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdSolutionItem.txt").FirstOrDefault());
            Assert.Equal(new Uri("solution://root/TopSolutionFolder/SecSolutionFolder/TrdSolutionFolder/TrdSolutionItem.txt"), result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolutionProjectsByPath_TheReturnsUris()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject").FirstOrDefault());
            Assert.Equal(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55"), result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolutionProjectItemWithItemGuid_TheReturnsResilientUri()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject\TopProjectItemWithItemGuid.cs").FirstOrDefault());
            Assert.Equal(new Uri("solution://B13B408F-4969-48C0-85C2-227461953FA7/B3FA54F7-EF27-4EEB-A15A-05EF37EEB0CF"), result);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "What??")]
        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithSolutionProjectItemWithoutItemGuid_TheReturnsResilientUri()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopSolutionFolder\SecSolutionFolder\TrdSolutionFolder\TrdProject\TopProjectItem.cs").FirstOrDefault());
            var itemGuidString = result.ToString().ToLower(CultureInfo.InvariantCulture).Replace("solution://b13b408f-4969-48c0-85c2-227461953fa7/", string.Empty);
            itemGuidString = itemGuidString.Trim(SolutionUriProvider.UriPathDelimiter);

            Guid itemGuid;
            Assert.True(Guid.TryParse(itemGuidString, out itemGuid));
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithProjectFolders_TheReturnsUris()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder\SecProjectFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/TopProjectFolder/SecProjectFolder/TrdProjectFolder"), result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenCreateUriWithProjectItems_TheReturnsUris()
        {
            var result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectItem.cs").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D2592611-C2DB-4CB0-AFF3-C1CF8102DCD6"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder\TopProjectFolderItem.cs").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/D6C06317-BA19-4E3C-8F6F-216AA2607172"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder\SecProjectFolder\SecProjectFolderItem.cs").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/827D5E95-B27A-49D9-8CCC-5B6B8794D1BB"), result);

            result = this.service.CreateUri(this.solution.Find(@"TopProject\TopProjectFolder\SecProjectFolder\TrdProjectFolder\TrdProjectFolderItem.cs").FirstOrDefault());
            Assert.Equal(new Uri("solution://46F565BA-4071-4ACE-A876-BEBFFF6A8B55/0F8E541A-784C-4EB7-AB71-BEEAEF10FF95"), result);
        }
    }
}