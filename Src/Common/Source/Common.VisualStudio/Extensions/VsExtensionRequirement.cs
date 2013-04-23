using System.Collections.Generic;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
#if VSVER11
    /// <summary>
    /// A wrapper for a VS VSIX registration requirement
    /// </summary>
    internal class VsExtensionRequirement : IExtensionRequirement
    {
        private ExtMan.IExtensionRequirement vsRequirement;

        /// <summary>
        /// Creates a new instance of the <see cref="VsExtensionRequirement"/> class.
        /// </summary>
        public VsExtensionRequirement(ExtMan.IExtensionRequirement vsRequirement)
        {
            this.vsRequirement = vsRequirement;
        }

        IDictionary<string, string> IExtensionRequirement.Attributes
        {
            get { return this.vsRequirement.Attributes; }
        }

        string IExtensionRequirement.Identifier
        {
            get { return this.vsRequirement.Identifier; }
        }

        IVersionRange IExtensionRequirement.VersionRange
        {
            get { return new VsVersionRange(this.vsRequirement.VersionRange); }
        }
    }
#endif
}