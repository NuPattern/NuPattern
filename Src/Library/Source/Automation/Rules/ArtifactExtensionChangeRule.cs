using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// Change rule for <see cref="ArtifactExtension"/> domain class.
	/// </summary>
	[RuleOn(typeof(ArtifactExtension), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ArtifactExtensionChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles the property change event for the AssociatedArtifact properties.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if (e.DomainProperty.Id == ArtifactExtension.OnArtifactActivationDomainPropertyId)
			{
				// Ensure we are not in deserialization mode.
				if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
					ArtifactExtension artifactExtension = (ArtifactExtension)e.ModelElement;
					if (artifactExtension != null)
					{
						artifactExtension.EnsureActivateArtifactExtensionAutomation();
					}
				}
			}
		}
	}
}