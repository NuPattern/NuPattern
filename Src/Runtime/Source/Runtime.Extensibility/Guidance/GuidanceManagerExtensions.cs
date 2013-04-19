using System;
using System.Linq;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Extensions to the <see cref="IGuidanceManager"/>.
    /// </summary>
    [CLSCompliant(false)]
    public static class GuidanceManagerExtensions
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<IGuidanceManager>();

        /// <summary>
        /// Gets whether the given guidance extension is installed in the system.
        /// </summary>
        public static bool IsInstalled(this IGuidanceManager manager, string extensionId)
        {
            return manager.InstalledGuidanceExtensions.Any(registration => registration.ExtensionId == extensionId);
        }

        /// <summary>
        /// Gets whether the given guidance extension has been instantiated in the solution.
        /// </summary>
        public static bool IsInstantiated(this IGuidanceManager manager, string extensionId)
        {
            return manager.InstantiatedGuidanceExtensions.Any(e => e.ExtensionId == extensionId);
        }

        /// <summary>
        /// Gets whether the given guidance extension has been instantiated in the solution.
        /// </summary>
        public static bool IsInstantiated(this IGuidanceManager manager, IGuidanceExtensionRegistration registration)
        {
            return manager.IsInstantiated(registration.ExtensionId);
        }

        /// <summary>
        /// Returns a unique name for the guidance extension instance.
        /// </summary>
        [CLSCompliant(false)]
        public static string GetUniqueInstanceName(this IGuidanceManager guidanceManager, string baseName)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => baseName, baseName);

            return UniqueNameGenerator.EnsureUnique(baseName,
                newName => !guidanceManager.InstantiatedGuidanceExtensions.Any(e => e.InstanceName.Equals(newName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Displays the guidance windows for guidance extension extensions.
        /// </summary>
        [CLSCompliant(false)]
        public static void ShowGuidanceWindows(this IGuidanceManager guidanceManager, IServiceProvider provider)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => provider, provider);

            var toolWindows = provider.GetService<IGuidanceWindowsService>();
            if (toolWindows != null)
            {
                tracer.TraceVerbose(Resources.GudianceManagerExtensions_TraceShowingGuidanceWindows);

                toolWindows.ShowGuidanceExplorer(provider);
                toolWindows.ShowGuidanceBrowser(provider);
            }
        }

        /// <summary>
        /// Activates (or creates) the first instance of a shared guidance workflow.
        /// </summary>
        [CLSCompliant(false)]
        public static void ActivateSharedGuidanceWorkflow(this IGuidanceManager guidanceManager, IServiceProvider provider, string extensionId)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            var registration = guidanceManager.InstalledGuidanceExtensions.First(e => e.ExtensionId == extensionId);
            if (registration == null)
            {
                tracer.TraceError(Resources.GuidanceManagerExtensions_ErrorNoRegistration, extensionId);
                return;
            }
            else
            {
                // Ensure at least one instance exists
                var instance = guidanceManager.InstantiatedGuidanceExtensions.FirstOrDefault(e => e.ExtensionId == extensionId);
                if (instance == null)
                {
                    // Create the first instance of the guidance
                    tracer.ShieldUI(() =>
                    {
                        instance = guidanceManager.Instantiate(extensionId, registration.DefaultName);
                    }, Resources.GuidanceManagerExtensions_ErrorGuidanceInstantiationFailed, registration.DefaultName, extensionId);
                }

                // Activate the instance
                if (instance != null)
                {
                    guidanceManager.ActivateGuidanceInstance(provider, instance);
                }
            }
        }

        /// <summary>
        /// Activates the given guidance workflow instance, and displays the guidance workflow in the guidance toolwindows.
        /// </summary>
        [CLSCompliant(false)]
        public static void ActivateGuidanceInstance(this IGuidanceManager guidanceManager, IServiceProvider provider, IGuidanceExtension instance)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => instance, instance);

            if (provider != null)
            {
                guidanceManager.ShowGuidanceWindows(provider);
            }

            tracer.TraceInformation(Resources.GuidanceManagerExtensions_TraceActivation, instance.InstanceName);

            // (workaround) Force a refresh of the active guidance extension in Guidance Explorer
            guidanceManager.ActiveGuidanceExtension = null;
            guidanceManager.ActiveGuidanceExtension = instance;
        }

        /// <summary>
        /// Determines whether the given guidance extension extension is installed and registered.
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static bool IsGuidanceRegistered(this IGuidanceManager guidanceManager, string extensionId)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            return guidanceManager.InstalledGuidanceExtensions.Any(e => e.ExtensionId == extensionId);
        }
    }
}
