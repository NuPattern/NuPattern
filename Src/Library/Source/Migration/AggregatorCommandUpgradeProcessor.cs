
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Commands;
using NuPattern.Library.Design;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Migration
{
    /// <summary>
    /// Migrates the delimited string values of the <see cref="AggregatorCommand"/>
    /// </summary>
    /// <remarks> this processor must run after the <see cref="CommandSettingsUpgradeProcessor"/></remarks>
    [SchemaUpgradeProcessorOptions(Order = 1200, TargetVersion = "1.2.0.0")]
    internal class AggregatorCommandUpgradeProcessor : IPatternModelSchemaUpgradeProcessor
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CommandSettingsUpgradeProcessor>();
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName CommandSettingsElementName = XName.Get("commandSettings", DefaultNamespace);
        private static readonly XName CommandSettingsPropertiesElementName = XName.Get("properties", DefaultNamespace);
        private const string CommandSettingsIdName = "Id";
        private const string CommandSettingsTypeIdName = "typeId";
        private const char CommandReferenceDelimitier = ';';
        private const string GuidRegEx = @"[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}";
        private static readonly string DelimitedListGuidRegEx = @"^(" + GuidRegEx + @"[\" + Convert.ToString(CommandReferenceDelimitier) + "]{0,1}){0,}$";

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

            // Locate all <commandSettings> that have a 'typeId="NuPattern.Library.Commands.AggregatorCommand"'
            var commandSettings = document.Descendants(CommandSettingsElementName)
                        .Where(cs => cs.Attribute(CommandSettingsTypeIdName) != null && cs.Attribute(CommandSettingsTypeIdName).Value == typeof(AggregatorCommand).FullName)
                        .Distinct();
            if (commandSettings.Any())
            {
                tracer.TraceInformation(Resources.AggregatorCommandUpgradeProcessor_TraceDeserialize);

                // Enumerate each <commandSettings> element
                commandSettings.ForEach(cmdSettingsElement =>
                    {
                        var id = cmdSettingsElement.Attribute(CommandSettingsIdName) != null ? cmdSettingsElement.Attribute(CommandSettingsIdName).Value : string.Empty;
                        var cmdPropsElement = cmdSettingsElement.Descendants(CommandSettingsPropertiesElementName).FirstOrDefault();
                        if (cmdPropsElement != null)
                        {
                            // Ensure we have a value for <properties>
                            var propertiesValue = cmdPropsElement.Value;
                            if (!string.IsNullOrEmpty(propertiesValue))
                            {
                                try
                                {
                                    // Determine if has a serialized 'string' value, as opposed to a 'Collection<CommmandReference>'
                                    var bindings = BindingSerializer.Deserialize<IEnumerable<IPropertyBindingSettings>>(propertiesValue);
                                    if (bindings != null && bindings.Any())
                                    {
                                        var existingBinding = bindings.FirstOrDefault(b => b.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));
                                        if (existingBinding != null)
                                        {
                                            // Ensure there is a value
                                            if (!String.IsNullOrEmpty(existingBinding.Value))
                                            {
                                                //Ensure value is previous GUID list format
                                                if (Regex.IsMatch(existingBinding.Value, DelimitedListGuidRegEx))
                                                {
                                                    // Read the delimitied array of strings
                                                    var referenceStrings = existingBinding.Value.Split(new[] { CommandReferenceDelimitier }, StringSplitOptions.RemoveEmptyEntries);
                                                    if (referenceStrings.Any())
                                                    {
                                                        tracer.TraceInformation(Resources.AggregatorCommandUpgradeProcessor_TraceDeserializeCommandSettings, id);

                                                        // Convert to command references
                                                        var references = new List<CommandReference>();
                                                        referenceStrings.ForEach(rs =>
                                                            {
                                                                Guid refId;
                                                                if (Guid.TryParse(rs, out refId))
                                                                {
                                                                    references.Add(new CommandReference(null)
                                                                        {
                                                                            CommandId = refId,
                                                                        });
                                                                }
                                                            });

                                                        // Update value of <properties> element
                                                        var newBinding = new PropertyBindingSettings
                                                        {
                                                            Name = Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList),
                                                            Value = BindingSerializer.Serialize(new Collection<CommandReference>(references)),
                                                        };
                                                        cmdPropsElement.SetValue(BindingSerializer.Serialize(new IPropertyBindingSettings[] { newBinding }));
                                                        this.IsModified = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (BindingSerializationException)
                                {
                                    // Ignore deserializaton exception
                                }
                            }
                        }
                    });
            }
        }
    }
}
