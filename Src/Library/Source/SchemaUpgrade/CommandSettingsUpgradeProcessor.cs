using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.SchemaUpgrade
{
    /// <summary>
    /// Migrates CommandSetting properties from XML to JSon.
    /// </summary>
    [SchemaUpgradeProcessorOptions(Order = 1100, TargetVersion = "1.2.0.0")]
    internal class CommandSettingsUpgradeProcessor : IPatternModelSchemaUpgradeProcessor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CommandSettingsUpgradeProcessor>();
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName CommandSettingsElementName = XName.Get("commandSettings", DefaultNamespace);
        private static readonly XName CommandSettingsPropertiesElementName = XName.Get("properties", DefaultNamespace);
        private static readonly XName PropertySettingsElementName = XName.Get("propertySettings", DefaultNamespace);
        private static readonly XName ValueProviderElementName = XName.Get("valueProvider", DefaultNamespace);
        private static readonly XName ValueProviderSettingElementName = XName.Get("valueProviderSettings", DefaultNamespace);
        private static readonly XName PropertySettingMonikerElementName = XName.Get("propertySettingsMoniker", DefaultNamespace);
        private const string CommandSettingsIdName = "Id";
        private const string ValueProviderSettingsIdName = "Id";
        private const string PropertySettingsIdName = "Id";
        private const string PropertySettingsMonikerIdName = "Id";
        private const string PropertySettingsNameName = "name";
        private const string PropertySettingsValueName = "value";
        private const string ValueProviderSettingsTypeIdName = "typeId";

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

            // Locate all <commandSettings>
            var commandSettings = document.Descendants(CommandSettingsElementName)
                        .Distinct();
            if (commandSettings.Any())
            {
                tracer.TraceInformation(Resources.CommandSettingsUpgradeProcessor_TraceDeserialize);

                // Enumerate each <commandSettings> element
                commandSettings.ForEach(cmdSettingsElement =>
                    {
                        var id = cmdSettingsElement.Attribute(CommandSettingsIdName) != null ? cmdSettingsElement.Attribute(CommandSettingsIdName).Value : string.Empty;
                        var cmdPropsElement = cmdSettingsElement.Descendants(CommandSettingsPropertiesElementName).FirstOrDefault();
                        if (cmdPropsElement != null)
                        {
                            // Determine if has any <propertySettings>
                            var propSettingsElements = cmdPropsElement.Descendants(PropertySettingsElementName);
                            if (propSettingsElements.Any())
                            {
                                tracer.TraceInformation(Resources.CommandSettingsUpgradeProcessor_TraceDeserializeCommandSettings, id);

                                var bindings = new List<IPropertyBindingSettings>();
                                var processedPropertySettings = new List<XElement>();

                                // Enumerate each <propertySettings> element
                                propSettingsElements.ForEach(propSettingElement =>
                                    {
                                        // Ensure we have not already processed this <propertySettings> element
                                        // (i.e. that it is not a nested <propertySettings> element)
                                        if (!processedPropertySettings.Contains(propSettingElement))
                                        {
                                            // Add to processed cache
                                            processedPropertySettings.Add(propSettingElement);

                                            // Create bindings 
                                            AddPropertySettings(cmdPropsElement, bindings, propSettingElement, processedPropertySettings);
                                        }
                                    });

                                // Update value of <properties> element
                                cmdPropsElement.SetValue(BindingSerializer.Serialize<IEnumerable<IPropertyBindingSettings>>(bindings));
                                this.IsModified = true;
                            }
                        }
                    });
            }
        }

        private static void AddPropertySettings(XElement cmdPropsElement, IList<IPropertyBindingSettings> bindings, XElement propSettingElement, IList<XElement> processedPropertySettings)
        {
            // Create a PropertyBindingSettings from the <PropertySettings> element.
            if (propSettingElement.Attributes().Any(a => a.Name == PropertySettingsNameName))
            {
                var binding = new PropertyBindingSettings
                {
                    Name = TrimName(propSettingElement.Attribute(PropertySettingsNameName).Value),
                    Value = propSettingElement.Attributes().Any(a => a.Name == PropertySettingsValueName)
                            ? DecodeValue(propSettingElement.Attribute(PropertySettingsValueName).Value)
                            : null,
                };

                // Determine if has a ValueProvider
                var valueProviderElement = propSettingElement.Descendants(ValueProviderElementName)
                                      .FirstOrDefault();
                if (valueProviderElement != null)
                {
                    var valueProviderSettingsElement = valueProviderElement.Descendants(ValueProviderSettingElementName)
                                            .FirstOrDefault();
                    if (valueProviderSettingsElement != null)
                    {
                        if (valueProviderSettingsElement.Attributes().Any(a => a.Name == ValueProviderSettingsIdName))
                        {
                            var vpBinding = new ValueProviderBindingSettings
                            {
                                TypeId = valueProviderSettingsElement.Attribute(ValueProviderSettingsTypeIdName).Value,
                            };

                            // Determine if the ValueProvider has nested <propertySettings> elements
                            var propSettingsMonikers = valueProviderSettingsElement.Descendants(PropertySettingMonikerElementName);
                            if (propSettingsMonikers.Any())
                            {
                                // Match each nested <propertySettings> element, which should be linked to <commandSettings><properties>
                                propSettingsMonikers.ForEach(psm =>
                                {
                                    var psmId = psm.Attribute(PropertySettingsMonikerIdName).Value;
                                    var vpPropSettings = cmdPropsElement.Descendants(PropertySettingsElementName)
                                            .FirstOrDefault(pse => pse.Attribute(PropertySettingsIdName).Value == psmId);
                                    if (vpPropSettings != null)
                                    {
                                        // Cache <propertySetting> element
                                        processedPropertySettings.Add(vpPropSettings);

                                        // Create Bindings
                                        AddPropertySettings(cmdPropsElement, vpBinding.Properties, vpPropSettings, processedPropertySettings);
                                    }
                                });
                            }

                            binding.ValueProvider = vpBinding;
                        }
                    }
                }

                bindings.Add(binding);
            }
        }

        private static string DecodeValue(string p)
        {
            return XmlConvert.DecodeName(p);
        }

        private static string TrimName(string p)
        {
            return p.Split('.').Last();
        }
    }
}
