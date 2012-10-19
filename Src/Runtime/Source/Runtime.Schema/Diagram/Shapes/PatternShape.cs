using System;
using System.Collections;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Customizations for the <see cref=" PatternShapeBase"/> class.
    /// </summary>
    public partial class PatternShapeBase
    {
        /// <summary>
        /// Returns the list of elements valid at this time.
        /// </summary>
        private static IList FilterElementsFromPatternSchemaForLaunchPoints(LinkedElementCollection<AutomationSettingsSchema> result)
        {
            return ShapeExtensions.FilterElementsFromCompartment<AutomationSettingsSchema, string>(result,
                item => (item.Classification == Runtime.AutomationSettingsClassification.LaunchPoint),
                item => string.Concat(item.AutomationType, item.Name));
        }
        private static IList FilterElementsFromPatternSchemaForAutomation(LinkedElementCollection<AutomationSettingsSchema> result)
        {
            return ShapeExtensions.FilterElementsFromCompartment<AutomationSettingsSchema, string>(result,
                item => (item.Classification == Runtime.AutomationSettingsClassification.General),
                item => string.Concat(item.AutomationType, item.Name));
        }
        private static IList FilterElementsFromPatternSchemaForProperties(LinkedElementCollection<PropertySchema> result)
        {
            return ShapeExtensions.FilterElementsFromCompartment<PropertySchema, string>(result,
                (item => true), item => item.Name);
        }
    }

    /// <summary>
    /// Customizations for the <see cref=" PatternShape"/> class.
    /// </summary>
    public partial class PatternShape : PatternShapeBase, IDisposable
    {
        /// <summary>
        /// The initial position of the shape.
        /// </summary>
        public static readonly PointD InitialPosition = new PointD(0.25d, 0.50d);

        private SquaredBottomShapeGeometry geometry = new SquaredBottomShapeGeometry();

        /// <summary>
        /// Finalizes an instance of the <see cref="PatternShape"/> class.
        /// </summary>
        ~PatternShape()
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
                return NodeSides.Right;
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
                geometryMinSize.Width += 1.3d;

                return (geometryMinSize);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user is allowed to reposition the ShapeElement.
        /// </summary>
        public override bool CanMove
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the shape has a drop shadow.
        /// </summary>
        public override bool HasShadow
        {
            get
            {
                return false;
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
        /// <returns>The CompartmentMappings for all compartments displayed on the shape.</returns>
        protected override CompartmentMapping[] GetCompartmentMappings(Type melType)
        {
            Guard.NotNull(() => melType, melType);

            return this.DecorateInheritedCompartmentNamedElements(base.GetCompartmentMappings(melType));
        }
    }
}
