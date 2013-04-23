using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Extensibility;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Change rule for the <see cref="T:NuPattern.Runtime.Schema.PatternElementSchema"/> domain class.
    /// </summary>
    [RuleOn(PatternElementSchemaDomainClassId, FireTime = TimeToFire.TopLevelCommit)]
    internal class PatternElementSchemaAddRule : AddRule
    {
        private const string PatternElementSchemaDomainClassId = "dbe13a31-7dcd-4fbd-a601-18ca765e264e";

        /// <summary>
        /// Handles the element added event for the PatternElementSchema element.
        /// </summary>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            base.ElementAdded(e);

            // Append the automation extensions to the element
            if (e.ModelElement.TryGetExtension<GuidanceExtension>() == null) //Fix paste operations
            {
                e.ModelElement.AddExtension<GuidanceExtension>();
            }
            if (e.ModelElement.TryGetExtension<ArtifactExtension>() == null) //Fix paste operations
            {
                e.ModelElement.AddExtension<ArtifactExtension>();
            }
            if (e.ModelElement.TryGetExtension<ValidationExtension>() == null) //Fix paste operations
            {
                // Only want this extension on Products
                if (e.ModelElement is IPatternSchema)
                {
                    e.ModelElement.AddExtension<ValidationExtension>();
                }
            }
        }
    }
}
