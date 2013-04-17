using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Required attribute that features must apply to register themselves 
    /// with the feature runtime.
    /// </summary>
    /// <remarks>
    /// This attribute must be applied at two locations: one is the 
    /// class implementing the <see cref="IFeatureExtension"/>, and the other 
    /// is as an assembly-level attribute on all projects that 
    /// collectively make up a feature (i.e. commands libraries, etc.).
    /// </remarks>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly)]
    public class FeatureAttribute : Attribute, IFeatureMetadata
    {
        private static readonly char[] FileNameInvalidChars = Path.GetInvalidFileNameChars();
        private string defaultName;

        /// <summary>
        /// Initializes the feature attribute with the given feature identifier, which should 
        /// match the containing VSIX identifier.
        /// </summary>
        public FeatureAttribute(string featureId)
        {
            this.FeatureId = featureId;
            this.DefaultName = RemoveInvalidFileChars(featureId);
        }

        /// <summary>
        /// The feature identifier.
        /// </summary>
        public string FeatureId { get; private set; }

        /// <summary>
        /// Default name for instances of this feature.
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