using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the PatternSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class PatternSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PatternSchema>();

        /// <summary>
        /// Validates if there is more than one default view.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateAtLeastOneView(ValidationContext context)
        {
            try
            {
                if (!this.Views.Any())
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PatternAtLeastOneView, this.Name),
                        Resources.Validate_PatternAtLeastOneViewCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternSchema>.GetMethod(n => n.ValidateAtLeastOneView(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if there is more than one default view.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateSingleDefaultView(ValidationContext context)
        {
            try
            {
                if (this.Views.Where(view => view.IsDefault).Count() != 1)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PatternSingleDefaultView, this.Name),
                        Resources.Validate_PatternSingleDefaultViewCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternSchema>.GetMethod(n => n.ValidateSingleDefaultView(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if there is at least one visible view.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateAtLeastOneVisibleView(ValidationContext context)
        {
            try
            {
                if (!this.Views.Where(view => view.IsVisible).Any())
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PatternAtLeastOneVisibleView, this.Name),
                        Resources.Validate_PatternAtLeastOneVisibleViewCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternSchema>.GetMethod(n => n.ValidateAtLeastOneVisibleView(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if the pattern has the default name.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidatePatternNotNamedDefault(ValidationContext context)
        {
            try
            {
                if (this.Name.Equals(Resources.PatternSchema_DefaultName, StringComparison.OrdinalIgnoreCase))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PatternNotNamedDefault, this.Name),
                        Resources.Validate_PatternNotNamedDefaultCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<PatternSchema>.GetMethod(n => n.ValidatePatternNotNamedDefault(context)).Name);

                throw;
            }
        }
    }
}