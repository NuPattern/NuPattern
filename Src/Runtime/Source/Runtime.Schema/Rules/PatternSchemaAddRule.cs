using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Triggers this notification rule whether a <see cref="PatternSchema"/> is added.
    /// </summary>
    [RuleOn(typeof(PatternSchema), FireTime = TimeToFire.TopLevelCommit)]
    internal class PatternSchemaAddRule : AddRule
    {
        /// <summary>
        /// Triggers this notification rule whether a <see cref="PatternSchema"/> is added.
        /// </summary>
        /// <param name="e">The provided data for this event.</param>
        public override void ElementAdded(ElementAddedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            var pattern = (PatternSchema)e.ModelElement;

            if (pattern.Store.TransactionManager.CurrentTransaction.IsSerializing)
            {
                //Set current diagram
                pattern.CurrentDiagramId = pattern.Store.GetDefaultView().DiagramId;

                // Reset illegal value
                if (pattern.IsCustomizable == CustomizationState.Inherited)
                {
                    pattern.IsCustomizable = CustomizationState.True;
                }
            }
            else
            {
                // Default for the pattern is True rather than Inherited as for 
                // every other customizable element, because it has no parent.
                pattern.IsCustomizable = CustomizationState.True;
            }
        }
    }
}