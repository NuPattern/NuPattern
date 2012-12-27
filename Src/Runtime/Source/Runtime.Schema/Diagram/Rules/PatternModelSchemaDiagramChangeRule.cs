using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Change rule for properties on the <see cref="PatternModelSchemaDiagram"/> class.
    /// </summary>
    [RuleOn(typeof(PatternModelSchemaDiagram), FireTime = TimeToFire.TopLevelCommit)]
    public partial class PatternModelSchemaDiagramChangeRule : ChangeRule
    {
        /// <summary>
        /// Handles property change events for the listed classes of this rule.
        /// </summary>
        /// <param name="e">The event args.</param>
        public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
        {
            Guard.NotNull(() => e, e);

            if (e.DomainProperty.Id == PatternModelSchemaDiagram.ShowHiddenEntriesDomainPropertyId)
            {
                if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
                {
                    var diagram = (PatternModelSchemaDiagram)e.ModelElement;

                    // Redraw all the compartments
                    var compartmentShapes = diagram.GetShapes<CompartmentShape>();
                    foreach (var shape in compartmentShapes)
                    {
                        foreach (CompartmentMapping mapping in shape.GetCompartmentMappings())
                        {
                            mapping.InitializeCompartmentShape(shape);
                        }
                    }
                }
            }
        }
    }
}