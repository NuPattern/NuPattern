using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery.UnitTests
{
	public class VsGalleryRepositorySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

        [Ignore]
        [TestClass]
		public class GivenAnOnlineGallery
		{
			private IGalleryRepository repository;

			[TestInitialize]
			public void Initialize()
			{
				this.repository = new VsGalleryRepository(
					() => new ServiceClient<ICategoryService>(
						new WSHttpBinding(SecurityMode.None),
						new EndpointAddress("http://visualstudiogallery.msdn.microsoft.com/services/Category2.svc")),
					() => new ServiceClient<IReleaseService>(
						new WSHttpBinding(SecurityMode.None),
						new EndpointAddress("http://visualstudiogallery.msdn.microsoft.com/services/Release3.svc")));
			}

			[TestMethod]
			public void WhenRootCategoriesRetrieved_ThenReturnsNonEmpty()
			{
				var root = this.repository.GetRootCategories("en-us");

				Assert.NotNull(root);
				Assert.True(root.Any());
			}

			[TestMethod]
			public void WhenCategoryTreeRetrieved_ThenReturnsNonEmpty()
			{
				var root = this.repository.GetRootCategories("en-us");
				var tree = this.repository.GetCategoryTree(root[0].Id, string.Empty, string.Empty, null, null, null, null, "en-us");

				Assert.NotNull(tree);
			}
		}
	}
}