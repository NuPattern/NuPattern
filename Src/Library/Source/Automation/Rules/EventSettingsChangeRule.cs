using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
    /// Change rule for <see cref="EventSettings"/> domain class.
	/// </summary>
	[RuleOn(typeof(EventSettings), FireTime = TimeToFire.TopLevelCommit)]
	public partial class EventSettingsChangeRule : ChangeRule
	{
		/// <summary>
        /// Handles the property change event for the <see cref="EventSettings"/> properties.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

            if (e.DomainProperty.Id == EventSettings.FilterForCurrentElementDomainPropertyId)
			{
				// Ensure we are not in deserialization mode.
				if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
                    EventSettings eventSettings = (EventSettings)e.ModelElement;

					// Cleanup condition when property value is reset.
					if (eventSettings.FilterForCurrentElement == false)
					{
						eventSettings.DeleteConditionForFiltering();
					}
					else
					{
						// Ensure the correct configured automation
                        eventSettings.EnsureConditionForFiltering();
					}
				}
			}
		}
	}
}