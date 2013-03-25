using System;
using System.IO;
using System.Linq;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Definitions for URI's for Text Templates
    /// </summary>
    public static class TextTemplateUri
    {
        /// <summary>
        /// The scheme of the URI
        /// </summary>
        public const string UriScheme = "t4";

        /// <summary>
        /// The host to use in a uri that is resolved relative to a VSIX extension.
        /// </summary>
        public const string ExtensionRelativeHost = "extension";

        /// <summary>
        /// The prefix for all Uris from this provider.
        /// </summary>
        public const string UriHostPrefix = UriScheme + "://" + ExtensionRelativeHost + "/";

        /// <summary>
        /// Returns the filename component of the Uri.
        /// </summary>
        /// <param name="uri">A URI in the form 't4://extension/{id}/{relative-path-to-t4-file}'</param>
        public static string ParseFileName(System.Uri uri)
        {
            var segments = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split(Path.AltDirectorySeparatorChar);
            if (segments.Length < 2)
            {
                return null;
            }

            return segments.Last();
        }
    }
}
