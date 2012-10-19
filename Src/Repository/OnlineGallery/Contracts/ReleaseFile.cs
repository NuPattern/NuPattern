using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Release file.
	/// </summary>
	[DataContract(Name = "ReleaseFile", Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class ReleaseFile : IExtensibleDataObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReleaseFile"/> class.
		/// </summary>
		public ReleaseFile()
		{
			this.Metadata = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the attachment.
		/// </summary>
		/// <value>The attachment.</value>
		[DataMember]
		public FileAttachment Attachment { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the download count.
		/// </summary>
		/// <value>The download count.</value>
		[DataMember]
		public int DownloadCount { get; set; }

		/// <summary>
		/// Gets or sets the release file id.
		/// </summary>
		/// <value>The release file id.</value>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		[DataMember]
		public Dictionary<string, string> Metadata { get; private set; }

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		[DataMember]
		public Release Parent { get; set; }

		/// <summary>
		/// Gets or sets the release file type.
		/// </summary>
		/// <value>The release file type.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "WCF data contract")]
		public ReleaseFileType Type { get; set; }
	}
}
