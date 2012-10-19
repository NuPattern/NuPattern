using System.Linq;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines the Toolkit gallery.
	/// </summary>
	public class ToolkitRepositoryDecorator : IGalleryRepository
	{
		private const int ToolsCategoryId = 3;
		private const int CustomCategoryId = 999999;

		private IGalleryRepository repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ToolkitRepositoryDecorator"/> class.
		/// </summary>
		/// <param name="repository">The repository.</param>
		public ToolkitRepositoryDecorator(IGalleryRepository repository)
		{
			this.repository = repository;
		}

		/// <summary>
		/// Gets the root categories.
		/// </summary>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The root categories for the repository.</returns>
		public Category[] GetRootCategories(string cultureName)
		{
			return this.repository.GetRootCategories(cultureName);
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
			var category = this.repository.GetCategoryTree(categoryId, projectType, templateType, skus, subSkus, templateGroupIds, visualStudioVersions, cultureName);

			if (category.Id == ToolsCategoryId)
			{
				// Append our custom category.
				category.Children = category.Children.Concat(new[] 
				{ 
 					new Category { Title = "Toolkits, Id = CustomCategoryId } 
				}).OrderBy(c => c.Title).ToArray();
			}

			return category;
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
			if (!string.IsNullOrEmpty(whereClause) && whereClause.Contains("Category.Id = " + CustomCategoryId))
			{
				whereClause = whereClause.Replace("Category.Id = " + CustomCategoryId, "Project.Metadata['ContentTypes'] LIKE '%FactorySchema%'");
			}

			return this.repository.Search(searchText, whereClause, orderByClause, skip, take);
		}
	}
}