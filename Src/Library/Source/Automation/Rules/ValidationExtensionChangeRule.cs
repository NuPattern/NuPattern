using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Library.Automation
{
	/// <summary>
	/// Change rule for <see cref="ValidationExtension"/> domain class.
	/// </summary>
	[RuleOn(typeof(ValidationExtension), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ValidationExtensionChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles the property change event for the set of validation properties.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if ((e.DomainProperty.Id == ValidationExtension.ValidationOnBuildDomainPropertyId)
				|| (e.DomainProperty.Id == ValidationExtension.ValidationOnCustomEventDomainPropertyId)
				|| (e.DomainProperty.Id == ValidationExtension.ValidationOnMenuDomainPropertyId)
				|| (e.DomainProperty.Id == ValidationExtension.ValidationOnSaveDomainPropertyId))
			{
				// Ensure we are not in deserialization mode.
				if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
					ValidationExtension validationExtension = (ValidationExtension)e.ModelElement;
					if (validationExtension != null)
					{
						validationExtension.EnsureValidationExtensionAutomation();
					}
				}
			}
		}
	}
}