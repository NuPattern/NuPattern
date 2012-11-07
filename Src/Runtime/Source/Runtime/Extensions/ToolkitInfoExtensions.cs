using System;
using Microsoft.VisualStudio.ExtensionManager;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Provides helper methods for dealing with <see cref="IToolkitInfo"/> instances.
	/// </summary>
	public static class ToolkitInfoExtensions
	{
		/// <summary>
		/// Converts an instance of an <see cref="IToolkitInfo"/> into a 
		/// Visual Studio <see cref="IRepositoryEntry"/>.
		/// </summary>
		[CLSCompliant(false)]
		public static IRepositoryEntry AsRepositoryEntry(this IToolkitInfo info)
		{
			Guard.NotNull(() => info, info);

			return new FakeRepositoryEntry { DownloadUrl = info.DownloadUri.OriginalString };
		}

		/// <summary>
		/// Defines a repository entry.
		/// </summary>
		private class FakeRepositoryEntry : IRepositoryEntry
		{
			/// <summary>
			/// Gets or sets the download URL.
			/// </summary>
			/// <value>The download URL.</value>
			public string DownloadUrl { get; set; }

			/// <summary>
			/// Gets or sets the vsix references.
			/// </summary>
			/// <value>The vsix references.</value>
			public string VsixReferences { get; set; }

#if VSVER11
            public string DownloadUpdateUrl
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
#endif
        }
	}
}
