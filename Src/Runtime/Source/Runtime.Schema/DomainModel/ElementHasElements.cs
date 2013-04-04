using System.ComponentModel;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// ElementHasChildElements relationship definition.
    /// </summary>
    [TypeDescriptionProvider(typeof(ContainingLinkSchemaTypeDescriptionProvider))]
    partial class ElementHasElements : IContainingLinkSchema
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