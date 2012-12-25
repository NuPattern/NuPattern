using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizes a change in the properties of the <see cref="SuppliedAsset"/> class.
	/// </summary>
	[RuleOn(typeof(SuppliedAsset), FireTime = TimeToFire.TopLevelCommit)]
	public partial class SuppliedAssetPropertyChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles the property change event for this element.
		/// </summary>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);
			base.ElementPropertyChanged(e);

			// Change the color of the shape for this element
			if (e.DomainProperty.Id == SuppliedAsset.IsUserSuppliedDomainPropertyId)
			{
				SuppliedAsset element = e.ModelElement as SuppliedAsset;
				if (element != null)
				{
					// Get shape for the mel
                    var suppliedAssetShape = PresentationViewsSubject.GetPresentation(element).OfType<SuppliedAssetShape>().FirstOrDefault();

					if (suppliedAssetShape != null)
					{
						suppliedAssetShape.SetShapeBrushColor(DiagramBrushes.ShapeBackground, suppliedAssetShape.GetStereotypeFillColor());
					}
				}
			}
		}
	}
}
