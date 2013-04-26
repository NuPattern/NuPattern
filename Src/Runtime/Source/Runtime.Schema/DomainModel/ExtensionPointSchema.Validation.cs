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
    /// Customizations for the ExtensionPointSchema class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    partial class ExtensionPointSchema
    {
        private static readonly ITracer tracer = Tracer.Get<ExtensionPointSchema>();

        /// <summary>
        /// Validates if an extension point has a duplicate name (with another element) within the owner.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateNameIsUnique(ValidationContext context)
        {
            try
            {
                IEnumerable<PatternElementSchema> sameNamedElements;
                if (this.View != null)
                {
                    // Get siblings in the owning view
                    sameNamedElements = this.View.AllElements()
                        .Where(element => element.Name.Equals(this.Name, System.StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    // Get siblings in the owning element
                    sameNamedElements = this.Owner.AllElements()
                        .Where(element => element.Name.Equals(this.Name, System.StringComparison.OrdinalIgnoreCase));
                }

                if (sameNamedElements.Count() > 1)
                {
                    context.LogError(
                        string.Format(CultureInfo.CurrentCulture, Resources.Validate_PatternElementNameIsNotUnique, this.Name),
                        Resources.Validate_PatternElementNameIsNotUniqueCode, this);
                }
            }
            catch (Exception ex)
            {
                tracer.Error(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<ExtensionPointSchema>.GetMethod(n => n.ValidateNameIsUnique(context)).Name);

                throw;
            }
        }


        /// <summary>
        /// Validates if an element or collection with the same OrderGroup has multiple OrderGroupComparers defined.
        /// </summary>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateOrderGroupComparerIsUniqueToGroup(ValidationContext context)
        {
            try
            {
                IEnumerable<IContainedElementInfo> siblingOrderGroupElements;

                if (this.View != null)
                {
                    // Get siblings in the owning view
                    siblingOrderGroupElements = this.View.AllElements().OfType<IContainedElementInfo>()
                        .Where(element => element.OrderGroup.Equals(this.OrderGroup));
                }
                else
                {
                    // Get siblings in the owning element
                    siblingOrderGroupElements = this.Owner.AllElements().OfType<IContainedElementInfo>()
                        .Where(element => element.OrderGroup.Equals(this.OrderGroup));
                }

                if (siblingOrderGroupElements.Count() > 1)
                {
                    if (!this.IsDefaultOrderComparer())
                    {
                        if (siblingOrderGroupElements
                            .Where(se =>
                                !se.IsDefaultOrderComparer()
                                && !se.OrderGroupComparerTypeName.Equals(this.OrderGroupComparerTypeName, StringComparison.OrdinalIgnoreCase))
                            .Any())
                        {
                            context.LogError(
                                string.Format(CultureInfo.CurrentCulture, Resources.Validate_ContainedElementInfoOrderGroupComparerIsUniqueToGroup, this.Name),
                                Resources.Validate_ContainedElementInfoOrderGroupComparerIsUniqueToGroupCode, this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.Error(
                    ex,
                    Resources.ValidationMethodFailed_Error,
                    Reflector<AbstractElementSchema>.GetMethod(n => n.ValidateNameIsUnique(context)).Name);

                throw;
            }
        }
    }
}