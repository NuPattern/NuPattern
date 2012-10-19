using System;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Schema PatternModel Designer.
    /// </summary>
    public partial class PatternModelDomainModel
    {
        /// <summary>
        /// Gets the list of non-generated domain model types.
        /// </summary>
        /// <returns>List of types.</returns>
        protected override Type[] GetCustomDomainModelTypes()
        {
            return new[]
			{ 
				typeof(NamedElementAddRule),
				typeof(CustomizableElementAddRule), 
				typeof(CustomizableElementChangeRule), 
				typeof(PatternSchemaAddRule),
				typeof(ViewSchemaAddRule), 
				typeof(ViewSchemaChangeRule), 
				typeof(FixUpMultipleDiagram),
				typeof(PropertySchemaChangeRule),
				typeof(ElementHasElementsChangeRule),
				typeof(PatternModelSchemaDiagramChangeRule),
				typeof(ElementSchemaAddRule),
				typeof(ExtensionPointSchemaAddRule),
			};
        }
    }
}