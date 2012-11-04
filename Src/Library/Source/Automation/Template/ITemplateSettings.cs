using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	partial interface ITemplateSettings
	{
		/// <summary>
		/// Gets the settings for the of the target file.
		/// </summary>
		IPropertyBindingSettings TargetFileName { get; }

		/// <summary>
		/// Gets the settings for the of the target path.
		/// </summary>
		IPropertyBindingSettings TargetPath { get; }
	}
}
