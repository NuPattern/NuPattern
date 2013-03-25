using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.References
{
    /// <summary>
    /// Defines a MEF Exported attribute to identify providers of Reference Kinds.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "MEF attributes are better non-sealed for extensibility.")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [CLSCompliant(false)]
    public class ReferenceKindProviderAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceKindProviderAttribute"/> class.
        /// </summary>
        public ReferenceKindProviderAttribute()
            : base(typeof(IReferenceKindProvider))
        {
        }
    }
}
