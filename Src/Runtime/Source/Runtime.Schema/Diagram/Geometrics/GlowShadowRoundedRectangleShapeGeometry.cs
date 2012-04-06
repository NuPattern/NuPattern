using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Rounded rectangle geometry with custom drop shadow.
    /// </summary>
    internal abstract class GlowShadowRoundedRectangleShapeGeometry : RoundedRectangleShapeGeometry
    {
        private static new readonly SizeD MaximumShadowOffset = new SizeD(0.03, 0.03);
        private static readonly Color ShadowColor = Color.Black;
        private const int ShadowColorOpacity = 128;
        private const float ShadowMagnifier = 0.15f;
        private const float ShadowGradientFocalPoint = 0.75f;

        /// <summary>
        /// Gets the offset of the shadow.
        /// </summary>
        public override SizeD ShadowOffset
        {
            get
            {
                return MaximumShadowOffset;
            }
        }

        /// <summary>
        /// Draws the shape's shadow.
        /// </summary>
        /// <remarks>
        /// The shadow is replaced with a glow.
        /// </remarks>
        protected override void DoPaintShadow(DiagramPaintEventArgs e, IGeometryHost geometryHost)
        {
            Guard.NotNull(() => geometryHost, geometryHost);
            Guard.NotNull(() => e, e);

            Graphics graphics = e.Graphics;
            GraphicsState state = graphics.Save();
            SizeD shadowOffset = this.ShadowOffset;
            try
            {
                GraphicsPath shapePath = this.GetPath(geometryHost);
                RectangleF shapeRectangle = shapePath.GetBounds();

                // Create shadow path
                GraphicsPath shadowPath = shapePath.Clone() as GraphicsPath;

                // Enlarge the shadow (by fixed magnifier amount)
                using (Matrix scaleMatrix = new Matrix())
                {
                    scaleMatrix.Scale(
                        (ShadowMagnifier / shapeRectangle.Width) + 1,
                        (ShadowMagnifier / shapeRectangle.Height) + 1);
                    shadowPath.Transform(scaleMatrix);

                    // Center shadow back on the shape
                    RectangleF shadowRectangle = shadowPath.GetBounds();
                    scaleMatrix.Reset();
                    scaleMatrix.Translate(
                        -((shadowRectangle.X + (shadowRectangle.Width / 2)) - (shapeRectangle.X + (shapeRectangle.Width / 2))),
                        -((shadowRectangle.Y + (shadowRectangle.Height / 2)) - (shapeRectangle.Y + (shapeRectangle.Height / 2))));
                    shadowPath.Transform(scaleMatrix);
                }

                // Set the clip region (on the shape)
                using (Region clip = graphics.Clip)
                {
                    graphics.SetClip(shapePath);

                    // Offset the shadow path (move diagonally down-right) from shape
                    using (Matrix translateMatrix = new Matrix())
                    {
                        translateMatrix.Translate((float)shadowOffset.Width, (float)shadowOffset.Height);
                        shadowPath.Transform(translateMatrix);
                    }

                    // Mask-off the shadow from the original shape
                    graphics.SetClip(shadowPath, CombineMode.Complement);
                    graphics.SetClip(clip, CombineMode.Intersect);

                    // Fill the shadow
                    using (PathGradientBrush shadowBrush = new PathGradientBrush(shadowPath))
                    {
                        shadowBrush.CenterColor = Color.FromArgb(ShadowColorOpacity, ShadowColor);
                        shadowBrush.SurroundColors = new Color[] { Color.Transparent };
                        shadowBrush.FocusScales = new PointF(ShadowGradientFocalPoint, ShadowGradientFocalPoint);
                        graphics.FillPath(shadowBrush, shadowPath);
                        graphics.ResetClip();
                    }
                }
            }
            finally
            {
                graphics.Restore(state);
            }
        }
    }
}
