using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests.UriProviders
{
	[TestClass]
	public class VsTemplateUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenTemplateTypeIsItem_ThenreturnsBaseUri()
		{
            Assert.Equal("template://item", 
                VsTemplateUriProvider.GetUriBase(TeamArchitect.PowerTools.Features.VsTemplateType.Item));
		}

        [TestMethod]
        public void WhenTemplateTypeIsProject_ThenreturnsBaseUri()
        {
            Assert.Equal("template://project",
                VsTemplateUriProvider.GetUriBase(TeamArchitect.PowerTools.Features.VsTemplateType.Project));
        }

        [TestMethod]
        public void WhenTemplateTypeIsProjectGroup_ThenreturnsBaseUri()
        {
            Assert.Equal("template://projectgroup",
                VsTemplateUriProvider.GetUriBase(TeamArchitect.PowerTools.Features.VsTemplateType.ProjectGroup));
        }
    }
}
