using System.Drawing;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Extensions to the <see cref="NodeShape"/> class
    /// </summary>
    internal static class NodeShapeExtensions
    {
        /// <summary>
        /// Moves shape to specified position (in device units) relative to the shape's parent element.
        /// </summary>
        public static void MoveTo(this NodeShape shape, int x, int y)
        {
            shape.MoveTo(new Point(x, y));
        }

        /// <summary>
        /// Moves shape to specified position (in device units) relative to the shape's parent element.
        /// </summary>
        public static void MoveTo(this NodeShape shape, Point position)
        {
            var newBounds = new RectangleD(PointD.Empty, shape.Bounds.Size);
            newBounds.X = position.X;
            newBounds.Y = position.Y;

            shape.Bounds = newBounds;
        }

        /// <summary>
        /// Moves shape to specified position (in world units) relative to the diagram.
        /// </summary>
        public static void MoveTo(this NodeShape shape, double x, double y)
        {
            shape.MoveTo(new PointD(x, y));
        }

        /// <summary>
        /// Moves shape to specified position (in world units) relative to the diagram.
        /// </summary>
        public static void MoveTo(this NodeShape shape, PointD position)
        {
            var newBounds = new RectangleD(PointD.Empty, shape.AbsoluteBounds.Size);
            newBounds.X = position.X;
            newBounds.Y = position.Y;

            shape.AbsoluteBounds = newBounds;
        }
    }
}
