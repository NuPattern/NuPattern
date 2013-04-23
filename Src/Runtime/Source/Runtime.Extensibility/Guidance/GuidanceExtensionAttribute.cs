using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Required attribute that guidance extensions must apply to register themselves 
    /// with the guidance extension runtime.
    /// </summary>
    /// <remarks>
    /// This attribute must be applied at two locations: one is the 
    /// class implementing the <see cref="IGuidanceExtension"/>, and the other 
    /// is as an assembly-level attribute on all projects that 
    /// collectively make up a guidance extension (i.e. commands libraries, etc.).
    /// </remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public class GuidanceExtensionAttribute : Attribute, IGuidanceExtensionMetadata
    {
        private static readonly char[] FileNameInvalidChars = Path.GetInvalidFileNameChars();
        private string defaultName;

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceExtensionAttribute"/> class.
        /// </summary>
        public GuidanceExtensionAttribute(string extensionId)
        {
            this.ExtensionId = extensionId;
            this.DefaultName = RemoveInvalidFileChars(extensionId);
        }

        /// <summary>
        /// The guidance extension identifier.
        /// </summary>
        public string ExtensionId { get; private set; }

        /// <summary>
        /// Default name for instances of this guidance extension.
        /// </summary>
        public string DefaultName
        {
            get { return defaultName; }
            set { defaultName = RemoveInvalidFileChars(value); }
        }

        private static string RemoveInvalidFileChars(string name)
        {
            return new string(name.Where(c => !FileNameInvalidChars.Contains(c)).ToArray());
        }
    }
}