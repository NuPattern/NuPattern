using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// A custom geometric shape based upon teh rounded rectangle shape.
    /// </summary>
    internal abstract class RoundedRectangleShapeGeometry : Microsoft.VisualStudio.Modeling.Diagrams.RoundedRectangleShapeGeometry
    {
        /// <summary>
        /// Gets the minimum allowable size of the shape.
        /// </summary>
        public abstract SizeD MinimumGeometrySize
        {
            get;
        }

        /// <summary>
        /// Gets the suggested connection points of this geometry.
        /// </summary>
        public abstract PointD[] GetGeometryConnectionPoints(IGeometryHost geometryHost);

        /// <summary>
        /// Draws a radiused arc on the given path, at given location.
        /// </summary>
        protected void DrawRadiusedArc(GraphicsPath path, double left, double top, float startAngle, float sweepAngle)
        {
            RectangleD arcRectangle = new RectangleD();
            arcRectangle.Width = (2 * this.Radius);
            arcRectangle.Height = (2 * this.Radius);

            path.AddArc((float)left, (float)top, (float)arcRectangle.Width, (float)arcRectangle.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a line on the given path.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Grouping instance methods.")]
        protected void DrawLine(GraphicsPath path, double fromX, double fromY, double toX, double toY)
        {
            path.AddLine((float)fromX, (float)fromY, (float)toX, (float)toY);
        }
    }
}
