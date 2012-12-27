using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Common.UnitTests
{
	[TestClass]
	public class ProxyServiceProviderSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		private IServiceProvider parent;
		private ProxyServiceProvider target;

		[TestInitialize]
		public void Initialize()
		{
			this.parent = new Mock<IServiceProvider>().Object;
			this.target = new ProxyServiceProvider(this.parent);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenCreatingNewWithNullServiceProvider_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new ProxyServiceProvider(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenGettingServiceWithNullServiceType_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => this.target.GetService(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenAddingServiceWithNullServiceType_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => this.target.AddService(null, new object()));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenAddingServiceWithNullServiceInstance_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => this.target.AddService(typeof(object), null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenRemovingServiceWithNullServiceProvider_ThenThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => this.target.RemoveService(null));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenGettingServiceAndNotFound_ThenReturnsNull()
		{
			Assert.Null(this.target.GetService(typeof(object)));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenGettingServiceNotContainedLocally_ThenReturnsServiceFromParentProvider()
		{
			var expected = new object();
			Mock.Get(this.parent).Setup(x => x.GetService(typeof(object))).Returns(expected);

			var service = this.target.GetService(typeof(object));

			Assert.Equal(expected, service);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenGettingServicePreviouslyAdded_ThenReturnsService()
		{
			var expected = new object();
			this.target.AddService(typeof(object), expected);

			var service = this.target.GetService(typeof(object));

			Assert.Equal(expected, service);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenGettingServiceAndServiceDefinedInBoth_ThenRetursLocalService()
		{
			Mock.Get(this.parent).Setup(x => x.GetService(typeof(object))).Returns(new object());

			var expected = new object();
			this.target.AddService(typeof(object), expected);

			var service = this.target.GetService(typeof(object));

			Assert.Equal(expected, service);
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenRemovingService_ThenGetServiceReturnsNull()
		{
			this.target.AddService(typeof(object), new object());

			this.target.RemoveService(typeof(object));

			Assert.Null(this.target.GetService(typeof(object)));
		}

		[TestMethod, TestCategory("Unit")]
		public void WhenRemovingServiceDefinedInParentProvider_ThenGetServiceReturnsService()
		{
			var expected = new object();
			Mock.Get(this.parent).Setup(x => x.GetService(typeof(object))).Returns(expected);
			this.target.AddService(typeof(object), new object());

			this.target.RemoveService(typeof(object));

			var service = this.target.GetService(typeof(object));

			Assert.Equal(expected, service);
		}
	}
}