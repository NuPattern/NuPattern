using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.References
{
    /// <summary>
    /// Provides type information for the <see cref="GuidanceReference"/> reference kind.
    /// </summary>
    [ReferenceKindProvider]
    [DisplayNameResource(@"GuidanceReference_DisplayName", typeof(Resources))]
    [DescriptionResource(@"GuidanceReference_Description", typeof(Resources))]
    [TypeConverter(typeof(GuidanceExtensionsTypeConverter))]
    [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
    public class GuidanceReference : ReferenceKindProvider<GuidanceReference, string>
    {
        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IGuidanceExtension> GetResolvedReferences(IProductElement element, IGuidanceManager guidanceManager)
        {
            return GetResolvedReferences(element, guidanceManager, r => true);
        }

        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IGuidanceExtension> GetResolvedReferences(IProductElement element, IGuidanceManager guidanceManager, Func<IReference, bool> whereFilter)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => whereFilter, whereFilter);

            return GetReferenceValues(element, whereFilter)
                .Select(reference => guidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(f => f.InstanceName.Equals(reference, StringComparison.OrdinalIgnoreCase)))
                .Where(item => item != null);
        }
    }
}