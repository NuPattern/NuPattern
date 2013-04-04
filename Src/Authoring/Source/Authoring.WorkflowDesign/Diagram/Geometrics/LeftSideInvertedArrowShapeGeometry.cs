using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// A custom geometric shape, based upon the rounded rectangle that replaces the left side with an inverted arrow.
    /// </summary>
    internal class LeftSideInvertedArrowShapeGeometry : GlowShadowRoundedRectangleShapeGeometry
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
                new PointD(boundingBox.Left + ArrowWidth, (boundingBox.Top + (boundingBox.Height / 2))),
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

            // Top side line
            this.DrawLine(basePath, boundingBox.Left, boundingBox.Top,
                (boundingBox.Right - radius), boundingBox.Top);

            // Top Right corner arc
            this.DrawRadiusedArc(basePath, (boundingBox.Right - (2 * radius)), boundingBox.Top, 270f, 90f);

            // Right side line
            this.DrawLine(basePath, boundingBox.Right, (boundingBox.Top + radius),
                boundingBox.Right, (boundingBox.Bottom - radius));

            // Bottom Right corner arc
            this.DrawRadiusedArc(basePath, (boundingBox.Right - (2 * radius)), (boundingBox.Bottom - (2 * radius)), 0f, 90f);

            // Bottom side line
            this.DrawLine(basePath, (boundingBox.Right - radius), boundingBox.Bottom,
                boundingBox.Left, boundingBox.Bottom);

            // Left Arrow bottom line
            this.DrawLine(basePath, boundingBox.Left, boundingBox.Bottom,
                (boundingBox.Left + ArrowWidth), (boundingBox.Top + (boundingBox.Height / 2)));

            // Left Arrow top line
            this.DrawLine(basePath, (boundingBox.Left + ArrowWidth), (boundingBox.Top + (boundingBox.Height / 2)),
                boundingBox.Left, boundingBox.Top);

            basePath.CloseFigure();

            return basePath;
        }
    }
}
