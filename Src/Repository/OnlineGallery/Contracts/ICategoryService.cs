using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Category service.
	/// </summary>
	[ServiceContract]
	public interface ICategoryService
	{
		/// <summary>
		/// Gets the category tree.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="projectType">Type of the project.</param>
		/// <param name="templateType">Type of the template.</param>
		/// <param name="skus">The list of SKU.</param>
		/// <param name="subSkus">The sub skus.</param>
		/// <param name="templateGroupIds">The template group ids.</param>
		/// <param name="visualStudioVersions">The visual studio versions.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The list of categories.</returns>
		[OperationContract(
			Action = "http://tempuri.org/ICategoryService2/GetCategoryTree",
			ReplyAction = "http://tempuri.org/ICategoryService2/GetCategoryTreeResponse")]
		[FaultContract(typeof(System.Guid),
			Action = "http://galleries.msdn.microsoft.com/faults/generic",
			Name = "guid",
			Namespace = "http://schemas.microsoft.com/2003/10/Serialization/")]
		Category GetCategoryTree(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName);

		/// <summary>
		/// Gets the root categories.
		/// </summary>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The list of root categories.</returns>
		[OperationContract(
			Action = "http://tempuri.org/ICategoryService2/GetRootCategories",
			ReplyAction = "http://tempuri.org/ICategoryService2/GetRootCategoriesResponse")]
		[FaultContract(typeof(System.Guid),
			Action = "http://galleries.msdn.microsoft.com/faults/generic",
			Name = "guid",
			Namespace = "http://schemas.microsoft.com/2003/10/Serialization/")]
		Category[] GetRootCategories(string cultureName);

		/// <summary>
		/// Gets the subcategories.
		/// </summary>
		/// <param name="categoryId">The category id.</param>
		/// <param name="projectType">Type of the project.</param>
		/// <param name="templateType">Type of the template.</param>
		/// <param name="skus">The skus for the category.</param>
		/// <param name="subSkus">The sub-skus.</param>
		/// <param name="templateGroupIds">The template group ids.</param>
		/// <param name="visualStudioVersions">The visual studio versions.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <returns>The subcategories for the given category.</returns>
		[OperationContract(
			Action = "http://tempuri.org/ICategoryService2/GetSubCategories",
			ReplyAction = "http://tempuri.org/ICategoryService2/GetSubCategoriesResponse")]
		[FaultContract(typeof(System.Guid),
			Action = "http://galleries.msdn.microsoft.com/faults/generic",
			Name = "guid",
			Namespace = "http://schemas.microsoft.com/2003/10/Serialization/")]
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SubCategories", Justification = "WCF operation contract")]
		Category[] GetSubCategories(int categoryId, string projectType, string templateType, string[] skus, string[] subSkus, int[] templateGroupIds, int[] visualStudioVersions, string cultureName);
	}
}
