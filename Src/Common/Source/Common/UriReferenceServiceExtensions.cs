using System;
using System.Runtime.InteropServices;

namespace NuPattern
{
    /// <summary>
    /// Provides usability helpers over <see cref="IUriReferenceService"/>.
    /// </summary>
    public static class UriReferenceServiceExtensions
    {
        /// <summary>
        /// Tries to resolve the given URI, and returns null if it fails.
        /// </summary>
        [CLSCompliant(false)]
        public static T TryResolveUri<T>(this IUriReferenceService uriReferenceService, Uri uri) where T : class
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