using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines a common interface for settings.
	/// </summary>
	public interface IAutomationSettings
	{
		/// <summary>
		/// Gets the identifier for this setting.
		/// </summary>
		Guid Id { get; }

		/// <summary>
		/// Gets the name of the settings.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the owner of the setting.
		/// </summary>
		IPatternElementSchema Owner { get; }

		/// <summary>
		/// Creates the runtime automation element for this setting element.
		/// </summary>
		IAutomationExtension CreateAutomation(IProductElement owner);

		/// <summary>
		/// Gets the classification of these settings.
		/// </summary>
		AutomationSettingsClassification Classification { get; }
	}
}