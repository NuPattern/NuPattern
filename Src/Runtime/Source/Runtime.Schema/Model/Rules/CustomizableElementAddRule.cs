using Microsoft.VisualStudio.Modeling;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Initialization when a Customizable Element is added to the model.
	/// </summary>
	[RuleOn(typeof(PatternSchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(ViewSchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(CollectionSchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(ElementSchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(ExtensionPointSchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(PropertySchema), FireTime = TimeToFire.TopLevelCommit)]
	[RuleOn(typeof(AutomationSettingsSchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class CustomizableElementAddRule : AddRule
	{
		/// <summary>
		/// Triggers this notification rule when a <see cref="CustomizableElementSchema"/> is added.
		/// </summary>
		/// <param name="e">The provided data for this event.</param>
		public override void ElementAdded(ElementAddedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			// Ensure this element has a customization policy and settings
			var element = (CustomizableElementSchema)e.ModelElement;
			if (element != null)
			{
				element.EnsurePolicyAndDefaultSettings();
			}
		}
	}
}