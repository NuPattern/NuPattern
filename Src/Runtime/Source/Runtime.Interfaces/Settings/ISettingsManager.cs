using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Manages runtime settings.
	/// </summary>
	public interface ISettingsManager
	{
		/// <summary>
		/// Event raised when settings are saved.
		/// </summary>
		event EventHandler<ChangedEventArgs<RuntimeSettings>> SettingsChanged;

		/// <summary>
		/// Reads the settings from the underlying state.
		/// </summary>
		RuntimeSettings Read();

		/// <summary>
		/// Saves the specified settings to the underlying state.
		/// </summary>
		void Save(RuntimeSettings settings);
	}
}
