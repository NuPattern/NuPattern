using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Automation.Template;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Custom validation rules.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class TemplateSettings
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<TemplateSettings>();

        [ValidationMethod(ValidationCategories.Menu | ValidationCategories.Save)]
        internal void ValidateAll(ValidationContext context)
        {
            var templateValidator = new TemplateValidator(this.Name,
                new UnfoldVsTemplateCommand.UnfoldVsTemplateSettings
                {
                    SanitizeName = this.SanitizeName,
                    SyncName = this.SyncName,
                    TemplateAuthoringUri = this.TemplateAuthoringUri,
                    TemplateUri = this.TemplateUri,
                    SettingsElement = (IAutomationSettingsSchema)this.Extends,
                    OwnerElement = this.Owner,
                    CreateElementOnUnfold = CreateElementOnUnfold
                }, context, this.Store);
            templateValidator.ValidateTemplateSettings(tracer);
        }

        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCommandIdIsValid(ValidationContext context)
        {
            try
            {
                if (this.CommandId != Guid.Empty)
                {
                    if (!this.Owner.GetAutomationSettings<ICommandSettings>().Any(c => c.Id.Equals(this.CommandId)))
                    {
                        if (this.Owner.IsInheritedFromBase &&
                            SettingsValidationHelper.TryToFixId<ICommandSettings>(this.Store, this.Owner, this.CommandId, id => this.CommandId = id))
                        {
                            return;
                        }

                        context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_CommandIsNotValid),
                            Resources.Validate_CommandIsNotValidCode, this.Extends);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateCommandIdIsValid(context)).Name);

                throw;
            }
        }

        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateWizardIdIsValid(ValidationContext context)
        {
            try
            {
                if (this.WizardId != Guid.Empty)
                {
                    if (!this.Owner.GetAutomationSettings<IWizardSettings>().Any(c => c.Id.Equals(this.WizardId)))
                    {
                        if (this.Owner.IsInheritedFromBase &&
                            SettingsValidationHelper.TryToFixId<IWizardSettings>(this.Store, this.Owner, this.WizardId, id => this.WizardId = id))
                        {
                            return;
                        }

                        context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_WizardIsNotValid),
                            Resources.Validate_WizardIsNotValidCode, this.Extends);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateWizardIdIsValid(context)).Name);

                throw;
            }
        }


        /// <summary>
        /// Validates that when no instantiation is configured that this is configured only on a pattern.
        /// </summary>
        /// <remarks>Having no instantiation options configured means this element ceases to define any context for the template unfold process, 
        /// so the template can be unfolded (without creating the element) at any time from any context. 
        /// Therfore, the most global context there is inthis toolkit is the pattern context, so these templates should be moved there for clarity.</remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNoInstantiationOnProductOnly(ValidationContext context)
        {
            try
            {
                var automationElement = this.Extends as IAutomationSettingsSchema;
                var ownerElement = automationElement.Parent as IPatternSchema;

                // Ensure not empty
                if ((this.CreateElementOnUnfold == false) && (this.UnfoldOnElementCreated == false)
                    && (ownerElement == null))
                {
                    context.LogWarning(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsNoInstantiationOnProductOnly,
                            this.Name),
                        Resources.Validate_TemplateSettingsNoInstantiationOnProductOnlyCode, this.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateNoInstantiationOnProductOnly(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that when no instantiation is configured that neither command nor wizard is configured.
        /// </summary>
        /// <remarks>Having no instantiation options configured means that this element never participates in the template unfold process,
        /// and therefore the command and wizard can never be run. Therefore this configuration is invalid.</remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNoAutomationWithNoInstantiation(ValidationContext context)
        {
            try
            {
                if ((this.CreateElementOnUnfold == false) && (this.UnfoldOnElementCreated == false)
                    && ((this.CommandId != Guid.Empty) || (this.WizardId != Guid.Empty)))
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsNoAutomationWithNoInstantiation,
                            this.Name),
                        Resources.Validate_TemplateSettingsNoAutomationWithNoInstantiationCode, this.Extends);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateNoAutomationWithNoInstantiation(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that Owner is a Product and not a Collection or Element.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateOwnerIsPattern(ValidationContext context)
        {
            try
            {
                var automationElement = this.Extends as IAutomationSettingsSchema;
                var ownerElement = automationElement.Parent as IPatternSchema;

                if (ownerElement == null)
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsOwnerIsPattern,
                            this.Owner.Name),
                        Resources.Validate_TemplateSettingsOwnerIsPatternCode, this.Owner as ModelElement);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateOwnerIsPattern(context)).Name);

                throw;
            }
        }


        /// <summary>
        /// Validates that Owning Product does not also have a <see cref="UnfoldVsTemplateCommand"/> with same template Uri.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateOwnerNotHaveUnfoldCommandWithSameTemplate(ValidationContext context)
        {
            try
            {
                var automationElement = this.Extends as IAutomationSettingsSchema;
                var ownerElement = automationElement.Parent as IPatternSchema;

                var unfoldCommands = ownerElement.GetAutomationSettings<CommandSettings>().Where(t => t.TypeId == typeof(UnfoldVsTemplateCommand).ToString());
                Func<IPropertyBindingSettings, bool> authoringProperty = p => p.Name == Reflector<UnfoldVsTemplateCommand>.GetPropertyName(t => t.TemplateAuthoringUri);

                if (unfoldCommands.Count() > 0 &&
                    unfoldCommands.Any(c => ((IBindingSettings)c).Properties.Any(authoringProperty) && ((IBindingSettings)c).Properties.First(authoringProperty).Value == this.TemplateAuthoringUri))
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsOwnerNotHaveUnfoldCommandWithSameTemplate,
                            this.Owner.Name),
                        Resources.Validate_TemplateSettingsOwnerNotHaveUnfoldCommandWithSameTemplateCode, this.Owner as ModelElement);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateSettings>.GetMethod(n => n.ValidateOwnerNotHaveUnfoldCommandWithSameTemplate(context)).Name);

                throw;
            }
        }
    }
}
