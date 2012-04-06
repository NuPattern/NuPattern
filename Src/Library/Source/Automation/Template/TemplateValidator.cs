using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Automation.Template
{
    class TemplateValidator
    {
        private UnfoldVsTemplateCommand.UnfoldVsTemplateSettings settings;
        private ITraceSource tracer;
        private ValidationContext context;
        private string name;
        private IServiceProvider serviceProvider;

        public TemplateValidator(string name, UnfoldVsTemplateCommand.UnfoldVsTemplateSettings unfoldVsTemplateSettings, ValidationContext context, IServiceProvider serviceProvider)
        {
            this.name = name;
            this.settings = unfoldVsTemplateSettings;
            this.context = context;
            this.serviceProvider = serviceProvider;
        }

        public void ValidateTemplateSettings(ITraceSource tracer)
        {
            this.tracer = tracer;
            this.ValidateAssociatedArtifactConfiguration();
            this.ValidateAuthoringUriIsValidAndTemplateIsConfiguredCorrectly(true);
            this.ValidateHidden();
            this.ValidateNoTwoTLPsInElement();
            this.ValidateTemplateUriIsNotEmpty();
        }

        public void ValidateCommandSettings(ITraceSource tracer)
        {
            this.tracer = tracer;
            this.ValidateAssociatedArtifactConfiguration();
            this.ValidateAuthoringUriIsValidAndTemplateIsConfiguredCorrectly(false);
            this.ValidateTemplateUriIsNotEmpty();
        }

        /// <summary>
        /// Validates that the element has a maximum of 1 TLP
        /// </summary>
        private void ValidateNoTwoTLPsInElement()
        {
            try
            {
                if (settings.OwnerElement.GetAutomationSettings<ITemplateSettings>().Count() > 1)
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsMoreThan1TLP,
                            settings.OwnerElement.Name),
                        Resources.Validate_TemplateSettingsMoreThan1TLPCode, settings.OwnerElement as ModelElement);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateValidator>.GetMethod(n => n.ValidateNoTwoTLPsInElement()).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that the Uri is not empty, and exists.
        /// </summary>
        internal void ValidateTemplateUriIsNotEmpty()
        {
            try
            {
                // Ensure not empty
                if (string.IsNullOrEmpty(settings.TemplateUri))
                {
                    context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsUriIsNotEmpty,
                            this.name),
                        Resources.Validate_TemplateSettingsUriIsNotEmptyCode, settings.SettingsElement as ModelElement);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateValidator>.GetMethod(n => n.ValidateTemplateUriIsNotEmpty()).Name);

                throw;
            }
        }

        internal void ValidateHidden()
        {
            if (!string.IsNullOrEmpty(settings.TemplateAuthoringUri))
            {
                IItemContainer item = GetVsTemplateProjectItem();

                if (item != null)
                {
                    //Doing all the validation in one method to avoid the performance impact of reading the file from disc
                    IVsTemplate template = VsTemplateFile.Read(item.PhysicalPath);
                    if (settings.CreateElementOnUnfold && template.TemplateData.Hidden.HasValue && template.TemplateData.Hidden.Value)
                    {
                        //should not be hidden if created on unfold
                        context.LogWarning(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsHiddenIsInvalidWhenCreateOnUnfold,
                            settings.OwnerElement.Name),
                        Resources.Validate_TemplateSettingsHiddenIsInvalidWhenCreateOnUnfoldCode, settings.SettingsElement as ModelElement);
                    }
                }
            }
        }

        private IItemContainer GetVsTemplateProjectItem()
        {
            var uriService = serviceProvider.GetService<IFxrUriReferenceService>();
            IItemContainer item = null;
            try
            {
                item = uriService.ResolveUri<IItemContainer>(new Uri(settings.TemplateAuthoringUri));
            }
            catch (NotSupportedException)
            {
            }
            catch (UriFormatException)
            {
            }
            return item;
        }

        internal void ValidateAuthoringUriIsValidAndTemplateIsConfiguredCorrectly(bool validatingTemplate)
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.TemplateAuthoringUri))
                {
                    IItemContainer item = GetVsTemplateProjectItem();
                    if (item != null)
                    {

                        //Doing all the validation in one method to avoid the performance impact of reading the file from disc
                        IVsTemplate template = VsTemplateFile.Read(item.PhysicalPath);
                        if (template == null)
                        {
                            if (!(settings.OwnerElement.Root.PatternModel.IsInTailorMode()))
                            {
                                context.LogError(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.Validate_TemplateSettingsAssociatedTemplateIsNotAValidTemplateFile,
                                    settings.OwnerElement.Name),
                                Resources.Validate_TemplateSettingsAssociatedTemplateIsNotAValidTemplateFileCode, settings.SettingsElement as ModelElement);
                            }
                            return;
                        }

                        if (!settings.TemplateUri.EndsWith(template.TemplateData.TemplateID, StringComparison.OrdinalIgnoreCase))
                        {
                            //wrong id
                            context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_TemplateSettingsTemplateIdDoesNotMatchReferencedTemplate,
                                this.name),
                            Resources.Validate_TemplateSettingsTemplateIdDoesNotMatchReferencedTemplateCode, settings.SettingsElement as ModelElement);
                        }

                        if (!((template.Type == VsTemplateType.Item && settings.TemplateUri.StartsWith(VsTemplateUriProvider.GetUriBase(VsTemplateType.Item), StringComparison.OrdinalIgnoreCase)) ||
                            (template.Type == VsTemplateType.Project && settings.TemplateUri.StartsWith(VsTemplateUriProvider.GetUriBase(VsTemplateType.Project), StringComparison.OrdinalIgnoreCase)) ||
                            (template.Type == VsTemplateType.ProjectGroup && settings.TemplateUri.StartsWith(VsTemplateUriProvider.GetUriBase(VsTemplateType.ProjectGroup), StringComparison.OrdinalIgnoreCase))))
                        {
                            //wrong uri for type
                            context.LogError(
                            string.Format(
                                CultureInfo.CurrentCulture,
                                Resources.Validate_TemplateSettingsTemplateTypeDoesNotMatchReferencedTemplate,
                                settings.OwnerElement.Name, template.Type),
                            Resources.Validate_TemplateSettingsTemplateTypeDoesNotMatchReferencedTemplateCode, settings.SettingsElement as ModelElement);
                        }

                        var elementReplacementsWizard = template.WizardExtension.GetExtension(typeof(ElementReplacementsWizard));

                        if (validatingTemplate)
                        {
                            var instantiationWizard = template.WizardExtension.GetExtension(typeof(InstantiationTemplateWizard));
                            if (instantiationWizard == null || elementReplacementsWizard == null)
                            {
                                //wizards not present
                                context.LogWarning(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.Validate_TemplateSettingsWizardsNotPresentInTemplate,
                                    settings.OwnerElement.Name),
                                Resources.Validate_TemplateSettingsWizardsNotPresentInTemplateCode, settings.SettingsElement as ModelElement);
                            }
                            else if (template.WizardExtension.ToList().IndexOf(instantiationWizard) > template.WizardExtension.ToList().IndexOf(elementReplacementsWizard))
                            {
                                //wizards in wrong order
                                context.LogWarning(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.Validate_TemplateSettingsWizardOrderIsNotCorrect,
                                    settings.OwnerElement.Name),
                                Resources.Validate_TemplateSettingsWizardOrderIsNotCorrectCode, settings.SettingsElement as ModelElement);
                            }
                        }
                        else
                        {
                            if (elementReplacementsWizard == null)
                            {
                                //wizards not present
                                context.LogWarning(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.Validate_TemplateSettingsWizardsNotPresentInTemplate,
                                    settings.OwnerElement.Name),
                                Resources.Validate_TemplateSettingsWizardsNotPresentInTemplateCode, settings.SettingsElement as ModelElement);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateValidator>.GetMethod(n => n.ValidateAuthoringUriIsValidAndTemplateIsConfiguredCorrectly(true)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that owner does not also have a <see cref="UnfoldVsTemplateCommand"/> with same template Uri.
        /// </summary>
        internal void ValidateAssociatedArtifactConfiguration()
        {
            try
            {
                if (!string.IsNullOrEmpty(settings.TemplateAuthoringUri))
                {
                    var uriService = serviceProvider.GetService<IFxrUriReferenceService>();
                    IItem item = null;
                    try
                    {
                        item = uriService.ResolveUri<IItemContainer>(new Uri(settings.TemplateAuthoringUri)).As<IItem>();
                    }
                    catch (NotSupportedException)
                    {
                        LogSolutionItemNotFound();
                        return;
                    }
                    catch (UriFormatException)
                    {
                        LogSolutionItemNotFound();
                        return;
                    }

                    if (!(string.Equals(item.Data.ItemType, "ProjectTemplate", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(item.Data.ItemType, "ItemTemplate", StringComparison.OrdinalIgnoreCase)))
                    {
                        //item type should be set to content
                        context.LogWarning(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_TemplateSettingsItemTypeShouldBeSetToTemplate,
                            settings.OwnerElement.Name),
                        Resources.Validate_TemplateSettingsItemTypeShouldBeSetToTemplateCode, settings.SettingsElement as ModelElement);
                    }

                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<TemplateValidator>.GetMethod(n => n.ValidateAssociatedArtifactConfiguration()).Name);

                throw;
            }
        }

        private void LogSolutionItemNotFound()
        {
            if (!settings.OwnerElement.Root.PatternModel.IsInTailorMode())
            {
                //item not found error
                context.LogError(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Validate_TemplateSettingsAuthoringUriIsInvalid,
                    settings.OwnerElement.Name),
                Resources.Validate_TemplateSettingsAuthoringUriIsInvalidCode, settings.SettingsElement as ModelElement);
            }
        }
    }

}
