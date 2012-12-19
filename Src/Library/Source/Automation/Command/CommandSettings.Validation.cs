using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Custom validation rules.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    public partial class CommandSettings
    {
    }

    /// <summary>
    /// Exports the validation methods for command settings.
    /// </summary>
    [CLSCompliant(false)]
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CommandSettingsValidations
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CommandSettingsValidations>();

        // We statically cache the lookups here.
        // TODO: this could be refactored into a separate global service.
        private static ILookup<string, Type> ValueProviders;
        private static ILookup<string, Type> Commands;
        private static ILookup<string, Lazy<ICommandValidationRule, ICommandValidationRuleMetadata>> Validators;

        [Import]
        internal IPlatuProjectTypeProvider ProjectTypeProvider
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

        /// <summary>
        /// Deletes unused bindings on all command automation.
        /// </summary>
        ///<remarks>
        /// This is the same behavior we have in FeatureBuilder/Runtime, which is necessary to do every time the model is validated.
        /// The reason is that bindings need to always be current, and invalid bindings must be deleted when the corresponding property in the underlying command implementation is removed (or renamed). Otherwise, the runtime binding will always fail as the property will not be found.
        /// This needs to be done on validation of the DSL as I don't think there's another deterministic point in time to validate the bindings (they can be invalidated when the command implementation is changed in the current solution, or in a third party library).
        ///</remarks>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Menu | ValidationCategories.Save)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "By Design")]
        internal void ValidatePropertyBindings(ValidationContext context, CommandSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                var toRemove = new List<PropertySettings>();

                if (settings.FindBoundType<IFeatureCommand>(Commands, context) != null)
                {
                    // We could resolve the command type, so the typedescriptor for this settings will have the command properties and we can check.
                    var commandProps = TypeDescriptor.GetProperties(settings).Cast<PropertyDescriptor>().Select(prop => prop.Name);
                    toRemove.AddRange(settings.Properties
                        .Where(prop => prop.ParentProvider == null && !commandProps.Contains(prop.Name)));
                }
                else
                {
                    context.LogWarning(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Validate_CannotResolveType,
                        settings.TypeId,
                        settings.Name),
                        Resources.Validate_CannotResolveTypeCode,
                        settings.Extends);
                }

                // The providers are all flattened in this collection of property bindings, 
                // and only those property bindings with a "ParentProvider" are the 
                // ones that configure value providers, not commands (which we have 
                // already evaluated above)
                var valueProviders = settings.Properties
                    .Where(prop => prop.ParentProvider != null)
                    .GroupBy(prop => prop.ParentProvider);

                // This should be generalized as it is the same for FeatureCommand, Condition, ValidationRule
                foreach (var valueProvider in valueProviders)
                {
                    var providerType = valueProvider.Key.FindBoundType<IValueProvider>(ValueProviders, context);
                    // We skip those valueproviders where we cannot resolve the type, as we 
                    // have no way of knowing what properties it has.
                    if (providerType == null)
                    {
                        context.LogWarning(string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_CannotResolveType,
                            valueProvider.Key.TypeId,
                            settings.Name),
                            Resources.Validate_CannotResolveTypeCode,
                            settings.Extends);

                        continue;
                    }

                    var providerProps = TypeDescriptor.GetProperties(providerType).Cast<PropertyDescriptor>().Select(prop => prop.Name);
                    toRemove.AddRange(valueProvider.Where(prop => !providerProps.Contains(prop.Name.Split('.').Last())));
                }

                if (toRemove.Count > 0)
                {
                    using (var transaction = settings.Store.TransactionManager.BeginTransaction("Removing invalid property bindings"))
                    {
                        foreach (var property in toRemove)
                        {
                            context.LogMessage(string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_InvalidPropertyBinding,
                                property.Name,
                                property.ParentProvider != null ? property.ParentProvider.TypeId : property.CommandSettings.TypeId),
                                Resources.Validate_InvalidPropertyBindingCode,
                                settings.Extends);

                            settings.Properties.Remove(property);
                        }

                        transaction.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<CommandSettingsValidations>.GetMethod(n => n.ValidatePropertyBindings(null, null)).Name);

                throw;
            }
        }
    }
}