using System;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility.Properties;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Extensions for the FeatureManager.
    /// </summary>
    public static class FeatureManagerExtensions
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<IFeatureManager>();

        /// <summary>
        /// Returns a unique name for the feature instance.
        /// </summary>
        [CLSCompliant(false)]
        public static string GetUniqueInstanceName(this IFeatureManager featureManager, string baseName)
        {
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNull(() => baseName, baseName);

            return UniqueNameGenerator.EnsureUnique(baseName,
                newName => !featureManager.InstantiatedFeatures.Any(feature => feature.InstanceName.Equals(newName, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Displays the guidance windows for feature extensions.
        /// </summary>
        [CLSCompliant(false)]
        public static void ShowGuidanceWindows(this IFeatureManager featureManager, IServiceProvider provider)
        {
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNull(() => provider, provider);

            var toolWindows = provider.GetService<IGuidanceWindowsService>();
            if (toolWindows != null)
            {
                tracer.TraceVerbose(Resources.FeatureManagerExtensions_TraceShowingGuidanceWindows);

                toolWindows.ShowGuidanceExplorer();
                toolWindows.ShowGuidanceBrowser();
            }
        }

        /// <summary>
        /// Activates (or creates) the first instance of a shared guidance workflow.
        /// </summary>
        [CLSCompliant(false)]
        public static void ActivateSharedGuidanceWorkflow(this IFeatureManager featureManager, IServiceProvider provider, string featureId)
        {
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNullOrEmpty(() => featureId, featureId);

            var registration = featureManager.InstalledFeatures.First(feature => feature.FeatureId == featureId);
            if (registration == null)
            {
                tracer.TraceError(Resources.FeatureManagerExtensions_ErrorNoRegistration, featureId);
                return;
            }
            else
            {
                // Ensure at least one instance exists
                var instance = featureManager.InstantiatedFeatures.FirstOrDefault(feature => feature.FeatureId == featureId);
                if (instance == null)
                {
                    // Create the first instance of the guidance
                    tracer.ShieldUI(() =>
                    {
                        instance = featureManager.Instantiate(featureId, registration.DefaultName);
                    }, Resources.FeatureManagerExtensions_ErrorGuidanceInstantiationFailed, registration.DefaultName, featureId);
                }

                // Activate the instance
                if (instance != null)
                {
                    featureManager.ActivateGuidanceInstance(provider, instance);
                }
            }
        }

        /// <summary>
        /// Activates the given guidance workflow instance, and displays the guidance workflow in the guidance toolwindows.
        /// </summary>
        [CLSCompliant(false)]
        public static void ActivateGuidanceInstance(this IFeatureManager featureManager, IServiceProvider provider, IFeatureExtension instance)
        {
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNull(() => instance, instance);

            if (provider != null)
            {
                featureManager.ShowGuidanceWindows(provider);
            }

            tracer.TraceInformation(Resources.FeatureManagerExtensions_TraceActivation, instance.InstanceName);

            // (workaround) Force a refresh of the active feature in Guidance Explorer
            featureManager.ActiveFeature = null;
            featureManager.ActiveFeature = instance;
        }

        /// <summary>
        /// Determines whether the given feature extension is installed and registered.
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static bool IsGuidanceRegistered(this IFeatureManager featureManager, string featureId)
        {
            Guard.NotNull(() => featureManager, featureManager);
            Guard.NotNullOrEmpty(() => featureId, featureId);

            return featureManager.InstalledFeatures.Any(feature => feature.FeatureId == featureId);
        }
    }
}
