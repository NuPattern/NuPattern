using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Store.Properties;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Helper methods for products.
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Determines whether the given pattern instance is a valid extension for any 
        /// of the given extension points.
        /// </summary>
        /// <param name="targetExtension">The pattern instance to verify.</param>
        /// <param name="supportedExtensionPoints">The supported extension points to verify against.</param>
        /// <returns>
        /// Returns <see langword="true"/> if the given pattern provides an extension point in <paramref name="supportedExtensionPoints"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsValidExtension(this IProduct targetExtension, IEnumerable<IExtensionPointInfo> supportedExtensionPoints)
        {
            Guard.NotNull(() => targetExtension, targetExtension);
            Guard.NotNull(() => supportedExtensionPoints, supportedExtensionPoints);

            if (targetExtension.Info == null)
            {
                return false;
            }
            else
            {
                return supportedExtensionPoints.Any(ep => targetExtension.Info.ProvidedExtensionPoints.Any(
                        e => e.ExtensionPointId.Equals(ep.RequiredExtensionPointId, StringComparison.OrdinalIgnoreCase)));
            }
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if the given pattern instance does not 
        /// provide any of the supported extension points.
        /// </summary>
        /// <param name="targetExtension">The pattern instance to verify.</param>
        /// <param name="supportedExtensionPoints">The supported extension points to verify against.</param>
        public static void ThrowIfInvalidExtension(this IProduct targetExtension, IEnumerable<IExtensionPointInfo> supportedExtensionPoints)
        {
            Guard.NotNull(() => targetExtension, targetExtension);

            if (!supportedExtensionPoints.Any())
            {
                throw new ArgumentException(string.Format(
                    Resources.Culture,
                    Resources.Product_NoExtensionPointsDefined,
                    targetExtension.DefinitionName));
            }

            if (!targetExtension.IsValidExtension(supportedExtensionPoints))
            {
                throw new ArgumentException(string.Format(
                    Resources.Culture,
                    Resources.Product_InvalidExtension,
                    targetExtension.DefinitionName));
            }
        }
    }
}
