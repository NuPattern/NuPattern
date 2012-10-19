using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.UriProviders;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
