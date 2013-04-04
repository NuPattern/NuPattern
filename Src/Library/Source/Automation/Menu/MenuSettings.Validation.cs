using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Authoring;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Custom validation rules.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class MenuSettings
    {
    }

    /// <summary>
    /// Exports the validations for <see cref="MenuSettings"/>.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class MenuSettingsValidations
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<MenuSettingsValidations>();

        // We statically cache the lookups here.
        // TODO: this could be refactored into a separate global service.
        private static ILookup<string, Type> ValueProviders;
        private static ILookup<string, Type> Conditions;
        private static ILookup<string, Lazy<Type>> Events;

        [Import]
        internal INuPatternProjectTypeProvider ProjectTypeProvider { get; set; }

        [Import]
        internal IBindingFactory BindingFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuSettingsValidations"/> class.
        /// </summary>
        [ImportingConstructor]
        public MenuSettingsValidations(IFeatureCompositionService composition)
        {
            Guard.NotNull(() => composition, composition);

            // We statically cache the lookups here.
            // TODO: this could be refactored into a separate global service.
            if (ValueProviders == null || Conditions == null || Events == null)
            {
                var valueProviders = composition.GetExports<IValueProvider, IFeatureComponentMetadata>();
                var conditions = composition.GetExports<ICondition, IFeatureComponentMetadata>();
                var events = composition.GetExports<IObservableEvent, IIdMetadata>();

                ValueProviders = valueProviders.ToLookup(item => item.Metadata.Id, item => item.Metadata.ExportingType);
                Conditions = conditions.ToLookup(item => item.Metadata.Id, item => item.Metadata.ExportingType);

                // Unlike feature components, the events are exposed globally, and are not 
                // decorated by the feature runtime, so there's no way to get to its ExportingType.
                // So the only way to retrieve its actual type is to get its instantiated value.
                // Which is anyways guaranteed to work as otherwise they wouldn't exist 
                // in the global catalog at all ;)
                Events = events.ToLookup(item => item.Metadata.Id, item => new Lazy<Type>(() => item.Value.GetType()));
            }
        }

        /// <summary>
        /// Validates that the CommandId is not empty, and exists.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCommandIdAndWizardIdIsNotEmpty(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                // Ensure not empty
                if (settings.CommandId == Guid.Empty && settings.WizardId == Guid.Empty)
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_MenuSettingsCommandIdAndWizardIdIsNotEmpty,
                            settings.Name),
                        Resources.Validate_MenuSettingsCommandIdAndWizardIdIsNotEmptyCode, settings.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateCommandIdAndWizardIdIsNotEmpty(null, null)).Name);

                throw;
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCommandIdIsValid(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                if (settings.CommandId != Guid.Empty)
                {
                    if (!settings.Owner.GetAutomationSettings<ICommandSettings>().Any(c => c.Id.Equals(settings.CommandId)))
                    {
                        if (settings.Owner.IsInheritedFromBase &&
                            SettingsValidationHelper.TryToFixId<ICommandSettings>(settings.Store, settings.Owner, settings.CommandId, id => settings.CommandId = id))
                        {
                            return;
                        }

                        context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_CommandIsNotValid),
                            Resources.Validate_CommandIsNotValidCode, settings.Extends);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateCommandIdIsValid(null, null)).Name);

                throw;
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateWizardIdIsValid(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                if (settings.WizardId != Guid.Empty)
                {
                    if (!settings.Owner.GetAutomationSettings<IWizardSettings>().Any(c => c.Id.Equals(settings.WizardId)))
                    {
                        if (settings.Owner.IsInheritedFromBase &&
                            SettingsValidationHelper.TryToFixId<IWizardSettings>(settings.Store, settings.Owner, settings.WizardId, id => settings.WizardId = id))
                        {
                            return;
                        }

                        context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_WizardIsNotValid),
                            Resources.Validate_WizardIsNotValidCode, settings.Extends);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateWizardIdIsValid(null, null)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Menu Text is not empty, and exists.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateTextIsNotEmpty(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                // Ensure not empty
                if (string.IsNullOrEmpty(settings.Text))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_MenuSettingsMenuTextIsNotEmpty, settings.Name),
                        Resources.Validate_MenuSettingsMenuTextIsNotEmptyCode,
                        settings.Extends);
                }

                // TODO: Ensure it still exists
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateTextIsNotEmpty(null, null)).Name);

                throw;
            }

        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateConditionsTypeIsNotEmpty(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                foreach (var condition in settings.ConditionSettings.Where(s => string.IsNullOrEmpty(s.TypeId)))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_ConditionSettingsTypeIsNotEmpty, settings.Name),
                        Resources.Validate_ConditionSettingsTypeIsNotEmptyCode,
                        settings);
                }

                // TODO: validate value providers for the condition bindings too:
                // var conditionBindings = this.Settings.ConditionSettings.Select(x => this.BindingFactory.Create<ICondition>(x)).ToArray();
                // then like in the CommandSettings.Validation.cs
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateConditionsTypeIsNotEmpty(null, null)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that the conditions TypeId is not empty, and exists.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCanResolveNonEmptyConditionsType(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                var conditions = settings.ConditionSettings;

                foreach (var condition in conditions.Where(s =>
                    !string.IsNullOrEmpty(s.TypeId) &&
                    s.FindBoundType<ICondition>(Conditions, context) == null))
                {
                    context.LogWarning(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Validate_CannotResolveType,
                        condition.TypeId,
                        settings.Name),
                        Resources.Validate_CannotResolveTypeCode,
                        settings.Extends);
                }

                // TODO: validate value providers for the condition bindings too:
                // var conditionBindings = this.Settings.ConditionSettings.Select(x => this.BindingFactory.Create<ICondition>(x)).ToArray();
                // then like in the CommandSettings.Validation.cs
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateCanResolveNonEmptyConditionsType(null, null)).Name);

                throw;
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateIconIsEmptyOrValid(ValidationContext context, MenuSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                if (!string.IsNullOrEmpty(settings.Icon))
                {
                    var uriService = ((IServiceProvider)settings.Store).GetService<IFxrUriReferenceService>();
                    var resolvedIcon = uriService.ResolveUri<ResourcePack>(new Uri(settings.Icon));

                    if (resolvedIcon == null)
                    {
                        context.LogError(
                                string.Format(CultureInfo.CurrentCulture, Resources.Validate_MenuSettingsIconDoesNotPointToAValidFile, settings.Name),
                                Resources.Validate_MenuSettingsIconDoesNotPointToAValidFileCode,
                                settings);
                        return;
                    }

                    if (resolvedIcon.Type == ResourcePackType.ProjectItem)
                    {
                        var item = resolvedIcon.GetItem();
                        if (item.Data.ItemType != "Resource")
                        {
                            context.LogError(
                                    string.Format(CultureInfo.CurrentCulture, Resources.Validate_MenuSettingsIconIsNotAResource, settings.Name, item.Name),
                                    Resources.Validate_MenuSettingsIconIsNotAResourceCode,
                                    settings);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<MenuSettingsValidations>.GetMethod(n => n.ValidateIconIsEmptyOrValid(null, null)).Name);

                throw;
            }
        }
    }
}