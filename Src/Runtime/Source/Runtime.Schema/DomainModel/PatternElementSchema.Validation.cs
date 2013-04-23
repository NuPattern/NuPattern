using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Custom validation rules.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class PatternElementSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PatternElementSchema>();

        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateValidationRulesTypeIsNotEmpty(ValidationContext context)
        {
            try
            {
                foreach (var settings in this.ValidationSettings.Where(s => string.IsNullOrEmpty(s.TypeId)))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_ValidationRuleSettingsTypeIsNotEmpty, this.Name),
                        Resources.Validate_ValidationRuleSettingsTypeIsNotEmptyCode,
                        this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternElementSchema>.GetMethod(n => n.ValidateValidationRulesTypeIsNotEmpty(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates the icon if the element has one.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateIcon(ValidationContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.Icon))
                {
                    var uriService = ((IServiceProvider)this.Store).GetService<IUriReferenceService>();
                    ResourcePack resolvedIcon = null;

                    try
                    {
                        resolvedIcon = uriService.ResolveUri<ResourcePack>(new Uri(this.Icon));
                    }
                    catch (UriFormatException)
                    {
                        context.LogError(
                                string.Format(CultureInfo.CurrentCulture, Resources.Validate_NamedElementIconDoesNotPointToAValidFile, this.Name),
                                Resources.Validate_NamedElementIconDoesNotPointToAValidFileCode,
                                this);
                        return;
                    }

                    if (resolvedIcon == null)
                    {
                        context.LogError(
                                string.Format(CultureInfo.CurrentCulture, Resources.Validate_NamedElementIconDoesNotPointToAValidFile, this.Name),
                                Resources.Validate_NamedElementIconDoesNotPointToAValidFileCode,
                                this);
                        return;
                    }

                    if (resolvedIcon.Type == ResourcePackType.ProjectItem)
                    {
                        var item = resolvedIcon.GetItem();
                        if (item.Data.ItemType != @"Resource")
                        {
                            context.LogError(
                                    string.Format(CultureInfo.CurrentCulture, Resources.Validate_NamedElementIconIsNotAResource, this.Name, item.Name),
                                    Resources.Validate_NamedElementIconIsNotAResource,
                                    this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternElementSchema>.GetMethod(n => n.ValidateIcon(context)).Name);

                throw;
            }
        }


    }
}