using System;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
	/// <summary>
	/// Definitions for the PatternToolkitLibrary toolkit project
	/// </summary>
	internal class AutomationLibraryToolkitInfo
	{
		/// <summary>
		/// Gets the pattern definition identifier.
		/// </summary>
		public static Guid ProductId = new Guid("d6139b37-9971-4834-a9dc-2b3daef962cf");

		/// <summary>
		/// Gets the VSIX identifier of this toolkit.
		/// </summary>
		public static string ToolkitId = "080eb0ef-518d-4807-9b5c-aa32d0032e0b";

		/// <summary>
		/// Gets the VSIX name of this toolkit.
		/// </summary>
		public static string RegistrationName = "NuPattern Toolkit Library";
	}
}

