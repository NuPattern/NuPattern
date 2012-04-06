using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Immutability;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Triggers this notification rule whether a <see cref="ViewSchema"/> is added.
	/// </summary>
	[RuleOn(typeof(ViewSchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ViewSchemaAddRule : AddRule
	{
		/// <summary>
		/// Triggers this notification rule whether a <see cref="ViewSchema"/> is added.
		/// </summary>
		/// <param name="e">The provided data for this event.</param>
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			var view = (ViewSchema)e.ModelElement;
			if (view.Store.TransactionManager.CurrentTransaction.IsSerializing)
			{
				if (view.IsDefault)
				{
					view.SetLocks(Locks.Delete);
				}
			}
		}
	}
}