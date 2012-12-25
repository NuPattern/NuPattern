using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Defines the basic extension information for a toolkit.
	/// </summary>
	public interface IToolkitInfo
	{
		/// <summary>
		/// Gets the identifier for the extension.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the name of the extension.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the description of the extension.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets the author of the extension.
		/// </summary>
		string Author { get; }

		/// <summary>
		/// Gets the version of the extension.
		/// </summary>
		Version Version { get; }

		/// <summary>
		/// Gets the URL to download the extension from.
		/// </summary>
		//// TODO replace this property by repository
		Uri DownloadUri { get; }
	}
}