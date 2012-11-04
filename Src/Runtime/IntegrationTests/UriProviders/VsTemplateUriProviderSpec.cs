using System;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
	[TestClass]
	public class VsTemplateUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		private IServiceProvider serviceProvider;
		private VsTemplateUriProvider target;

		[TestInitialize]
		public void Initialize()
		{
			this.serviceProvider = VsIdeTestHostContext.ServiceProvider;
			this.target = new VsTemplateUriProvider(this.serviceProvider);
		}

		[TestMethod, TestCategory("Integration")]
		[HostType("VS IDE")]
		public void WhenResolvingProjectTemplate_ThenGetsTemplate()
		{
			var template = this.target.ResolveUri(new Uri("template://Project/CSharp/Microsoft.CSharp.ClassLibrary"));

			Assert.NotNull(template);
		}

		[TestMethod, TestCategory("Integration")]
		[HostType("VS IDE")]
		public void WhenResolvingItemTemplate_ThenGetsTemplate()
		{
			var template = this.target.ResolveUri(new Uri("template://Item/CSharp/Microsoft.CSharp.Class"));

			Assert.NotNull(template);
		}

		[TestMethod, TestCategory("Integration")]
		[HostType("VS IDE")]
		public void WhenResolvingProjectTemplateViaUriService_ThenGetsTemplate()
		{
			var template = this.serviceProvider.GetService<IFxrUriReferenceService>().ResolveUri<IVsTemplate>(new Uri("template://Project/CSharp/Microsoft.CSharp.ClassLibrary"));

			Assert.NotNull(template);
		}

		[TestMethod, TestCategory("Integration")]
		[HostType("VS IDE")]
		public void WhenResolvingItemTemplateViaUriService_ThenGetsTemplate()
		{
			var template = this.serviceProvider.GetService<IFxrUriReferenceService>().ResolveUri<IVsTemplate>(new Uri("template://Item/CSharp/Microsoft.CSharp.Class"));

			Assert.NotNull(template);
		}
	}
}
