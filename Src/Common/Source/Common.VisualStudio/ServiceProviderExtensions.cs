using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Defines extension methods related to <see cref="IServiceProvider"/>.
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Attemps to retrieve the given service, and returns null if it's not present 
        /// or it fails for whatever reason.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design")]
        public static T TryGetService<T>(this IServiceProvider serviceProvider)
        {
            try
            {
                return serviceProvider.GetService<T>();
            }
            catch { }

            return default(T);
        }

        /// <summary>
        /// Tries to get type-based services from the unmanaged service provider.
        /// </summary>
        /// <typeparam name="TReg">The type of the registration of the service to get.</typeparam>
        /// <typeparam name="TService">The type of the service to get.</typeparam>
        /// <param name="provider">The service provider.</param>
        /// <returns>The requested service, or a null reference if the service could not be located.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By Design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By Design")]
        public static TService TryGetService<TReg, TService>(this IServiceProvider provider)
        {
            Guard.NotNull(() => provider, provider);

            try
            {
                return (TService)provider.GetService(typeof(TReg));
            }
            catch { }

            return default(TService);
        }

        /// <summary>
        /// Gets type-based services from the unmanaged service provider.
        /// </summary>
        /// <typeparam name="TReg">The type of the registration of the service to get.</typeparam>
        /// <typeparam name="TService">The type of the service to get.</typeparam>
        /// <param name="provider">The service provider.</param>
        /// <returns>The requested service, or a null reference if the service could not be located.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By Design")]
        public static TService GetService<TReg, TService>(this IServiceProvider provider)
        {
            Guard.NotNull(() => provider, provider);

            return (TService)provider.GetService(typeof(TReg));
        }
    }
}