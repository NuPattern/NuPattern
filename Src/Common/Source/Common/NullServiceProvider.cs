using System;

namespace NuPattern
{
    /// <summary>
    /// A null service provider
    /// </summary>
    public class NullServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static IServiceProvider Instance { get; private set; }

        static NullServiceProvider()
        {
            Instance = new NullServiceProvider();
        }

        private NullServiceProvider()
        {
        }

        /// <summary>
        /// Gets the specified service from the provider.
        /// </summary>
        /// <param name="serviceType">the type of the service to fetch</param>
        public object GetService(Type serviceType)
        {
            return null;
        }
    }
}
