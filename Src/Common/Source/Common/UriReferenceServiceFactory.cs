using System.Collections.Generic;

namespace NuPattern
{
    /// <summary>
    /// A factory for creating new <see cref="UriReferenceService"/> instances.
    /// </summary>
    public static class UriReferenceServiceFactory
    {
        /// <summary>
        /// Creates a new instance of the service with teh specified providers.
        /// </summary>
        /// <returns></returns>
        public static IUriReferenceService CreateService(IEnumerable<IUriReferenceProvider> providers)
        {
            Guard.NotNull(() => providers, providers);

            return new UriReferenceService(providers);
        }
    }
}
