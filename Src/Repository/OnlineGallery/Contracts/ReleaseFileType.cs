using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Release file type.
	/// </summary>
	[DataContract(Name = "ReleaseFileType", Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class ReleaseFileType : IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the release file type id.
		/// </summary>
		[DataMember]
		public string Id { get; set; }
	}
}
