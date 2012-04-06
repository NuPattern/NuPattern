using System;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Metadata for exported automation extensions
	/// </summary>
	[CLSCompliant(false)]
	public interface IExportedAutomationMetadata
	{
		/// <summary>
		/// Gets the name of the icon resource.
		/// </summary>
		/// <value>The name of the icon resource.</value>
		string IconPath { get; }

		/// <summary>
		/// Gets the display name.
		/// </summary>
		/// <value>The display name.</value>
		string DisplayName { get; }

		/// <summary>
		/// Gets the type of the exporting.
		/// </summary>
		/// <value>The type of the exporting.</value>
		Type ExportingType { get; }
	}
}