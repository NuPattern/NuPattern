using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Custom validation rules.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class CommandSettings
    {
    }

    /// <summary>
    /// Exports the validation methods for command settings.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class CommandSettingsValidations
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CommandSettingsValidations>();

        // We statically cache the lookups here.
        // TODO: this could be refactored into a separate global service.
        private static ILookup<string, Type> ValueProviders;
        private static ILookup<string, Type> Commands;
        private static ILookup<string, Lazy<ICommandValidationRule, ICommandValidationRuleMetadata>> Validators;

        [Import]
        internal INuPatternProjectTypeProvider ProjectTypeProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSettingsValidations"/> class.
        /// </summary>
        [ImportingConstructor]
        public CommandSettingsValidations(IFeatureCompositionService composition)
        {
            Guard.NotNull(() => composition, composition);

            // We statically cache the lookups here.
            // TODO: this could be refactored into a separate global service.
            if (ValueProviders == null || Commands == null || Validators == null)
            {
                var valueProviders = composition.GetExports<IValueProvider, IFeatureComponentMetadata>();
                var commands = composition.GetExports<IFeatureCommand, IFeatureComponentMetadata>();
                var validators = composition.GetExports<ICommandValidationRule, ICommandValidationRuleMetadata>();

                ValueProviders = valueProviders.ToLookup(item => item.Metadata.Id, item => item.Metadata.ExportingType);
                Commands = commands.ToLookup(item => item.Metadata.Id, item => item.Metadata.ExportingType);
                Validators = validators.ToLookup(item => item.Metadata.CommandType.ToString(), item => item);
            }
        }

        /// <summary>
        /// Validates that the Type is not empty, and exists.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateTypeIsNotEmpty(ValidationContext context, CommandSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                // Ensure not empty
                if (string.IsNullOrEmpty(settings.TypeId))
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_CommandSettingsTypeIsNotEmpty,
                            settings.Name),
                        Resources.Validate_CommandSettingsTypeIsNotEmptyCode, settings.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<CommandSettingsValidations>.GetMethod(n => n.ValidateTypeIsNotEmpty(null, null)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates commands that export the CommandValidationRuleAttribute.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCommands(ValidationContext context, CommandSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            foreach (var validator in Validators[settings.TypeId])
            {
                validator.Value.Validate(context, settings.Extends as IAutomationSettingsSchema, settings);
            }
        }
    }
}