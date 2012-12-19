using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customizes a change in the properties of the <see cref="ProducedAsset"/> class.
	/// </summary>
	[RuleOn(typeof(ProducedAsset), FireTime = TimeToFire.TopLevelCommit)]
	public partial class ProducedAssetPropertyChangeRule : ChangeRule
	{
		/// <summary>
		/// Handles the property change event for this element.
		/// </summary>
		public override void ElementPropertyChanged(ElementPropertyChangedEventArgs e)
		{
			Guard.NotNull(() => e, e);
			base.ElementPropertyChanged(e);

			// Change the color of the shape for this element
			if (e.DomainProperty.Id == ProducedAsset.IsFinalDomainPropertyId)
			{
				ProducedAsset element = e.ModelElement as ProducedAsset;
				if (element != null)
				{
					// Get shape for the mel
					var producedAssetShape = PresentationViewsSubject.GetPresentation(element).OfType<ProducedAssetShape>().FirstOrDefault();

					if (producedAssetShape != null)
					{
						producedAssetShape.SetShapeBrushColor(DiagramBrushes.ShapeBackground, producedAssetShape.GetStereotypeFillColor());
					}
				}
			}
		}
	}
}
