using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
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
    partial class EventSettings
    {
    }

    /// <summary>
    /// Exports the validations for <see cref="EventSettings"/>.
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class EventSettingsValidations
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<EventSettingsValidations>();

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
        /// Initializes a new instance of the <see cref="EventSettingsValidations"/> class.
        /// </summary>
        [ImportingConstructor]
        public EventSettingsValidations(IFeatureCompositionService composition)
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
        internal void ValidateCommandIdAndWizardIdIsNotEmpty(ValidationContext context, EventSettings settings)
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
                            Resources.Validate_EventSettingsCommandIdAndWizardIdIsNotEmpty,
                            settings.Name),
                        Resources.Validate_EventSettingsCommandIdAndWizardIdIsNotEmptyCode, settings.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateCommandIdAndWizardIdIsNotEmpty(null, null)).Name);

                throw;
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCommandIdIsValid(ValidationContext context, EventSettings settings)
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
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateCommandIdIsValid(null, null)).Name);

                throw;
            }
        }

        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateWizardIdIsValid(ValidationContext context, EventSettings settings)
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
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateWizardIdIsValid(null, null)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that the EventId is not empty.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateEventIdIsNotEmpty(ValidationContext context, EventSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                // Ensure not empty
                if (String.IsNullOrEmpty(settings.EventId))
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_EventIdSettingsEventIsNotEmpty,
                            settings.Name),
                        Resources.Validate_EventIdSettingsEventIsNotEmptyCode, settings.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateEventIdIsNotEmpty(null, null)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that if the EventId is not empty,it can be resolved.
        /// </summary>
        [Export(typeof(System.Action<ValidationContext, object>))]
        [AuthoringValidationExtension]
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        internal void ValidateCanResolveNonEmptyEventId(ValidationContext context, EventSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                // Ensure not empty
                if (!string.IsNullOrEmpty(settings.EventId) && GetEventType(settings.EventId, context) == null)
                {
                    context.LogWarning(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Validate_CannotResolveType,
                        settings.EventId,
                        settings.Name),
                        Resources.Validate_CannotResolveTypeCode,
                        settings.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateCanResolveNonEmptyEventId(null, null)).Name);

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
        internal void ValidateConditionsTypeIsNotEmpty(ValidationContext context, EventSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                var conditions = settings.ConditionSettings;
                foreach (var condition in conditions.Where(s => string.IsNullOrEmpty(s.TypeId)))
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
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateConditionsTypeIsNotEmpty(null, null)).Name);

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
        internal void ValidateCanResolveNonEmptyConditionsType(ValidationContext context, EventSettings settings)
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
                    Reflector<EventSettingsValidations>.GetMethod(n => n.ValidateCanResolveNonEmptyConditionsType(null, null)).Name);

                throw;
            }
        }

        private static Type GetEventType(string typeId, ValidationContext context)
        {
            if (string.IsNullOrEmpty(typeId))
                return null;

            var eventType = Events[typeId].Select(value => value.Value).FirstOrDefault();

            if (eventType == null)
            {
                eventType = context.TryGetProjectComponentType<IObservableEvent>(typeId);
                //eventType = (from type in this.ProjectTypeProvider.GetTypes<IObservableEvent>()
                //             let component = type.AsFeatureComponent()
                //             where component != null && component.Id == typeId
                //             select type)
                //             .FirstOrDefault();
            }

            return eventType;
        }
    }
}