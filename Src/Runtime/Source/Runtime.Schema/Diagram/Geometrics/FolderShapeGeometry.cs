using System.Drawing.Drawing2D;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// A custom geometric shape, based upon the rounded rectangle that replaces the top side with a folder tab.
	/// </summary>
	internal class FolderShapeGeometry : GlowShadowRoundedRectangleShapeGeometry
	{
		private const float TabWidth = 0.60f;
		private const float TabHeight = 0.12f;

		/// <summary>
		/// Gets the minimum allowable size of the shape.
		/// </summary>
		public override SizeD MinimumGeometrySize
		{
			get
			{
				double width = TabWidth + TabHeight + (4 * this.Radius);
				double height = TabHeight + (2 * this.Radius);
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
                new PointD(boundingBox.Left, (boundingBox.Top + (boundingBox.Height / 2))),
                new PointD(boundingBox.Right, (boundingBox.Top + (boundingBox.Height / 2))),
                new PointD((boundingBox.Left + (boundingBox.Width / 2)), boundingBox.Bottom),
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

			// Top of tab side line
			this.DrawLine(basePath, (boundingBox.Left + radius), boundingBox.Top,
				(boundingBox.Left + TabWidth), boundingBox.Top);

			// Tab top arc
			this.DrawRadiusedArc(basePath, (boundingBox.Left + TabWidth), boundingBox.Top, 270f, 45f);

			// Tab slope line
			// This line will be joined in automatically

			// Tab bottom arc
			this.DrawRadiusedArc(basePath,
				(boundingBox.Left + TabWidth + TabHeight + radius), (boundingBox.Top + TabHeight - (2 * radius)), 135f, -45f);

			// Top side line
			this.DrawLine(basePath, (boundingBox.Left + TabWidth + TabHeight + (2 * radius)), (boundingBox.Top + TabHeight),
				(boundingBox.Right - radius), (boundingBox.Top + TabHeight));

			// Top Right corner arc
			this.DrawRadiusedArc(basePath, (boundingBox.Right - (2 * radius)), (boundingBox.Top + TabHeight), 270f, 90f);

			// Right side line
			this.DrawLine(basePath, boundingBox.Right, (boundingBox.Top + radius + TabHeight),
				boundingBox.Right, boundingBox.Bottom);

			// Bottom side line
			this.DrawLine(basePath, boundingBox.Right, boundingBox.Bottom,
				(boundingBox.Left + radius), boundingBox.Bottom);

			// bottom left corner arc
			this.DrawRadiusedArc(basePath, (boundingBox.Left), (boundingBox.Bottom - (2 * radius)), 90f, 90f);

			// Left side line
			this.DrawLine(basePath, boundingBox.Left, (boundingBox.Bottom - radius),
				boundingBox.Left, (boundingBox.Top + radius));

			basePath.CloseFigure();

			return basePath;
		}
	}
}
