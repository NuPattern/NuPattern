
namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizations to the <see cref="DesignConnector"/> class.
	/// </summary>
	public partial class DesignConnector
	{
		/// <summary>
		/// Prevent user from moving connector from connector points.
		/// </summary>
		public override bool CanMoveAnchorPoints
		{
			get
			{
				return true;
			}
		}
	}
}
