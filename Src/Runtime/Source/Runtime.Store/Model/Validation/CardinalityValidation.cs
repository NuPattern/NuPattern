using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.Store
{
    /// <summary>
    /// Performs runtime cardinality validation.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    public partial class InstanceBase
    {
        /// <summary>
        /// Validates the cardinality.
        /// </summary>
        /// <param name="context">The context.</param>
        [ValidationMethod(ValidationCategories.Custom, CustomCategory = ValidationConstants.RuntimeValidationCategory)]
        public void ValidateCardinality(ValidationContext context)
        {
            Guard.NotNull(() => context, context);

            var elementContainer = this as IElementContainer;

            if (elementContainer != null && elementContainer.Info != null)
            {
                elementContainer.Info.Elements.ForEach(element =>
                    {
                        var elementCount = elementContainer.Elements.Count(e => e.Info == element);

                        if (((element.Cardinality == Cardinality.OneToOne) || (element.Cardinality == Cardinality.OneToMany))
                            && (elementCount < 1))
                        {
                            context.LogError(
                                string.Format(
                                    CultureInfo.CurrentCulture, Resources.Cardinality_ValidationAtLeastOne,
                                    elementContainer.GetInstanceName(), element.DisplayName),
                                Resources.Cardinality_ValidationAtLeastOneCode,
                                this);
                        }

                        if (((element.Cardinality == Cardinality.OneToOne) || (element.Cardinality == Cardinality.ZeroToOne))
                            && (elementCount > 1))
                        {
                            context.LogError(
                                string.Format(
                                    CultureInfo.CurrentCulture, Resources.Cardinality_ValidationAtMostOne,
                                    elementContainer.GetInstanceName(), element.DisplayName),
                                Resources.Cardinality_ValidationAtMostOneCode,
                                this);
                        }
                    });
            }
        }
    }
}