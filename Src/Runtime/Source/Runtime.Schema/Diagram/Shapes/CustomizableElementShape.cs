using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Customizations for the <see cref="CustomizableElementShape"/> class.
	/// </summary>
	public partial class CustomizableElementShape
	{
		/// <summary>
		/// Gets the tailoring shape styles for this shape.
		/// </summary>
		internal abstract ITailoredShapeElementStyles TailoringColors
		{
			get;
		}

		/// <summary>
		/// Initializes the resources for each instance of the shape.
		/// </summary>
		protected override void InitializeInstanceResources()
		{
			base.InitializeInstanceResources();

			// Set tailorable colors
			NamedElementSchema element = this.Subject as NamedElementSchema;
			if ((element != null) && (element.IsInheritedFromBase))
			{
				// Set tailoring styles for this instance.
				this.SetShapeBrushColor(DiagramBrushes.ShapeBackground, this.TailoringColors.FillColor);
				this.SetShapeBrushColor(DiagramBrushes.ShapeText, this.TailoringColors.TextColor);
				this.SetShapePenColor(DiagramPens.ShapeOutline, this.TailoringColors.OutlineColor);

				// TODO: Set tailoring styles for items in all compartments
				////var compartmentsInfo = this.GetCompartmentDescriptions();
				////foreach (var compartmentInfo in compartmentsInfo)
				////{
				////    ElementListCompartment compartment = this.FindCompartment(compartmentInfo.Name) as ElementListCompartment;
				////    if (compartment != null)
				////    {
				////        foreach (var item in compartment.Items)
				////        {
				////        NamedElementSchema listedElement = item as NamedElementSchema;
				////        if ((listedElement != null) && (listedElement.IsInheritedFromBase))
				////        {
				////            compartment.
				////        }
				////        else
				////        {
				////            compartment.ItemTextColor =
				////        }
				////    }
				////}
			}
			else
			{
				// Reset to orginally configured styles for this instance.
				this.InitializeResources(this.StyleSet);
			}
		}
	}
}
