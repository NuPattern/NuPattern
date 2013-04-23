using System;
using System.Globalization;
using Microsoft.VisualStudio.Modeling.Validation;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizes the CustomizableElement domain class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class CustomizableElementSchemaBase
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CustomizableElementSchemaBase>();

        /// <summary>
        /// Validates if an element does not have any modifications to its policy that are explicitly overridden by its customization state.
        /// </summary>
        /// <remarks>
        /// Notify the author if they have an element that is explicity not-customizable (IsCustomized=False), 
        /// but have modified some of the settings.
        /// </remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCustomizedSettingsOverriddenByCustomizableState(ValidationContext context)
        {
            try
            {
                if (this.IsCustomizationEnabled)
                {
                    if ((this.IsCustomizable == CustomizationState.False)
                        && (this.Policy.IsModified == true))
                    {
                        context.LogWarning(
                            string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_CustomizedSettingsOverriddenByCustomizableState, this.Name),
                            Properties.Resources.Validate_CustomizedSettingsOverriddenByCustomizableStateCode, this);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<CustomizableElementSchemaBase>.GetMethod(n => n.ValidateCustomizedSettingsOverriddenByCustomizableState(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an element is customizable but no customizations.
        /// </summary>
        /// <remarks>
        /// Notify the author if they have a customizable element, but no customizations currently configured.
        /// </remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCustomizedStateWithNoCustomizableSettings(ValidationContext context)
        {
            try
            {
                if (this.IsCustomizationEnabled && this.IsCustomizationPolicyModifyable &&
                    this.Policy.CustomizationLevel == CustomizedLevel.None)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_CustomizedStateWithNoCustomizableSettings, this.Name),
                        Resources.Validate_CustomizedStateWithNoCustomizableSettingsCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<CustomizableElementSchemaBase>.GetMethod(n => n.ValidateCustomizedStateWithNoCustomizableSettings(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that an element is not explicity customizable if ancestors are customizable.
        /// </summary>
        /// <remarks>
        /// Notify the author if a child element is explicitly customizable (IsCustomizable = true), when the a parent/ancestor is already customizable. 
        /// The explicit IsCustomizable=true on this chid element is redundant, and adds nothing (as long as parent/ancestor state does not change).
        /// </remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCustomizableStateRedundant(ValidationContext context)
        {
            try
            {
                if (this.IsCustomizationEnabled)
                {
                    // Check if customizable explicitly
                    if (this.IsCustomizable == CustomizationState.True)
                    {
                        var parent = this.GetParentCustomizationElement();
                        if (parent != null)
                        {
                            // Check if parent already enabled.
                            if (parent.IsCustomizationPolicyModifyable == true)
                            {
                                context.LogWarning(
                                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_CustomizableStateRedundantAndShouldInherit, this.Name),
                                    Properties.Resources.Validate_CustomizableStateRedundantAndShouldInheritCode, this);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<CustomizableElementSchemaBase>.GetMethod(n => n.ValidateCustomizableStateRedundant(context)).Name);

                throw;
            }
        }
    }
}
