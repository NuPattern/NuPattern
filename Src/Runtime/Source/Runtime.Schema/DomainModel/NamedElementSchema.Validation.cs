using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Reflection;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the NamedElementSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    abstract partial class NamedElementSchema
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<NamedElementSchema>();

        private static readonly string[] ReservedCodeIdentifiers = new[] 
        { 
            "Name", // From the 'Display Name' of the "Runtime.Store.ProductElement.InstanceName" property
            Reflector<Runtime.IProductElement>.GetProperty(p => p.InstanceName).Name, // 'InstanceName'
            //"Pattern", // From the 'Display Name' of the "Runtime.Store.Pattern" element
            //"View", // From the 'Display Name' of the "Runtime.Store.View" element
            //"Collection", // From the 'Display Name' of the "Runtime.Store.Collection" element
            //"Element", // From the 'Display Name' of the "Runtime.Store.Element" element
            //"Property", // From the 'Display Name' of the "Runtime.Store.Property" element
            //"ProductElement", // From the 'Display Name' of the "Runtime.Store.Property" element
            //"Pattern", // From the 'Display Name' of the "Runtime.Store.Property" element
        };

        /// <summary>
        /// Validates if an element has a valid name.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNameIsLegal(ValidationContext context)
        {
            try
            {
                if (!DataFormats.IsValidCSharpIdentifier(this.Name))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementNameNotValid, this.Name),
                        Properties.Resources.Validate_NamedElementNameNotValidCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<NamedElementSchema>.GetMethod(n => n.ValidateNameIsLegal(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an element has a valid code identifier.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCodeIdentifierIsLegal(ValidationContext context)
        {
            try
            {
                if (!DataFormats.IsValidCSharpIdentifier(this.CodeIdentifier))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementCodeIdentifierNotValid, this.Name),
                        Properties.Resources.Validate_NamedElementCodeIdentifierNotValidCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<NamedElementSchema>.GetMethod(n => n.ValidateCodeIdentifierIsLegal(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an element has a duplicate code identifier (with any another element).
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCodeIdentifierIsUnique(ValidationContext context)
        {
            try
            {
                IEnumerable<NamedElementSchema> sameNamedElements = null;

                var property = this as PropertySchema;
                if (property != null)
                {
                    if (property.Owner != null)
                    {
                        sameNamedElements = property.Owner.Properties.OfType<PropertySchema>()
                            .Where(element => element.CodeIdentifier.Equals(this.CodeIdentifier, StringComparison.OrdinalIgnoreCase));
                    }
                }
                else
                {
                    var automation = this as AutomationSettingsSchema;
                    if (automation != null)
                    {
                        if (automation.Owner != null)
                        {
                            sameNamedElements = automation.Owner.AutomationSettings.OfType<AutomationSettingsSchema>()
                                .Where(element => element.CodeIdentifier.Equals(this.CodeIdentifier, StringComparison.OrdinalIgnoreCase));
                        }
                    }
                    else
                    {
                        sameNamedElements = this.Store.ElementDirectory.AllElements
                            .OfType<NamedElementSchema>().Where(element => element.CodeIdentifier.Equals(this.CodeIdentifier, StringComparison.OrdinalIgnoreCase));
                    }
                }

                if (sameNamedElements.Count() > 1)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementCodeIdentifierIsNotUnique, this.Name),
                        Properties.Resources.Validate_NamedElementCodeIdentifierIsNotUniqueCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<NamedElementSchema>.GetMethod(n => n.ValidateCodeIdentifierIsUnique(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an element has a valid display name.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateDisplayNameIsLegal(ValidationContext context)
        {
            try
            {
                if (!IsValidDisplayName(this.DisplayName))
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementDisplayNameNotValid, this.Name),
                        Properties.Resources.Validate_NamedElementDisplayNameNotValidCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<NamedElementSchema>.GetMethod(n => n.ValidateDisplayNameIsLegal(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Validates if an element code identifier is not a reserved identifier.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateCodeIdentifierIsNotReserved(ValidationContext context)
        {
            try
            {
                if (ReservedCodeIdentifiers.SingleOrDefault(cid => cid.Equals(this.CodeIdentifier, StringComparison.OrdinalIgnoreCase)) != null)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Properties.Resources.Validate_NamedElementCodeIdentifierIsNotReserved, this.Name,
                        string.Join(", ", ReservedCodeIdentifiers)),
                        Properties.Resources.Validate_NamedElementCodeIdentifierIsNotReservedCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.TraceError(
                    ex,
                    Properties.Resources.ValidationMethodFailed_Error,
                    Reflector<NamedElementSchema>.GetMethod(n => n.ValidateCodeIdentifierIsNotReserved(context)).Name);

                throw;
            }
        }

        /// <summary>
        /// Determines whether the given value is a valid display name.
        /// </summary>
        private static bool IsValidDisplayName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value.Length > DataFormats.DesignTime.MaxPropertyLength)
            {
                return false;
            }

            return Regex.IsMatch(value, DataFormats.DesignTime.DisplayNameExpression);
        }
    }
}
