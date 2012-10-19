using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Defines constant values for the runtime.
	/// </summary>
	public static class Constants
	{
		/// <summary>
		/// Runtime store file extension.
		/// </summary>
		public const string RuntimeStoreExtension = ".slnbldr";
		
		/// <summary>
		/// Runtime store editor description.
		/// </summary>
		public const string RuntimeStoreEditorDescription = "Solution Builder";

		/// <summary>
		/// Current toolkit version.
		/// </summary>
		public static readonly Version DslVersion = new Version("1.2.0.0");

		/// <summary>
		/// The name of the registry key for storing settings for the runtime.
		/// </summary>
		public const string RegistrySettingsKeyName = "PatternToolkitExtensions";
		
		/// <summary>
		/// The product name.
		/// </summary>
		public const string ProductName = "Pattern Toolkit Manager";
	}
}