using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Category contract.
	/// </summary>
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class Category : IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the children.
		/// </summary>
		/// <value>The children.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		public Category[] Children { get; set; }

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		[DataMember]
		public Category Parent { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		[DataMember]
		public string Title { get; set; }
	}
}
