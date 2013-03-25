using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Design;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility.Design;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.References
{
    /// <summary>
    /// Provides type information for the <see cref="GuidanceReference"/> reference kind.
    /// </summary>
    [ReferenceKindProvider]
    [DisplayNameResource("GuidanceReference_DisplayName", typeof(Resources))]
    [DescriptionResource("GuidanceReference_Description", typeof(Resources))]
    [TypeConverter(typeof(FeatureExtensionsTypeConverter))]
    [Editor(typeof(StandardValuesEditor), typeof(UITypeEditor))]
    public class GuidanceReference : ReferenceKindProvider<GuidanceReference, string>
    {
        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IFeatureExtension> GetResolvedReferences(IProductElement element, IFeatureManager featureManager)
        {
            return GetResolvedReferences(element, featureManager, r => true);
        }

        /// <summary>
        /// Returns all the <see cref="IReference"/>s for the current element that can be converted to the given type and are not empty.
        /// </summary>
        [CLSCompliant(false)]
        public static IEnumerable<IFeatureExtension> GetResolvedReferences(IProductElement element, IFeatureManager featureManager, Func<IReference, bool> whereFilter)
        {
            Guard.NotNull(() => element, element);
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNull(() => whereFilter, whereFilter);

            return GetReferences(element, whereFilter)
                .Select(reference => featureManager.InstantiatedFeatures.FirstOrDefault(f => f.InstanceName.Equals(reference, StringComparison.OrdinalIgnoreCase)))
                .Where(item => item != null);
        }
    }
}