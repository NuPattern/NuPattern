using System;
using System.Collections;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Customizations for the <see cref=" CollectionShapeBase"/> class.
	/// </summary>
	public partial class CollectionShapeBase
	{
		/// <summary>
		/// Returns the list of elements valid at this time.
		/// </summary>
		private static IList FilterElementsFromCollectionSchemaForLaunchPoints(LinkedElementCollection<AutomationSettingsSchema> result)
		{
			return ShapeExtensions.FilterElementsFromCompartment<AutomationSettingsSchema, string>(result,
				item => (item.Classification == Runtime.AutomationSettingsClassification.LaunchPoint),
				item => string.Concat(item.AutomationType, item.Name));
		}

		private static IList FilterElementsFromCollectionSchemaForAutomation(LinkedElementCollection<AutomationSettingsSchema> result)
		{
			return ShapeExtensions.FilterElementsFromCompartment<AutomationSettingsSchema, string>(result,
				item => (item.Classification == Runtime.AutomationSettingsClassification.General),
				item => string.Concat(item.AutomationType, item.Name));
		}

        private static IList FilterElementsFromCollectionSchemaForProperties(LinkedElementCollection<PropertySchema> result)
        {
            return ShapeExtensions.FilterElementsFromCompartment<PropertySchema, string>(result,
                (item => true), item => item.Name);
        }
	}

	/// <summary>
	/// Customizations for the <see cref=" CollectionShape"/> class.
	/// </summary>
	public partial class CollectionShape : CollectionShapeBase, IDisposable
	{
		private FolderShapeGeometry geometry = new FolderShapeGeometry();

		/// <summary>
		/// Finalizes an instance of the <see cref="CollectionShape"/> class.
		/// </summary>
		~CollectionShape()
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
				return NodeSides.Right | NodeSides.Left;
			}
		}

		/// <summary>
		/// Constrain width of shape to maintain shape integrity.
		/// </summary>
		public override SizeD MinimumResizableSize
		{
			get
			{
				// Add additonal width constraint to the basic shape geometry constraints
				SizeD geometryMinSize = this.geometry.MinimumGeometrySize;
				geometryMinSize.Width += 0.5d;

				return (geometryMinSize);
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
		/// Gets the tailoring color styles.
		/// </summary>
		internal override ITailoredShapeElementStyles TailoringColors
		{
			get
			{
				return new TailoredShapeStyles(
					this.TailoringFillColor,
					this.TailoringTextColor,
					this.TailoringOutlineColor);
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

		/// <summary>
		/// Gets an array of CompartmentMappings for all compartments displayed on this shape
		/// (including compartment maps defined on base shapes).
		/// </summary>
		/// <param name="melType">The type of the DomainClass that this shape is mapped to.</param>
		/// <returns>The CompartmentMappings for all comparments displayed on <paramref name="melType"/>.</returns>
		protected override CompartmentMapping[] GetCompartmentMappings(Type melType)
		{
			Guard.NotNull(() => melType, melType);

			return this.DecorateInheritedCompartmentNamedElements(base.GetCompartmentMappings(melType));
		}
	}
}