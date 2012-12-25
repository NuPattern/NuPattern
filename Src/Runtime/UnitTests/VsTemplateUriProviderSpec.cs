using System;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NuPattern.Runtime.UnitTests
{
	[TestClass]
	public class VsTemplateUriProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		private Mock<IServiceProvider> serviceProvider = new Mock<IServiceProvider>();

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingWithNullServiceProvider_ThenThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() => new VsTemplateUriProvider(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingAnUriWithNullTemplate_ThenThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() => new VsTemplateUriProvider(this.serviceProvider.Object).CreateUri(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenResolvingAnUriWithoutPath_ThenThrowsException()
		{
			Assert.Throws<ArgumentException>(() => new VsTemplateUriProvider(this.serviceProvider.Object).ResolveUri(new Uri("template://Project/CSharp")));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingAnUriWithTemplateId_ThenReturnsReturnsUri()
		{
			var template = Mocks.Of<IVsTemplate>().First(t =>
				t.TypeString == "Project" &&
				t.TemplateData.ProjectType == "CSharp" &&
				t.TemplateData.TemplateID == "Foo");

			var provider = new VsTemplateUriProvider(this.serviceProvider.Object);
			var uri = provider.CreateUri(template);

			Assert.Equal(new Uri("template://Project/CSharp/Foo"), uri);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingAnUriWithTemplateName_ThenReturnsUri()
		{
			var template = Mocks.Of<IVsTemplate>().First(t =>
				t.TypeString == "Project" &&
				t.TemplateData.ProjectType == "CSharp" &&
				t.TemplateData.Name.Value == "Foo");

			var provider = new VsTemplateUriProvider(this.serviceProvider.Object);
			var uri = provider.CreateUri(template);

			Assert.Equal(new Uri("template://Project/CSharp/Foo"), uri);
		}
	}
}