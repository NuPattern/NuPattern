using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// A custom geometric shape, based upon the rounded rectangle that replaces the bottom corners with squared corners.
    /// </summary>
    internal class SquaredBottomShapeGeometry : GlowShadowRoundedRectangleShapeGeometry
    {
        /// <summary>
        /// Gets the minimum allowable size of the shape.
        /// </summary>
        public override SizeD MinimumGeometrySize
        {
            get
            {
                double width = (2 * this.Radius);
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
            return null;
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
                (boundingBox.Right - radius), boundingBox.Top);

            // Top Right corner arc
            this.DrawRadiusedArc(basePath, boundingBox.Right - (2 * radius), boundingBox.Top, 270f, 90f);

            // Right side line
            this.DrawLine(basePath, boundingBox.Right, (boundingBox.Top + radius),
                boundingBox.Right, boundingBox.Bottom);

            // Bottom side line
            this.DrawLine(basePath, boundingBox.Right, boundingBox.Bottom,
                boundingBox.Left, boundingBox.Bottom);

            // Left side line
            this.DrawLine(basePath, boundingBox.Left, boundingBox.Bottom,
                boundingBox.Left, (boundingBox.Top + radius));

            basePath.CloseFigure();

            return basePath;
        }
    }
}
