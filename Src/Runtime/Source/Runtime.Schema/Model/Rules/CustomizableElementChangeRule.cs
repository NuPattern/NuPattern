using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

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
	public partial class CustomizableElementChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles property change events for the listed classes of this rule.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			// Ensure this elment is a customizable element
			var element = (CustomizableElementSchema)e.ModelElement;
			if (element != null)
			{
				if (!element.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
					// Repaint the top level compartment shape (to update decorators)
					if (e.DomainProperty.Id == CustomizableElementSchema.IsCustomizableDomainPropertyId)
					{
						if ((element is PatternSchema)
							|| (element is ViewSchema)
							|| (element is CollectionSchema)
							|| (element is ElementSchema)
							|| (element is ExtensionPointSchema))
						{
							RepaintCompartmentShape(element);
						}

						var property = element as PropertySchema;
						if (property != null)
						{
							RepaintCompartmentShape(property.Owner);
						}

						var automation = element as AutomationSettingsSchema;
						if (automation != null)
						{
							RepaintCompartmentShape(automation.Owner);
						}
					}
				}
			}
		}

		/// <summary>
		/// Repaints the compartment shape for the given element.
		/// </summary>
		private static void RepaintCompartmentShape(CustomizableElementSchema element)
		{
			Guard.NotNull(() => element, element);

			foreach (var shape in PresentationViewsSubject.GetPresentation(element))
			{
				CompartmentShape compartment = shape as CompartmentShape;
				if (compartment != null)
				{
					compartment.Invalidate();
				}
			}
		}
	}
}