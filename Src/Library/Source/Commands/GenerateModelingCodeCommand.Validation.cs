using System;
using System.ComponentModel.Composition;
using System.Globalization;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Automation;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
    /// <summary>
    /// Validations for the <see cref="GenerateModelingCodeCommand"/> command
    /// </summary>
    [CommandValidationRule(typeof(GenerateModelingCodeCommand))]
    public class GenerateModelingCodeCommandValidation : ICommandValidationRule
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<UnfoldVsTemplateCommandValidation>();

        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider serviceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Called when validations are needed for the command
        /// </summary>
        /// <param name="context">Validation Context where to add validation errors</param>
        /// <param name="settingsElement">The settings element in the model being validated</param>
        /// <param name="settings">Settings for the command</param>
        public virtual void Validate(ValidationContext context, IAutomationSettingsSchema settingsElement, ICommandSettings settings)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => settings, settings);

            try
            {
                var authoringUri = settings.GetOrCreatePropertyValue(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TemplateAuthoringUri), string.Empty);
                if (!string.IsNullOrEmpty(authoringUri))
                {
                    var uriService = serviceProvider.GetService<IFxrUriReferenceService>();
                    IItem item = null;
                    try
                    {
                        item = uriService.ResolveUri<IItemContainer>(new Uri(authoringUri)).As<IItem>();
                    }
                    catch (NotSupportedException)
                    {
                        LogSolutionItemNotFound(context, settingsElement, settings);
                        return;
                    }
                    catch (UriFormatException)
                    {
                        LogSolutionItemNotFound(context, settingsElement, settings);
                        return;
                    }

                    if (!(string.Equals(item.Data.ItemType, BuildAction.Content.ToString(), StringComparison.OrdinalIgnoreCase)))
                    {
                        context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_GenerateModelingCodeCommandItemTypeShouldBeSetToContent,
                            settings.Name),
                        Resources.Validate_GenerateModelingCodeCommandItemTypeShouldBeSetToContentCode, settingsElement as ModelElement);
                    }

                    if (!(string.Equals(item.Data.IncludeInVSIX, Boolean.TrueString.ToUpperInvariant(), StringComparison.OrdinalIgnoreCase)))
                    {
                        context.LogError(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXShouldBeSetToTrue,
                            settings.Name),
                        Resources.Validate_GenerateModelingCodeCommandIIncludeInVSIXShouldBeSetToTrueCode, settingsElement as ModelElement);
                    }
                }
                else
                {
                    LogSolutionItemNotFound(context, settingsElement, settings);
                    return;
                }

                var targetFilename = settings.GetOrCreatePropertyValue(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetFileName), string.Empty);
                if (string.IsNullOrEmpty(targetFilename))
                {
                    context.LogError(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Validate_GenerateModelingCodeCommandTargetFilenameEmpty,
                        settings.Name),
                    Resources.Validate_GenerateModelingCodeCommandTargetFilenameEmptyCode, settingsElement as ModelElement);
                }

                var isConfiguredOnProduct = ((settingsElement.Parent as IPatternSchema) != null);
                var targetPath = settings.GetOrCreatePropertyValue(Reflector<GenerateModelingCodeCommand>.GetPropertyName(u => u.TargetPath), string.Empty);
                if ((!string.IsNullOrEmpty(targetPath))
                    && (targetPath.StartsWith(PathResolver.ResolveArtifactLinkCharacter, StringComparison.OrdinalIgnoreCase))
                    && !isConfiguredOnProduct)
                {
                    context.LogWarning(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.Validate_GenerateModelingCodeCommandTargetPathStartsWithResolver,
                        settings.Name, targetPath, PathResolver.ResolveArtifactLinkCharacter),
                    Resources.Validate_GenerateModelingCodeCommandTargetPathStartsWithResolverCode, settingsElement as ModelElement);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<GenerateModelingCodeCommandValidation>.GetMethod(n => n.Validate(null, null, null)).Name);

                throw;
            }
        }

        private static void LogSolutionItemNotFound(ValidationContext context, IAutomationSettingsSchema settingsElement, ICommandSettings settings)
        {
            if (!settings.Owner.Root.PatternModel.IsInTailorMode())
            {
                //item not found error
                context.LogError(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Validate_GenerateModelingCodeAuthoringUriIsInvalid,
                    settings.Name),
                Resources.Validate_GenerateModelingCodeAuthoringUriIsInvalidCode, settingsElement as ModelElement);
            }
        }
    }
}
