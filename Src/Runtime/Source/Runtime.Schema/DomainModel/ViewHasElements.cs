using System.ComponentModel;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// ViewHasAbstractElements relationship definition.
    /// </summary>
    [TypeDescriptionProvider(typeof(ContainingLinkSchemaTypeDescriptionProvider))]
    partial class ViewHasElements : IContainingLinkSchema
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