using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;

namespace NuPattern.Runtime.References
{
    /// <summary>
    /// Extension methods for the <see cref="IReference"/> class.
    /// </summary>
    public static class ReferenceExtensions
    {
        /// <summary>
        /// Resolves the given reference to an item in the solution.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="referenceService"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static IItemContainer Resolve(this IReference reference, IFxrUriReferenceService referenceService)
        {
            Guard.NotNull(() => reference, reference);
            Guard.NotNull(() => referenceService, referenceService);

            if (String.IsNullOrEmpty(reference.Value))
            {
                return null;
            }

            // Ensure we can create a valid Uri from the reference value
            Uri referenceUri;
            if (Uri.TryCreate(reference.Value, UriKind.RelativeOrAbsolute, out referenceUri) == false)
            {
                return null;
            }

            // Get the solution item
            return referenceService.TryResolveUri<IItemContainer>(referenceUri);
        }

    }
}
