using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// File attachement.
	/// </summary>
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class FileAttachment : IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		[DataMember]
		public string FileName { get; set; }

		/// <summary>
		/// Gets or sets the file attachment id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the upload date.
		/// </summary>
		[DataMember]
		public DateTime UploadDate { get; set; }
	}
}
