using System;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Extensibility;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Change rule for property schema domain class.
	/// </summary>
	[RuleOn(typeof(PropertySchema), FireTime = TimeToFire.TopLevelCommit)]
	public partial class PropertySchemaChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles property change events for the listed classes of this rule.
		/// </summary>
		/// <param name="e">The event args.</param>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);

			if (e.DomainProperty.Id == PropertySchema.TypeDomainPropertyId)
			{
				if (!e.ModelElement.Store.TransactionManager.CurrentTransaction.IsSerializing)
				{
					var property = (PropertySchema)e.ModelElement;

					// Clear any previous DefaultValue of the property.
					var defaultValueName = Reflector<PropertySchema>.GetPropertyName(x => x.RawDefaultValue);
					var descriptor = TypeDescriptor.GetProperties(property)[defaultValueName];
					descriptor.ResetValue(property);

					// Repaint the owners compartment shape (to update the custom displayed text value)
					if (property.Owner != null)
					{
						foreach (var shape in PresentationViewsSubject.GetPresentation(property.Owner))
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
		}
	}
}