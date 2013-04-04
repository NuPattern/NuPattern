using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Monitors changes to <see cref="ISettingsManager"/> and reflects them 
    /// in the runtime traces.
    /// </summary>
    internal class TracingSettingsMonitor : IDisposable
    {
        private ISettingsManager settingsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TracingSettingsMonitor"/> class.
        /// </summary>
        /// <param name="settingsManager">The settings manager.</param>
        public TracingSettingsMonitor(ISettingsManager settingsManager)
        {
            Guard.NotNull(() => settingsManager, settingsManager);

            this.settingsManager = settingsManager;
            this.settingsManager.SettingsChanged += this.OnSettingsChanged;

            this.Initialize();
        }

        /// <summary>
        /// Stops tracking changes to the settings.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Derived classes can override this method to release managed and unmanaged resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.settingsManager.SettingsChanged -= this.OnSettingsChanged;
            }
        }

        private void Initialize()
        {
            var settings = this.settingsManager.Read();

            ApplySettings(settings);
        }

        private void OnSettingsChanged(object sender, ChangedEventArgs<IRuntimeSettings> e)
        {
            ClearSettings(e.OldValue);
            ApplySettings(e.NewValue);
        }

        private static bool HasSettings(IRuntimeSettings settings)
        {
            return settings != null && settings.Tracing != null;
        }

        private static void ApplySettings(IRuntimeSettings settings)
        {
            if (HasSettings(settings))
            {
                Tracer.GetOrCreateUnderlyingSource(TracingSettings.DefaultRootSourceName).Switch.Level = settings.Tracing.RootSourceLevel;

                if (settings.Tracing.TraceSources != null)
                {
                    foreach (var setting in settings.Tracing.TraceSources)
                    {
                        var source = Tracer.GetOrCreateUnderlyingSource(setting.SourceName);
                        source.Listeners.Add(new DefaultTraceListener());
                        source.Switch.Level = setting.LoggingLevel;
                    }
                }
            }
        }

        private static void ClearSettings(IRuntimeSettings settings)
        {
            if (HasSettings(settings) && settings.Tracing.TraceSources != null)
            {
                Tracer.GetOrCreateUnderlyingSource(TracingSettings.DefaultRootSourceName).Switch.Level = TracingSettings.DefaultRootSourceLevel;

                foreach (var setting in settings.Tracing.TraceSources)
                {
                    var source = Tracer.GetOrCreateUnderlyingSource(setting.SourceName);
                    source.Switch.Level = SourceLevels.Off;
                }
            }
        }
    }
}
