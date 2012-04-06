using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// ElementHasChildElements relationship definition.
    /// </summary>
    [TypeDescriptionProvider(typeof(ContainingLinkSchemaTypeDescriptionProvider))]
    public partial class ElementHasElements : IContainingLinkSchema
    {
        /// <summary>
        /// Returns the value of the Cardinality property.
        /// </summary>
        private string GetCardinalityCaptionValue()
        {
            return CardinalityConstants.ConvertToString(this.Cardinality);
        }
    }
}