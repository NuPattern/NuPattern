namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines a service to provide categories in a gallery.
	/// </summary>
	public class CategoryService : ICategoryService
	{
		private IGalleryRepository repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="CategoryService"/> class.
		/// </summary>
		public CategoryService()
		{
			this.repository = new ToolkitRepositoryDecorator(new VsGalleryRepository());
		}

		/// <summary>
		/// Gets the category tree.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="projectType">Type of the project.</param>
		/// <param name="templateType">Type of the template.</param>
		/// <param name="skus">The list of SKUs.</param>
		/// <param name="subSkus">The sub SKUs.</param>
		/// <param name="templateGroupIds">The template group ids.</param>
		/// <param name="visualStudioVersions">The versions.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The list of categories.</returns>
		public Category GetCategoryTree(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName)
		{
			return this.repository.GetCategoryTree(categoryId, projectType, templateType, skus, subSkus, templateGroupIds, visualStudioVersions, cultureName);
		}

		/// <summary>
		/// Gets the root categories.
		/// </summary>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The list of root categories.</returns>
		public Category[] GetRootCategories(string cultureName)
		{
			return this.repository.GetRootCategories(cultureName);
		}

		/// <summary>
		/// Gets the sub categories.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="projectType">Type of the project.</param>
		/// <param name="templateType">Type of the template.</param>
		/// <param name="skus">The list of SKUs.</param>
		/// <param name="subSkus">The sub skus.</param>
		/// <param name="templateGroupIds">The template group ids.</param>
		/// <param name="visualStudioVersions">The versions.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The list of subcategories for a given category.</returns>
		public Category[] GetSubCategories(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName)
		{
			return new Category[0];
		}
	}
}
