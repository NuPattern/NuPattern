
namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Provides some default categories that components might use.
    /// </summary>
    internal static class GuidanceComponentCategories
    {
        /// <summary>
        /// Components with this category deal with the Windows registry.
        /// </summary>
        public const string Registry = "Registry";
        /// <summary>
        /// Components with this category deal with the file system.
        /// </summary>
        public const string FileSystem = "FileSystem";
        /// <summary>
        /// Components with this category deal with Visual Studio, the Solution Explorer, etc.
        /// </summary>
        public const string Ide = "IDE";
        /// <summary>
        /// Components with this category deal with UML and DSL diagrams and modeling projects.
        /// </summary>
        public const string Modeling = "Modeling";
        /// <summary>
        /// Components with this category deal with XML files and documents.
        /// </summary>
        public const string Xml = "XML";
        /// <summary>
        /// Components with this category deal with User Interface components.
        /// </summary>
        public const string UI = "UI";
        /// <summary>
        /// Default category.
        /// </summary>
        public const string General = "General";
    }
}
