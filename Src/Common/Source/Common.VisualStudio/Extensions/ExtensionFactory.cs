using System;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A factory for creating extensions.
    /// </summary>
    [CLSCompliant(false)]
    public static class ExtensionFactory
    {
        /// <summary>
        /// Creates a new extension for the given VS extension
        /// </summary>
        /// <param name="vsExtension">The VS extension</param>
        /// <returns></returns>
        public static IExtension CreateExtension(ExtMan.IExtension vsExtension)
        {
            return new VsExtension(vsExtension);
        }

        /// <summary>
        /// CReates a new installed extension for the given installed VS extension
        /// </summary>
        /// <param name="installedVsExtension">The installed VS extension</param>
        /// <returns></returns>
        public static IInstalledExtension CreateInstalledExtension(ExtMan.IInstalledExtension installedVsExtension)
        {
            return new VsInstalledExtension(installedVsExtension);
        }
    }
}
