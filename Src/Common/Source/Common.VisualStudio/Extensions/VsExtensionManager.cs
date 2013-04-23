using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using ExtMan = Microsoft.VisualStudio.ExtensionManager;

namespace NuPattern.VisualStudio.Extensions
{
    /// <summary>
    /// A wrapper for a VS VSIX manager
    ///  </summary>
    [Export(typeof(IExtensionManager))]
    internal class VsExtensionManager : IExtensionManager
    {
        private ExtMan.IVsExtensionManager vsManager;

        /// <summary>
        /// Creates a new instance of the <see cref="VsExtensionManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public VsExtensionManager(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            this.vsManager = serviceProvider.GetService<ExtMan.SVsExtensionManager, ExtMan.IVsExtensionManager>();
        }

        /// <summary>
        /// Creates a new extension from extension manifest
        /// </summary>
        /// <param name="extensionPath">Full path to the extension manifest on disk</param>
        /// <returns></returns>
        public IExtension CreateExtension(string extensionPath)
        {
            return new VsExtension(this.vsManager.CreateExtension(extensionPath));
        }

        /// <summary>
        /// Returns extensions that are currently enabled
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IInstalledExtension> GetEnabledExtensions()
        {
            return this.vsManager.GetEnabledExtensions().Select(ex => new VsInstalledExtension(ex));
        }

        /// <summary>
        /// Returns the installed extension.
        /// </summary>
        /// <param name="identifier">The identifier of the extension</param>
        /// <returns></returns>
        public IInstalledExtension GetInstalledExtension(string identifier)
        {
            return new VsInstalledExtension(this.vsManager.GetInstalledExtension(identifier));
        }

        /// <summary>
        /// Returns all installed extensions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IInstalledExtension> GetInstalledExtensions()
        {
            return this.vsManager.GetInstalledExtensions().Select(ie => new VsInstalledExtension(ie));
        }
    }
}
