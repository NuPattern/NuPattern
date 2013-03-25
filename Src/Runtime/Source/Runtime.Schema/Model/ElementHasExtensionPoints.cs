using System.ComponentModel;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// ElementHasExtensionPoints relationship definition.
    /// </summary>
    [TypeDescriptionProvider(typeof(ContainingLinkSchemaTypeDescriptionProvider))]
    partial class ElementHasExtensionPoints : IContainingLinkSchema
    {
        /// <summary>
        /// Returns the value of the Cardinality property.
        /// </summary>
        private string GetCardinalityCaptionValue()
        {
            return CardinalityConstants.ConvertToString(this.Cardinality);
        }

        /// <summary>
        /// Gets whether the extension point can be created by default.
        /// </summary>
        public bool AutoCreate
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether to allow UI for adding the extension point.
        /// </summary>
        public bool AllowAddNew
        {
            get { return true; }
        }
    }
}