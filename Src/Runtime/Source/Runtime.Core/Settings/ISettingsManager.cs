using System;

namespace NuPattern.Runtime.Settings
{
    /// <summary>
    /// Manages runtime settings.
    /// </summary>
    internal interface ISettingsManager
    {
        /// <summary>
        /// Event raised when settings are saved.
        /// </summary>
        event EventHandler<ChangedEventArgs<IRuntimeSettings>> SettingsChanged;

        /// <summary>
        /// Reads the settings from the underlying state.
        /// </summary>
        IRuntimeSettings Read();

        /// <summary>
        /// Saves the specified settings to the underlying state.
        /// </summary>
        void Save(IRuntimeSettings settings);
    }
}
