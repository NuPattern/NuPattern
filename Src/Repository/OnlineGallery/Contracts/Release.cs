using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Release contract.
	/// </summary>
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class Release : IExtensibleDataObject
	{
		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the date released.
		/// </summary>
		/// <value>The date released.</value>
		[DataMember]
		public DateTime? DateReleased { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the files.
		/// </summary>
		/// <value>The files.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		//// TODO use a List<ReleaseFile> instead and get rid of the setter
		public ReleaseFile[] Files { get; set; }

		/// <summary>
		/// Gets or sets the release id.
		/// </summary>
		/// <value>The release id.</value>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is current release.
		/// </summary>
		/// <value>
		/// This is <c>true</c> if this instance is current release; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsCurrentRelease { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is displayed on home page.
		/// </summary>
		/// <value>
		/// This is <c>true</c> if this instance is displayed on home page; otherwise, <c>false</c>.
		/// </value>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "HomePage", Justification = "WCF data contract")]
		[DataMember]
		//// TODO use a List<ReleaseFile> instead and get rid of the setter
		public bool IsDisplayedOnHomePage { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is public.
		/// </summary>
		/// <value>This is <c>true</c> if this instance is public; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the release name.
		/// </summary>
		/// <value>The release name.</value>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the project.
		/// </summary>
		/// <value>The project.</value>
		[DataMember]
		public Project Project { get; set; }

		/// <summary>
		/// Gets or sets the rating.
		/// </summary>
		/// <value>The rating.</value>
		[DataMember]
		public double Rating { get; set; }

		/// <summary>
		/// Gets or sets the ratings count.
		/// </summary>
		/// <value>The ratings count.</value>
		[DataMember]
		public int RatingsCount { get; set; }

		/// <summary>
		/// Gets or sets the reviews count.
		/// </summary>
		/// <value>The reviews count.</value>
		[DataMember]
		public int ReviewsCount { get; set; }
	}
}
