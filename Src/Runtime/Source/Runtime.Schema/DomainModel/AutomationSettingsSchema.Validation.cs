using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the AutomationSettingsSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class AutomationSettingsSchema
    {
        private static readonly ITracer tracer = Tracer.Get<AutomationSettingsSchema>();

        /// <summary>
        /// Validates if an property has a duplicate name (with another property) in the current container.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNameIsUnique(ValidationContext context)
        {
            try
            {
                IEnumerable<AutomationSettingsSchema> sameNamedElements = this.Owner.AutomationSettings
                    .Where(setting => setting.Name.Equals(this.Name, System.StringComparison.OrdinalIgnoreCase));

                if (sameNamedElements.Count() > 1)
                {
                    // Check if one of the properties is a system property
                    if (sameNamedElements.FirstOrDefault(property => property.IsSystem == true) != null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_AutomationSettingsNameSameAsSystem, this.Name),
                            Properties.Resources.Validate_AutomationSettingsNameSameAsSystemCode, this);
                    }
                    else
                    {
                        context.LogError(
                            string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_AutomationSettingsNameIsNotUnique, this.Name),
                            Properties.Resources.Validate_AutomationSettingsNameIsNotUniqueCode, this);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.Error(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<AutomationSettingsSchema>.GetMethod(n => n.ValidateNameIsUnique(context)).Name);

                throw;
            }
        }
    }
}