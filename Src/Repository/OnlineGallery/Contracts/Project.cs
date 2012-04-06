using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Patterning.Repository.OnlineGallery
{
	/// <summary>
	/// Project contract.
	/// </summary>
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Galleries.Domain.Model")]
	public partial class Project : IExtensibleDataObject
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Project"/> class.
		/// </summary>
		public Project()
		{
			this.Metadata = new Dictionary<string, string>();
			this.Metadata["SizeInBytes"] = "1";
			this.Metadata["LCID"] = "1033";
			this.Metadata["LastModified"] = String.Empty;
			this.Metadata["Type"] = "Control";
			this.Metadata["ReferralUrl"] = String.Empty;
			this.Metadata["Author"] = String.Empty;
			this.Metadata["MoreInfoUrl"] = String.Empty;
			this.Metadata["ReportAbuseUrl"] = String.Empty;
			this.Metadata["PreviewImage"] = String.Empty;
			this.Metadata["Icon"] = String.Empty;
			this.Metadata["VsixId"] = String.Empty;
			this.Metadata["VsixVersion"] = String.Empty;
			this.Metadata["VsixReferences"] = String.Empty;
			this.Metadata["SubType"] = String.Empty;
			this.Metadata["ProjectType"] = String.Empty;
			this.Metadata["TemplateGroupID"] = String.Empty;
			this.Metadata["DefaultName"] = String.Empty;
			this.Metadata["SupportsCodeSeparation"] = String.Empty;
			this.Metadata["SupportsMasterPage"] = String.Empty;
			this.Metadata["ProjectTypeFriendly"] = String.Empty;
			this.Metadata["VSEditions"] = String.Empty;
		}

		/// <summary>
		/// Gets or sets the structure that contains extra data.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</returns>
		public ExtensionDataObject ExtensionData { get; set; }

		/// <summary>
		/// Gets or sets the affiliate id.
		/// </summary>
		/// <value>The affiliate id.</value>
		[DataMember]
		public int AffiliateId { get; set; }

		/// <summary>
		/// Gets or sets the categories.
		/// </summary>
		/// <value>The categories.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		public Category[] Categories { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>The created date.</value>
		[DataMember]
		public DateTime CreatedDate { get; set; }

		/// <summary>
		/// Gets or sets the current release.
		/// </summary>
		/// <value>The current release.</value>
		[DataMember]
		public Release CurrentRelease { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether discussions is enabled.
		/// </summary>
		/// <value>This is <c>true</c> if discussions is enabled; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool DiscussionsEnabled { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the file release is enabled.
		/// </summary>
		/// <value>This is <c>true</c> if the file release is enabled; otherwise, <c>false</c>.</value>
		[DataMember]
		public bool FileReleaseEnabled { get; set; }

		/// <summary>
		/// Gets or sets the project id.
		/// </summary>
		/// <value>The project id.</value>
		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the initial name of the project.
		/// </summary>
		/// <value>The initial name of the project.</value>
		[DataMember]
		public string InitialProjectName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is published.
		/// </summary>
		/// <value>
		/// This is <c>true</c> if this instance is published; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool IsPublished { get; set; }

		/// <summary>
		/// Gets the metadata.
		/// </summary>
		/// <value>The metadata.</value>
		[DataMember]
		public Dictionary<string, string> Metadata { get; private set; }

		/// <summary>
		/// Gets or sets the project name.
		/// </summary>
		/// <value>The project name.</value>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the releases.
		/// </summary>
		/// <value>The releases.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		public Release[] Releases { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the source code browsing is enabled.
		/// </summary>
		/// <value>
		/// This is <c>true</c> if the source code browsing is enabled; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool SourceCodeBrowsingEnabled { get; set; }

		/// <summary>
		/// Gets or sets the project tags.
		/// </summary>
		/// <value>The project tags.</value>
		[DataMember]
		[SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "WCF data contract")]
		public string[] Tags { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		[DataMember]
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the work item tracking enabled.
		/// </summary>
		/// <value>
		/// This is <c>true</c> if the work item tracking enabled; otherwise, <c>false</c>.
		/// </value>
		[DataMember]
		public bool WorkItemTrackingEnabled { get; set; }
	}
}
