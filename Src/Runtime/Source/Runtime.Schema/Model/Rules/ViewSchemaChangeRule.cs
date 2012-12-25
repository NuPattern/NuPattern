using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Immutability;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Triggers this notification rule whether a <see cref="ViewSchema"/> is updated.
	/// </summary>
	[RuleOn(typeof(ViewSchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ViewSchemaChangeRule : ChangeRule
	{
		/// <summary>
		/// Triggers this notification rule whether a <see cref="ViewSchema"/> is updated.
		/// </summary>
		/// <param name="e">The provided data for this event.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
			{
				var view = (ViewSchema)e.ModelElement;

				if (view.IsDefault)
				{
					var otherViews = view.Pattern.Views.Where(vw => vw != view && vw.IsDefault);

					view.Store.TransactionManager.DoWithinTransaction(() =>
					{
						foreach (var otherView in otherViews)
						{
							otherView.SetLocks(Locks.None);
							otherView.IsDefault = false;
						}
					});

					view.SetLocks(Locks.Delete);
				}
				else if (view.Pattern.Views.Count == 1 || !view.Pattern.Views.Any(vw => vw.IsDefault))
				{
					view.WithTransaction(vw => vw.IsDefault = true);
				}
			}
		}
	}
}