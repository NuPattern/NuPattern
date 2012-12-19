
namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// Customizations for the <see cref="Asset"/> class.
    /// </summary>
    public partial class Asset
    {
        /// <summary>
        /// Returns the value of the IsSuppliedToTool property.
        /// </summary>
        internal bool GetIsSuppliedToToolValue()
        {
            return this.AllProducingTools.Count > 0;
        }
    }
}
