using System.ServiceModel;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Defines the release for a VSIX.
	/// </summary>
	[ServiceContract(SessionMode = SessionMode.NotAllowed)]
	public interface IReleaseService
	{
		/// <summary>
		/// Searches the specified search text.
		/// </summary>
		/// <param name="searchText">The search text.</param>
		/// <param name="whereClause">The where clause.</param>
		/// <param name="orderByClause">The order by clause.</param>
		/// <param name="skip">The quantity of rows to skip.</param>
		/// <param name="take">The quantity of rows to take.</param>
		/// <returns>The releases that satify the selected filters.</returns>
		[OperationContract(
			Action = "http://tempuri.org/IReleaseService3/Search",
			ReplyAction = "http://tempuri.org/IReleaseService3/SearchResponse")]
		[FaultContractAttribute(typeof(System.Guid),
			Action = "http://galleries.msdn.microsoft.com/faults/generic",
			Name = "guid",
			Namespace = "http://schemas.microsoft.com/2003/10/Serialization/")]
		ReleaseQueryResult Search(string searchText, string whereClause, string orderByClause, int? skip, int? take);
	}
}
