using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// A custom geometric shape, based upon the rounded rectangle that replaces the rigth side with an arrow.
    /// </summary>
    internal class RightSideArrowShapeGeometry : GlowShadowRoundedRectangleShapeGeometry
    {
        private const float ArrowWidth = 0.3f;

        /// <summary>
        /// Gets the minimum allowable size of the shape.
        /// </summary>
        public override SizeD MinimumGeometrySize
        {
            get
            {
                double width = ArrowWidth + (2 * this.Radius);
                double height = (2 * this.Radius);
                return new SizeD(width, height);
            }
        }

        /// <summary>
        /// Gets the suggested connection points of this geometry.
        /// </summary>
        /// <value></value>
        public override PointD[] GetGeometryConnectionPoints(IGeometryHost geometryHost)
        {
            RectangleD boundingBox = this.GetPerimeterBoundingBox(geometryHost);

            return new PointD[] 
            {
                new PointD(boundingBox.Right, (boundingBox.Top + (boundingBox.Height / 2))),
            };
        }

        /// <summary>
        /// Returns the path of the shape.
        /// </summary>
        protected override GraphicsPath GetPath(RectangleD boundingBox)
        {
            GraphicsPath basePath = base.GetPath(boundingBox);
            double radius = this.Radius;

            // Ensure work to do.
            if (radius <= (double)0)
            {
                return basePath;
            }

            // Starting at top left and going clockwise.
            basePath.Reset();
            basePath.StartFigure();

            // Top Left corner arc
            this.DrawRadiusedArc(basePath, boundingBox.Left, boundingBox.Top, 180f, 90f);

            // Top side line
            this.DrawLine(basePath, (boundingBox.Left + radius), boundingBox.Top,
                (boundingBox.Right - ArrowWidth), boundingBox.Top);

            // Arrow top line
            this.DrawLine(basePath, (boundingBox.Right - ArrowWidth), boundingBox.Top,
                boundingBox.Right, (boundingBox.Top + (boundingBox.Height / 2)));

            // Arrow bottom line
            this.DrawLine(basePath, (boundingBox.Right), (boundingBox.Top + (boundingBox.Height / 2)),
                (boundingBox.Right - ArrowWidth), boundingBox.Bottom);

            // Bottom side line
            this.DrawLine(basePath, (boundingBox.Right - ArrowWidth), boundingBox.Bottom,
                (boundingBox.Left + radius), boundingBox.Bottom);

            // Bottom left corner arc
            this.DrawRadiusedArc(basePath, boundingBox.Left, (boundingBox.Bottom - (2 * radius)), 90f, 90f);

            // Left side line
            this.DrawLine(basePath, boundingBox.Left, (boundingBox.Bottom - radius),
                boundingBox.Left, (boundingBox.Top + radius));

            basePath.CloseFigure();

            return basePath;
        }
    }
}
