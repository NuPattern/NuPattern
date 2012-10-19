using System.Globalization;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Properties;

namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
    /// <summary>
    /// Cutomizes the <see cref="ProducedAsset"/> class.
    /// </summary>
    [ValidationState(ValidationState.Enabled)]
    public partial class ProducedAsset
    {
        /// <summary>
        /// Validates that a produced asset that is not an input to a tool, is final.
        /// </summary>
        /// <remarks>
        /// Notify the author if they have an deliverable asset not marked as final.
        /// </remarks>
        [ValidationMethod(ValidationCategories.Save | ValidationCategories.Menu)]
        internal void ValidateProducedAssetNotIntermediateAndNotFinal(ValidationContext context)
        {
            if (this.IsSuppliedToTool == false && this.IsFinal == false)
            {
                context.LogError(string.Format(
                    CultureInfo.CurrentCulture, Resources.Validate_ProducedAssetNotIntermediateAndNotFinal, this.Name),
                    Resources.Validate_ProducedAssetNotIntermediateAndNotFinalCode);
            }
        }
    }
}
