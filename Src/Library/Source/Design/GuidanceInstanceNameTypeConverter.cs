using System.ComponentModel;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime.Guidance;

namespace NuPattern.Library.Design
{
    /// <summary>
    /// Provides a standard values list of the instantiated guidance extensions in the 
    /// <see cref="IGuidanceManager"/> service.
    /// </summary>
    internal class GuidanceInstanceNameTypeConverter : TypeConverter
    {
        private static readonly ITracer tracer = Tracer.Get<GuidanceInstanceNameTypeConverter>();

        /// <summary>
        /// Returns <see langword="true"/> if there's a <see cref="IGuidanceManager"/> service available 
        /// from the current <paramref name="context"/>.
        /// </summary>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            var guidanceManager = context.GetService<IGuidanceManager>();

            if (guidanceManager != null)
            {
                return true;
            }
            else
            {
                tracer.Warn(Resources.ServiceUnavailable, typeof(IGuidanceManager));
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
        /// Returns the list of instantiated guidance extensions.
        /// </summary>
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var guidanceManager = context.GetService<IGuidanceManager>();

            return new StandardValuesCollection(guidanceManager.InstantiatedGuidanceExtensions.Select(x => x.InstanceName).ToList());
        }
    }
}
