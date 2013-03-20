using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Default implementation of <see cref="ISettingsManager"/> which uses <see cref="ShellSettingsManager"/>.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ISettingsManager))]
    public class SettingsManager : ISettingsManager
    {
        private const string RuntimeSettings = "RuntimeSettings";
        private const string Tracing = "Tracing";
        private const string SourceNames = "SourceNames";
        private const string RootSourceLevel = "RootSourceLevel";

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SettingsManager>();
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        [ImportingConstructor]
        public SettingsManager([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Event raised when settings are saved.
        /// </summary>
        public event EventHandler<ChangedEventArgs<RuntimeSettings>> SettingsChanged = (sender, args) => { };

        /// <summary>
        /// Saves the specified settings to the underlying state.
        /// </summary>
        public void Save(RuntimeSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            var store = GetStore(this.serviceProvider);

            //// TODO: refactor this code to be generic and reflection/typedescritor-based.

            var tracingRoot = Path.Combine(StoreConstants.RegistrySettingsKeyName, RuntimeSettings, Tracing);
            var sourceNamesRoot = Path.Combine(StoreConstants.RegistrySettingsKeyName, RuntimeSettings, Tracing, SourceNames);

            var oldValue = ReadSettings(this.serviceProvider);

            // Recreate the settings for tracing.
            if (store.CollectionExists(tracingRoot))
            {
                store.DeleteCollection(tracingRoot);
            }

            store.CreateCollection(sourceNamesRoot);

            if (settings.Tracing != null)
            {
                store.SetString(tracingRoot, RootSourceLevel, settings.Tracing.RootSourceLevel.ToString());

                if (settings.Tracing.TraceSources != null)
                {
                    foreach (var sourceSetting in settings.Tracing.TraceSources)
                    {
                        if (!string.IsNullOrEmpty(sourceSetting.SourceName))
                        {
                            store.SetString(sourceNamesRoot, sourceSetting.SourceName, sourceSetting.LoggingLevel.ToString());
                        }
                    }
                }
            }

            this.SettingsChanged(this, new ChangedEventArgs<RuntimeSettings>(oldValue, settings));
        }

        /// <summary>
        /// Reads the settings from the underlying state.
        /// </summary>
        public RuntimeSettings Read()
        {
            return ReadSettings(this.serviceProvider);
        }

        private static RuntimeSettings ReadSettings(IServiceProvider serviceProvider)
        {
            var store = GetStore(serviceProvider);

            //// TODO: refactor this code to be generic and reflection/typedescritor-based.

            var settings = new RuntimeSettings();

            var tracingRoot = Path.Combine(StoreConstants.RegistrySettingsKeyName, RuntimeSettings, Tracing);
            var sourceNamesRoot = Path.Combine(StoreConstants.RegistrySettingsKeyName, RuntimeSettings, Tracing, SourceNames);

            if (!store.CollectionExists(tracingRoot))
            {
                return settings;
            }

            if (store.PropertyExists(tracingRoot, RootSourceLevel))
            {
                var rootSourceLevel = store.GetString(tracingRoot, RootSourceLevel);

                if (Enum.IsDefined(typeof(SourceLevels), rootSourceLevel))
                {
                    settings.Tracing.RootSourceLevel = (SourceLevels)Enum.Parse(typeof(SourceLevels), rootSourceLevel);
                }
            }

            foreach (var sourceName in store.GetPropertyNames(sourceNamesRoot))
            {
                var value = store.GetString(sourceNamesRoot, sourceName);

                if (Enum.IsDefined(typeof(SourceLevels), value))
                {
                    settings.Tracing.TraceSources.Add(new TraceSourceSetting(sourceName, (SourceLevels)Enum.Parse(typeof(SourceLevels), value)));
                }
                else
                {
                    tracer.TraceWarning(Resources.SettingsManager_InvalidTraceLevel, value, sourceName);
                }
            }

            return settings;
        }

        private static WritableSettingsStore GetStore(IServiceProvider serviceProvider)
        {
            var manager = new ShellSettingsManager(serviceProvider);
            var store = manager.GetWritableSettingsStore(SettingsScope.UserSettings);
            return store;
        }
    }
}
