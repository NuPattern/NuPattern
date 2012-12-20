
namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// Customizations for the <see cref="VariabilityRequirement"/> class.
    /// </summary>
    public partial class VariabilityRequirement
    {
        /// <summary>
        /// Returns the value of the IsSatisfiedByTool property.
        /// </summary>
        internal bool GetIsSatisfiedByProductionToolValue()
        {
            return this.ProductionTools.Count > 0;
        }
    }
}
