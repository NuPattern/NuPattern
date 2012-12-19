using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NuPattern.Runtime.UnitTests.UriProviders
{
	[TestClass]
	public class VsTemplateUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod, TestCategory("Unit")]
		public void WhenTemplateTypeIsItem_ThenreturnsBaseUri()
		{
            Assert.Equal("template://item", 
                VsTemplateUriProvider.GetUriBase(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.VsTemplateType.Item));
		}

        [TestMethod, TestCategory("Unit")]
        public void WhenTemplateTypeIsProject_ThenreturnsBaseUri()
        {
            Assert.Equal("template://project",
                VsTemplateUriProvider.GetUriBase(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.VsTemplateType.Project));
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenTemplateTypeIsProjectGroup_ThenreturnsBaseUri()
        {
            Assert.Equal("template://projectgroup",
                VsTemplateUriProvider.GetUriBase(Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.VsTemplateType.ProjectGroup));
        }
    }
}
