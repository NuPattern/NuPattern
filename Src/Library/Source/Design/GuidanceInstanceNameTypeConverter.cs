using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Provides a standard values list of the instantiated feature extensions in the 
    /// <see cref="IFeatureManager"/> service.
    /// </summary>
    internal class GuidanceInstanceNameTypeConverter : TypeConverter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceInstanceNameTypeConverter>();

        /// <summary>
        /// Returns <see langword="true"/> if there's a <see cref="IFeatureManager"/> service available 
        /// from the current <paramref name="context"/>.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            var featureManager = context.GetService<IFeatureManager>();

            if (featureManager != null)
            {
                return true;
            }
            else
            {
                tracer.TraceWarning(Resources.ServiceUnavailable, typeof(IFeatureManager));
                return false;
            }
        }

        /// <summary>
        /// Returns <see langword="false"/>.
        /// </summary>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }

        /// <summary>
        /// Returns the list of instantiated feature extensions.
        /// </summary>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var featureManager = context.GetService<IFeatureManager>();

            return new StandardValuesCollection(featureManager.InstantiatedFeatures.Select(x => x.InstanceName).ToList());
        }
    }
}
