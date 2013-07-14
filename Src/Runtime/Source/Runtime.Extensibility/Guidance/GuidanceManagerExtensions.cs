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
        private static readonly ITracer tracer = Tracer.Get<IGuidanceManager>();

        /// <summary>
        /// Gets whether the given guidance extension is installed in the system.
        /// </summary>
        public static bool IsInstalled(this IGuidanceManager manager, string extensionId)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            return manager.FindRegistration(extensionId) != null;
        }

        /// <summary>
        /// Gets whether the given guidance extension has been instantiated in the solution.
        /// </summary>
        public static bool IsInstantiated(this IGuidanceManager manager, string extensionId)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            return manager.InstantiatedGuidanceExtensions.Any(ge => ge.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets whether the given guidance extension has been instantiated in the solution.
        /// </summary>
        public static bool IsInstantiated(this IGuidanceManager manager, string extensionId, string instanceName)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            return FindInstance(manager, extensionId, instanceName) != null;
        }

        /// <summary>
        /// Gets the registration with given extension 
        /// </summary>
        public static IGuidanceExtensionRegistration FindRegistration(this IGuidanceManager manager, string extensionId)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            return manager.InstalledGuidanceExtensions.FirstOrDefault(reg => reg.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the instance with given name and extension 
        /// </summary>
        public static IGuidanceExtension FindInstance(this IGuidanceManager manager, string extensionId, string instanceName)
        {
            Guard.NotNull(() => manager, manager);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            return manager.InstantiatedGuidanceExtensions.FirstOrDefault(ge => ge.ExtensionId.Equals(extensionId, StringComparison.OrdinalIgnoreCase)
                && ge.InstanceName.Equals(instanceName, StringComparison.OrdinalIgnoreCase));
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
                newName => !guidanceManager.InstantiatedGuidanceExtensions.Any(ge => ge.InstanceName.Equals(newName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Displays the guidance windows for guidance extension extensions.
        /// </summary>
        [CLSCompliant(false)]
        public static void ShowGuidanceWindows(this IGuidanceManager guidanceManager, IServiceProvider provider)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => provider, provider);

            var toolWindows = provider.GetService<IPatternWindows>();
            if (toolWindows != null)
            {
                tracer.Verbose(Resources.GuidanceManagerExtensions_TraceShowingGuidanceWindows);

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
            Guard.NotNull(() => provider, provider);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            var registration = guidanceManager.InstalledGuidanceExtensions.First(e => e.ExtensionId == extensionId);
            if (registration == null)
            {
                tracer.Error(Resources.GuidanceManagerExtensions_ErrorNoRegistration, extensionId);
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
            Guard.NotNull(() => provider, provider);
            Guard.NotNull(() => instance, instance);

            if (provider != null)
            {
                guidanceManager.ShowGuidanceWindows(provider);
            }

            tracer.Info(Resources.GuidanceManagerExtensions_TraceActivation, instance.InstanceName);

            // Activate guidance extension in Guidance Explorer
            guidanceManager.ActiveGuidanceExtension = instance;
        }

        /// <summary>
        /// Instantiates a new guidance workflow instance, and displays the guidance workflow in the guidance toolwindows.
        /// </summary>
        [CLSCompliant(false)]
        public static IGuidanceExtension InstantiateGuidanceInstance(this IGuidanceManager guidanceManager, IServiceProvider provider, string extensionId, string instanceName)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            Guard.NotNull(() => provider, provider);
            Guard.NotNullOrEmpty(() => extensionId, extensionId);

            if (provider != null)
            {
                guidanceManager.ShowGuidanceWindows(provider);
            }

            tracer.Info(Resources.GuidanceManagerExtensions_TraceInstantiation, extensionId, instanceName);

            // Create a new instance of guidance workflow
            var instance = guidanceManager.Instantiate(extensionId, instanceName);

            // Activate guidance extension in Guidance Explorer
            guidanceManager.ActiveGuidanceExtension = instance;

            return instance;
        }
    }
}
