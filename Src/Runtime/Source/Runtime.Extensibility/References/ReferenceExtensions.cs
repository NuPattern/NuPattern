using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NuPattern.VisualStudio.Solution;

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
        public static IItemContainer Resolve(this IReference reference, IUriReferenceService referenceService)
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

        /// <summary>
        /// Adds the specified tag to the <see cref="IReference.Tag"/> if it does not already exist.
        /// </summary>
        public static void AddTag(this IReference reference, string tag)
        {
            Guard.NotNull(()=> reference, reference);

            if (!string.IsNullOrEmpty(tag))
            {
                var tags = new List<string>();
                if (!string.IsNullOrEmpty(reference.Tag))
                {
                    tags = reference.Tag.Split(PathResolver.ReferenceTagDelimiter)
                        .Select(t => t.Trim(new[] {' '}))
                        .Where(t => !string.IsNullOrEmpty(t))
                        .ToList();
                }

                if (!tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    tags.Add(tag);
                    reference.Tag = string.Join(PathResolver.ReferenceTagDelimiter.ToString(CultureInfo.InvariantCulture), 
                        tags);
                }
            }
        }
    }
}
