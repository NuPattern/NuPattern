using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Default implementation of <see cref="IToolkitInfo"/>.
	/// </summary>
	//// TODO this class should be removed as is only for serialization of toolkit references
	public class ToolkitInfo : IToolkitInfo
	{
		/// <summary>
		/// Gets or sets the identifier for the extension.
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets or sets the name of the extension.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the description of the extension.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets a Url to download the extension from.
		/// </summary>
		//// TODO replace this property by repository
		public Uri DownloadUri { get; set; }

		/// <summary>
		/// Gets or sets the author of the extension.
		/// </summary>
		/// <value></value>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the version of the extension.
		/// </summary>
		public Version Version { get; set; }
	}
}