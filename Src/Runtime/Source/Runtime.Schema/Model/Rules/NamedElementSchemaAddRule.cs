using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Immutability;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Initialization when a Customizable Element is added to the model.
	/// </summary>
	[RuleOn(typeof(NamedElementSchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class NamedElementAddRule : AddRule
	{
		/// <summary>
		/// Triggers this notification rule when a <see cref="CustomizableElementSchema"/> is added.
		/// </summary>
		/// <param name="e">The provided data for this event.</param>
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			var element = (NamedElementSchema)e.ModelElement;
			if (element != null)
			{
				// Ensure only when de/serializing
				if (element.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
					if (element.IsInheritedFromBase)
					{
						element.SetLocks(Locks.Delete);
					}
				}
			}
		}
	}
}