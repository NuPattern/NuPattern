using System.Linq;
using System.Xml.Linq;
using NuPattern.Diagnostics;
using NuPattern.Library.Automation;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;

namespace NuPattern.Library.SchemaUpgrade
{
    /// <summary>
    /// Migrates the property name of the <see cref="GuidanceExtension"/> automation.
    /// </summary>
    /// <remarks> this processor must run after the <see cref="CommandSettingsUpgradeProcessor"/></remarks>
    [SchemaUpgradeProcessorOptions(Order = 1200, TargetVersion = "1.2.0.0")]
    internal class GuidanceExtensionUpgradeProcessor : IPatternModelSchemaUpgradeProcessor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GuidanceExtensionUpgradeProcessor>();
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName GuidanceExtensionElementName = XName.Get("guidanceExtension", DefaultNamespace);
        private const string GuidanceFeatureIdPropertyName = "guidanceFeatureId";
        private static readonly string ExtensionIdPropertyName = Reflector<GuidanceExtension>.GetPropertyName(x => x.ExtensionId).MakeCamel();

        /// <summary>
        /// Gets whether the document has been changed.
        /// </summary>
        public bool IsModified { get; private set; }

        /// <summary>
        /// Processes the document
        /// </summary>
        /// <param name="document">The document to process.</param>
        public void ProcessSchema(XDocument document)
        {
            Guard.NotNull(() => document, document);

            this.IsModified = false;

            // Locate all <guidanceExtension> that have a 'guidanceFeatureId=' property
            var extensionSettings = document.Descendants(GuidanceExtensionElementName)
                        .Where(cs => cs.Attribute(GuidanceFeatureIdPropertyName) != null)
                        .Distinct();
            if (extensionSettings.Any())
            {
                tracer.TraceInformation(Resources.GuidanceExtensionUpgradeProcessor_TraceDeserialize);

                // Enumerate each <guidanceExtension> element
                extensionSettings.ForEach(extensionElement =>
                    {
                        // Replace guidanceFeatureId attribute
                        var featureId = string.Empty;
                        var guidanceFeatureIdAttribute = extensionElement.Attribute(GuidanceFeatureIdPropertyName);
                        if (guidanceFeatureIdAttribute != null)
                        {
                            featureId = guidanceFeatureIdAttribute.Value;
                            guidanceFeatureIdAttribute.Remove();
                        }

                        extensionElement.Add(new XAttribute(ExtensionIdPropertyName, featureId));
                        this.IsModified = true;
                    });
            }
        }
    }
}
