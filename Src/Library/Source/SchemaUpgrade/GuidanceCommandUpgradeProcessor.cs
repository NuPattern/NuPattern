using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NuPattern.Diagnostics;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.SchemaUpgrade
{
    /// <summary>
    /// Migrates the names and property names of the <see cref="ActivateGuidanceWorkflowCommand"/>, 
    /// <see cref="InstantiateGuidanceWorkflowCommand"/> and <see cref="ActivateOrInstantiateSharedGuidanceWorkflowCommand"/> commands.
    /// </summary>
    /// <remarks> this processor must run after the <see cref="CommandSettingsUpgradeProcessor"/></remarks>
    [SchemaUpgradeProcessorOptions(Order = 1200, TargetVersion = @"1.2.0.0")]
    internal class GuidanceCommandUpgradeProcessor : IPatternModelSchemaUpgradeProcessor
    {
        private static readonly ITracer tracer = Tracer.Get<GuidanceCommandUpgradeProcessor>();
        private const string DefaultNamespace = SchemaConstants.DefaultNamespace;
        private static readonly XName CommandSettingsElementName = XName.Get(@"commandSettings", DefaultNamespace);
        private static readonly XName CommandSettingsPropertiesElementName = XName.Get(@"properties", DefaultNamespace);
        private const string CommandSettingsTypeIdName = @"typeId";
        private const string ActivateCommandTypeName = @"NuPattern.Library.Commands.ActivateFeatureCommand";
        private const string InstantiateCommandTypeName = @"NuPattern.Library.Commands.InstantiateFeatureCommand";
        private const string ActivateOrInstantiateCommandTypeName = @"NuPattern.Library.Commands.ActivateOrInstantiateSharedFeatureCommand";
        private const string FeatureIdPropertyName = @"FeatureId";

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

            // Locate all <commandSettings> that have a 'typeId=' any of the old feature commands
            var commandSettings = document.Descendants(CommandSettingsElementName)
                        .Where(cs => cs.Attribute(CommandSettingsTypeIdName) != null)
                        .Where(cs => cs.Attribute(CommandSettingsTypeIdName).Value == ActivateCommandTypeName
                        || cs.Attribute(CommandSettingsTypeIdName).Value == InstantiateCommandTypeName
                        || cs.Attribute(CommandSettingsTypeIdName).Value == ActivateOrInstantiateCommandTypeName)
                        .Distinct();
            if (commandSettings.Any())
            {
                tracer.Info(Resources.GuidanceCommandUpgradeProcessor_TraceDeserialize);

                // Enumerate each <commandSettings> element
                commandSettings.ForEach(cmdSettingsElement =>
                    {
                        // Rename command TypeId
                        var typeId = cmdSettingsElement.Attribute(CommandSettingsTypeIdName) != null ? cmdSettingsElement.Attribute(CommandSettingsTypeIdName).Value : string.Empty;
                        var newTypeId = string.Empty;
                        switch (typeId)
                        {
                            case ActivateCommandTypeName:
                                newTypeId = typeof(ActivateGuidanceWorkflowCommand).FullName;
                                break;

                            case InstantiateCommandTypeName:
                                newTypeId = typeof(InstantiateGuidanceWorkflowCommand).FullName;
                                break;

                            case ActivateOrInstantiateCommandTypeName:
                                newTypeId = typeof(ActivateOrInstantiateSharedGuidanceWorkflowCommand).FullName;
                                break;
                        }

                        if (!String.IsNullOrEmpty(newTypeId))
                        {
                            // Rename command typeId
                            cmdSettingsElement.Attribute(CommandSettingsTypeIdName).SetValue(newTypeId);
                            this.IsModified = true;

                            //Rename 'FeatureId' property to 'ExtensionId' property
                            // Note, not all these commands have properties
                            var cmdPropsElement = cmdSettingsElement.Descendants(CommandSettingsPropertiesElementName).FirstOrDefault();
                            if (cmdPropsElement != null)
                            {
                                var propertiesValue = cmdPropsElement.Value;
                                if (!string.IsNullOrEmpty(propertiesValue))
                                {
                                    try
                                    {
                                        var bindings = BindingSerializer.Deserialize<IEnumerable<IPropertyBindingSettings>>(propertiesValue).ToList();
                                        if (bindings != null && bindings.Any())
                                        {
                                            // Note, not all the commands had the 'FeatureId' property
                                            var featureIdProperty = bindings.FirstOrDefault(p => p.Name == FeatureIdPropertyName);
                                            if (featureIdProperty != null)
                                            {
                                                // Replace 'FeatureId' Property binding
                                                featureIdProperty.Name = Reflector<InstantiateGuidanceWorkflowCommand>.GetPropertyName(x => x.ExtensionId);

                                                // Write bindings back
                                                cmdPropsElement.SetValue(BindingSerializer.Serialize(bindings));
                                                this.IsModified = true;
                                            }
                                        }
                                    }
                                    catch (BindingSerializationException)
                                    {
                                        // Ignore deserializaton exception
                                    }
                                }
                            }
                        }
                    });
            }
        }
    }
}
