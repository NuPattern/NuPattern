using System;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Defines constant values for the runtime.
    /// </summary>
    public static class StoreConstants
    {
        /// <summary>
        /// Runtime store file extension.
        /// </summary>
        public const string RuntimeStoreExtension = @".slnbldr";
        
        /// <summary>
        /// Runtime store editor description.
        /// </summary>
        public const string RuntimeStoreEditorDescription = @"Solution Builder";

        /// <summary>
        /// Current toolkit version.
        /// </summary>
        public static readonly Version DslVersion = new Version(@"1.2.0.0");

        /// <summary>
        /// The name of the registry key for storing settings for the runtime.
        /// </summary>
        public const string RegistrySettingsKeyName = @"NuPatternToolkitManager";
        
        /// <summary>
        /// The product name.
        /// </summary>
        public const string ProductName = @"NuPattern Toolkit Manager";
    }
}