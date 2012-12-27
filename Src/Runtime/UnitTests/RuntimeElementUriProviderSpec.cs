using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.UriProviders;

namespace NuPattern.Runtime.IntegrationTests
{
	[TestClass]
	public class RuntimeElementUriProviderSpec
	{
		private static readonly IAssertion Assert = new Assertion();

		private RuntimeElementUriProvider provider;
		private IInstanceBase element;
		private Guid id;

		[TestInitialize]
		public void Initialize()
		{
			this.id = Guid.NewGuid();
			this.provider = new RuntimeElementUriProvider();

			var serviceProvider = new Mock<IServiceProvider>();
			var namedElement = new Mock<IInstanceBase>();
			var productStore = new Mock<IProductState>();
			var manager = new Mock<IPatternManager>();

			namedElement.SetupGet(n => n.Id).Returns(this.id);
			productStore.Setup(p => p.FindAll<IInstanceBase>()).Returns(new IInstanceBase[] { namedElement.Object });
			manager.SetupGet(m => m.Store).Returns(productStore.Object);
			serviceProvider.Setup(sp => sp.GetService(typeof(IPatternManager))).Returns(manager.Object);

			this.element = namedElement.Object;
			this.provider.ServiceProvider = serviceProvider.Object;
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenResolvingNullUri_ThenThrowsException()
		{
			Assert.Throws<ArgumentNullException>(
				() => this.provider.ResolveUri(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenResolvingInvalidUri_ThenThrowsException()
		{
			Assert.Throws<ArgumentException>(
				() => this.provider.ResolveUri(new Uri("foo://")));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenResolvingValidUri_ThenResolvesProject()
		{
			var resolvedElement =
				this.provider.ResolveUri(new Uri(string.Format("solution://{0}", this.id.ToString())));

			Assert.NotNull(resolvedElement);
			Assert.Equal(resolvedElement.Id, this.id);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingAnUriWithNullNamedElement_ThenThrowsException()
		{
			Assert.Throws<ArgumentNullException>(() => this.provider.CreateUri(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingAnUriWithNamedElement_ThenReturnsUri()
		{
			var uri = this.provider.CreateUri(this.element);

			Assert.Equal(new Uri(string.Format("patternManager://{0}", this.element.Id)), uri);
		}
	}
}