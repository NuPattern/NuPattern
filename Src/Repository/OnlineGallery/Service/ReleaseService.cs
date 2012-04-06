namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines a service to provide releases in a gallery.
	/// </summary>
	public class ReleaseService : IReleaseService
	{
		private IGalleryRepository repository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ReleaseService"/> class.
		/// </summary>
		public ReleaseService()
		{
			this.repository = new ToolkitRepositoryDecorator(new VsGalleryRepository());
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
		/// The releases that satify the selected filters.
		/// </returns>
		public ReleaseQueryResult Search(string searchText, string whereClause, string orderByClause, int? skip, int? take)
		{
			return this.repository.Search(searchText, whereClause, orderByClause, skip, take);
		}
	}
}