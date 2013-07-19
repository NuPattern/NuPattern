using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Shell;

namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// A guidance provider
    /// </summary>
    [Export(typeof(IGuidanceProvider))]
    internal class GuidanceProvider : IGuidanceProvider
    {
        /// <summary>
        /// Gets or sets the <see cref="IGuidanceManager"/>.
        /// </summary>
        [Import]
        internal IGuidanceManager GuidanceManager { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/>.
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Whether the extension is registered
        /// </summary>
        /// <param name="extensionId">The guidance extension identifier</param>
        public bool IsRegistered(string extensionId)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            return this.GuidanceManager.IsInstalled(extensionId);
        }

        /// <summary>
        /// Whether the instance of the extension exists
        /// </summary>
        /// <param name="extensionId">The guidance extension identifier</param>
        /// <param name="instanceName">The name of the instance</param>
        public bool InstanceExists(string extensionId, string instanceName)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            return this.GuidanceManager.IsInstantiated(extensionId, instanceName);
        }

        /// <summary>
        /// Creates a new instance of the extension
        /// </summary>
        /// <param name="extensionId">The guidance extension identifier</param>
        /// <param name="instanceName">The name of the instance</param>
        public void CreateInstance(string extensionId, string instanceName)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            this.GuidanceManager.InstantiateGuidanceInstance(this.ServiceProvider, extensionId, instanceName);
        }

        /// <summary>
        /// Activates the instance of the extension
        /// </summary>
        /// <param name="instanceName">The name of the instance</param>
        public void ActivateInstance(string instanceName)
        {
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            var workflow = this.GuidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(ge => ge.InstanceName.Equals(instanceName, StringComparison.OrdinalIgnoreCase));
            if (workflow != null)
            {
                this.GuidanceManager.ActivateGuidanceInstance(this.ServiceProvider, workflow);
            }
        }

        /// <summary>
        /// Returns a unique instance name, using the given <paramref name="instanceName"/> as a seed.
        /// </summary>
        /// <param name="instanceName">The name of the instance to use as a seed</param>
        public string GetUniqueInstanceName(string instanceName)
        {
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            return this.GuidanceManager.GetUniqueInstanceName(instanceName);
        }

        /// <summary>
        /// Gets the default instance name of an extension.
        /// </summary>
        /// <param name="extensionId">The guidance extension identifier</param>
        public string GetDefaultInstanceName(string extensionId)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            var registration = this.GuidanceManager.InstalledGuidanceExtensions.FirstOrDefault(ge => ge.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
            if (registration != null)
            {
                return registration.DefaultName;
            }

            return null;
        }
    }
}
