using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Provides usability helpers over <see cref="IFxrUriReferenceService"/>.
    /// </summary>
    public static class IFxrUriReferenceServiceExtensions
    {
        /// <summary>
        /// Tries to resolve the given URI, and returns null if it fails.
        /// </summary>
        [CLSCompliant(false)]
        public static T TryResolveUri<T>(this IFxrUriReferenceService uriReferenceService, Uri uri) where T : class
        {
            Guard.NotNull(() => uriReferenceService, uriReferenceService);

            try
            {
                return uriReferenceService.ResolveUri<T>(uri);
            }
            catch (UriFormatException)
            {
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
            catch (COMException)
            {
                return null;
            }
        }
    }
}