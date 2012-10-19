using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines repositry for the Visual Studio gallery.
	/// </summary>
	public class VsGalleryRepository : IGalleryRepository
	{
		private Func<ServiceClient<ICategoryService>> categoriesClientFactory;
		private Func<ServiceClient<IReleaseService>> releasesClientFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="VsGalleryRepository"/> class.
		/// </summary>
		public VsGalleryRepository()
		{
			this.categoriesClientFactory = () => new ServiceClient<ICategoryService>();
			this.releasesClientFactory = () => new ServiceClient<IReleaseService>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VsGalleryRepository"/> class.
		/// </summary>
		/// <param name="categoriesClientFactory">The categories client factory.</param>
		/// <param name="releasesClientFactory">The releases client factory.</param>
		[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "By WCF design")]
		public VsGalleryRepository(Func<ServiceClient<ICategoryService>> categoriesClientFactory, Func<ServiceClient<IReleaseService>> releasesClientFactory)
		{
			this.categoriesClientFactory = categoriesClientFactory;
			this.releasesClientFactory = releasesClientFactory;
		}

		/// <summary>
		/// Gets the root categories.
		/// </summary>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The root categories.</returns>
		public Category[] GetRootCategories(string cultureName)
		{
			using (var client = this.categoriesClientFactory())
			{
				return client.Channel.GetRootCategories(cultureName);
			}
		}

		/// <summary>
		/// Gets the category tree.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="projectType">Type of the project.</param>
		/// <param name="templateType">Type of the template.</param>
		/// <param name="skus">The list of SKUs.</param>
		/// <param name="subSkus">The sub skus.</param>
		/// <param name="templateGroupIds">The template group ids.</param>
		/// <param name="visualStudioVersions">The visual studio versions.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The category tree.</returns>
		public Category GetCategoryTree(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName)
		{
			using (var client = this.categoriesClientFactory())
			{
				return client.Channel.GetCategoryTree(categoryId, projectType, templateType, skus, subSkus, templateGroupIds, visualStudioVersions, cultureName);
			}
		}

		/// <summary>
		/// Searches the specified search text.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="orderByClause">The order by clause.</param>
		/// <param name="skip">The quantity of rows to skip.</param>
		/// <param name="take">The quantity of rows to take.</param>
		/// <returns>
		/// The releases that satiefies the given filters.
		/// </returns>
		public ReleaseQueryResult Search(string searchText, string whereClause, string orderByClause, int? skip, int? take)
		{
			using (var client = this.releasesClientFactory())
			{
				return client.Channel.Search(searchText, whereClause, orderByClause, skip, take);
			}
		}
	}
}
