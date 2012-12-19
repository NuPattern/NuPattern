
namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations for the <see cref="ProductionTool"/> class.
	/// </summary>
	public partial class ProductionTool
	{
		/// <summary>
		/// Returns the value of the IsSatisfyingVariability property.
		/// </summary>
		internal bool GetIsSatisfyingVariabilityValue()
		{
			return this.VariabilityRequirements.Count > 0;
		}
	}
}
