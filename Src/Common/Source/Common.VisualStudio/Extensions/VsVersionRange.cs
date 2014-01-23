using System;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
#if VSVER11 || VSVER12
    /// <summary>
    /// A wrapper for a VS VSIX registration <see cref="ExtMan.VersionRange"/>
    /// </summary>
    internal class VsVersionRange : IVersionRange
    {
        private ExtMan.VersionRange vsVersionRange;
        /// <summary>
        /// Creates a new instance of the <see cref="VsVersionRange"/> class.
        /// </summary>
        public VsVersionRange(ExtMan.VersionRange vsVersionRange)
        {
            this.vsVersionRange = vsVersionRange;
        }

        public bool IsMaximumInclusive
        {
            get { return this.vsVersionRange.IsMaximumInclusive; }
        }

        public bool IsMinimumInclusive
        {
            get { return this.vsVersionRange.IsMinimumInclusive; }
        }

        public Version Maximum
        {
            get { return this.vsVersionRange.Maximum; }
        }

        public Version Minimum
        {
            get { return this.vsVersionRange.Minimum; }
        }

        public bool Contains(Version value)
        {
            return this.vsVersionRange.Contains(value);
        }
    }
#endif
}
