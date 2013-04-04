using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NuPattern.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the <see cref="PatternElementConnector"/> class.
    /// </summary>
    partial class PatternElementConnector
    {
        /// <summary>
        /// Prevent user from moving connector from connector points.
        /// </summary>
        public override bool CanMoveAnchorPoints
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the tailoring shape styles for this connector.
        /// </summary>
        internal ITailoredShapeElementStyles TailoringColors
        {
            get
            {
                return new TailoredConnectorStyles(
                    this.TailoringTextColor,
                    this.TailoringOutlineColor);
            }
        }

        /// <summary>
        /// Initializes the resources for each instance of the connector.
        /// </summary>
        protected override void InitializeInstanceResources()
        {
            base.InitializeInstanceResources();

            // TODO: Need to find a better pattern for connectors, becuase this code never executes as a connector does not have a non-null MEL
            // to get to the target MEL from.

            // Set tailorable colors (based upon the target element)
            ElementLink link = this.Subject as ElementLink;
            if (link != null)
            {
                NamedElementSchema targetElement = DomainRoleInfo.GetTargetRolePlayer(link) as NamedElementSchema;
                if ((targetElement != null) && (targetElement.IsInheritedFromBase))
                {
                    // Set tailoring styles for this instance.
                    this.SetShapeBrushColor(DiagramBrushes.ConnectionLineDecorator, this.TailoringColors.TextColor);
                    this.SetShapePenColor(DiagramPens.ConnectionLine, this.TailoringColors.OutlineColor);
                    this.SetShapePenColor(DiagramPens.ConnectionLineDecorator, this.TailoringColors.OutlineColor);
                    this.SetShapeBrushColor(DiagramBrushes.ShapeText, this.TailoringColors.TextColor);
                }
                else
                {
                    // Reset to orginally configured styles for this instance.
                    this.InitializeResources(this.StyleSet);
                }
            }
            else
            {
                // Reset to orginally configured styles for this instance.
                this.InitializeResources(this.StyleSet);
            }
        }
    }
}