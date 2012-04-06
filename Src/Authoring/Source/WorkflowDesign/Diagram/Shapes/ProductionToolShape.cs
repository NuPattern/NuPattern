using System;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
	/// <summary>
	/// Customized shape for a Produced Asset.
	/// </summary>
	public partial class ProductionToolShape : ProductionToolShapeBase, IDisposable
	{
		private RightPointingArrowShapeGeometry geometry = new RightPointingArrowShapeGeometry();

		/// <summary>
		/// Finalizes an instance of the <see cref="ProductionToolShape"/> class.
		/// </summary>
		~ProductionToolShape()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// Returns the geometry for the shape.
		/// </summary>
		public override ShapeGeometry ShapeGeometry
		{
			get
			{
				return this.geometry;
			}
		}

		/// <summary>
		/// Gets the sides of the node shape which can be resized by the user.
		/// Default behavior is that all sides may be resized.
		/// </summary>
		public override NodeSides ResizableSides
		{
			get
			{
				return NodeSides.All;
			}
		}

		/// <summary>
		/// Constrain width of shape to maintain shape integrity.
		/// </summary>
		public override SizeD MinimumResizableSize
		{
			get
			{
				// Fix width/height constraints
				return (new SizeD(2.0d, 0.6d));
			}
		}

		/// <summary>
		/// Gets whether the shape has custom connection points.
		/// </summary>
		public override bool HasConnectionPoints
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Defines the connection points for the shape.
		/// </summary>
		public override void EnsureConnectionPoints(LinkShape link)
		{
			PointD[] connectionPoints = this.geometry.GetGeometryConnectionPoints(this);
			if (connectionPoints != null)
			{
				foreach (PointD connectionPoint in connectionPoints)
				{
					this.CreateConnectionPoint(connectionPoint);
				}
			}
			else
			{
				base.EnsureConnectionPoints(link);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes instances.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.geometry != null)
				{
					this.geometry.Dispose();
				}
			}
		}
	}
}