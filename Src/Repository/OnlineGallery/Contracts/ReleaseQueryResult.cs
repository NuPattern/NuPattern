using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Release query result.
	/// </summary>
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/VsGallery.WebServices")]
	public partial class ReleaseQueryResult : IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the releases.
		/// </summary>
		/// <value>The releases.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		//// TODO use a List<ReleaseFile> instead and get rid of the setter
		public Release[] Releases { get; set; }

		/// <summary>
		/// Gets or sets the total count.
		/// </summary>
		/// <value>The total count.</value>
		[DataMember]
		public int TotalCount { get; set; }
	}
}
