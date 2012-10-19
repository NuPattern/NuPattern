using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime.Schema.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Customizations for the PropertySchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    public partial class PropertySchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PropertySchema>();

        private static readonly string[] ReservedDisplayNames = new[] 
        { 
            "Name", // From the Display Name of the "Runtime.Store.ProductElement.InstanceName" property
            "Notes", // From the Display Name of the "Runtime.Store.InstanceBase.Notes" property
            "Related Items", // From the Display Name of the "Runtime.Store.ProductElement.References" property
        };

        /// <summary>
        /// Validates that the Type property is valid.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateTypeIsLegal(ValidationContext context)
        {
            try
            {
                var type = System.Type.GetType(this.Type, false, true);
                if ((type == null) || (!PropertySchema.PropertyValueTypes.Any(p => p.DataType.Equals(type))))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PropertyTypeUnknown, this.Name, this.Type),
                        Resources.Validate_PropertyTypeUnknownCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateTypeIsLegal(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates of the given property has a value for its type.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCategoryIsLegal(ValidationContext context)
        {
            try
            {
                if (!IsValidCategory(this.Category))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PropertyCategoryNotValid, this.Name),
                        Resources.Validate_PropertyCategoryNotValidCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateCategoryIsLegal(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an property has a duplicate name (with another property) in the current container.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNameIsUnique(ValidationContext context)
        {
            try
            {
                IEnumerable<PropertySchema> sameNamedElements = this.Owner.Properties
                    .Where(property => property.Name == this.Name);

                if (sameNamedElements.Count() > 1)
                {
                    // Check if one of the properties is a system property
                    if (sameNamedElements.FirstOrDefault(property => property.IsSystem == true) != null)
                    {
                        context.LogError(
                            string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_PropertyNameSameAsSystem, this.Name),
                            Properties.Resources.Validate_PropertyNameSameAsSystemCode, this);
                    }
                    else
                    {
                        context.LogError(
                            string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_PropertyNameIsNotUnique, this.Name),
                            Properties.Resources.Validate_PropertyNameIsNotUniqueCode, this);
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateNameIsUnique(context)).Name);

                throw;
            }
        }

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
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateValidationRulesTypeIsNotEmpty(context)).Name);

                throw;
            }
        }

        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateDefaultValueHasValueOrValueProvider(ValidationContext context)
        {
            try
            {
                if (this.DefaultValue.HasValue() && this.DefaultValue.HasValueProvider())
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PropertyDefaultValueHasValueOrValueProvider, this.Name),
                        Resources.Validate_PropertyDefaultValueHasValueOrValueProviderCode,
                        this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateDefaultValueHasValueOrValueProvider(context)).Name);

                throw;
            }
        }

        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateValueProviderOrDefaultValue(ValidationContext context)
        {
            try
            {
                if (this.DefaultValue.IsConfigured() && this.ValueProvider.IsConfigured())
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PropertyValueProviderOrDefaultValue, this.Name),
                        Resources.Validate_PropertyValueProviderOrDefaultValueCode,
                        this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateValueProviderOrDefaultValue(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates that property display name is not reserved.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateDisplayNameIsNotReserved(ValidationContext context)
        {
            try
            {
                if (ReservedDisplayNames.SingleOrDefault(dname => dname.Equals(this.DisplayName, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_PropertyDisplayNameReserved, this.Name,
                        string.Join(", ", ReservedDisplayNames)),
                        Properties.Resources.Validate_PropertyDisplayNameReservedCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<PropertySchema>.GetMethod(n => n.ValidateDisplayNameIsNotReserved(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Determines whether the given value is a valid category.
        /// </summary>
        private static bool IsValidCategory(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Length > DataFormats.DesignTime.MaxPropertyLength)
            {
                return false;
            }

            return Regex.IsMatch(value, DataFormats.DesignTime.CategoryNameExpression);
        }
    }
}