namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines a repository to implement different galleries.
	/// </summary>
	public interface IGalleryRepository
	{
		/// <summary>
		/// Gets the root categories.
		/// </summary>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The root categories for the repository.</returns>
		Category[] GetRootCategories(string cultureName);

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
		Category GetCategoryTree(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName);

		/// <summary>
		/// Searches the specified search text.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="orderByClause">The order by clause.</param>
		/// <param name="skip">The quantity of rows to skip.</param>
		/// <param name="take">The quantity of rows to take.</param>
		/// <returns>The releases that satiefies the given filters.</returns>
		ReleaseQueryResult Search(string searchText, string whereClause, string orderByClause, int? skip, int? take);
	}
}
