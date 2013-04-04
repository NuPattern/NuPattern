using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Diagrams.GraphObject;
using NuPattern.Modeling;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Customizations for the <see cref="ViewShape"/> class.
    /// </summary>
    partial class ViewShape
    {
        private const double ShapeMarginX = 0.25d;
        private const double ShapeMarginY = 0.25d;

        private static double LastShapeBottom = 0d;

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
                return new SizeD(1.3d, this.AbsoluteBounds.Height);
            }
        }

        /// <summary>
        /// Gets the tailoring shape styles for the shape.
        /// </summary>
        internal virtual ITailoredShapeElementStyles TailoringColors
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
        /// PortPlacement Determining Logic is encapsulated in this method.
        /// </summary>
        /// <param name="parentShapeDimensions">Only the width and height are used since the other parameter is relative.</param>
        /// <param name="childShapeRelativeBounds">RelativeBounds of child with respect to parent.</param>
        public override PortPlacement GetChildPortPlacement(RectangleD parentShapeDimensions, RectangleD childShapeRelativeBounds)
        {
            return PortPlacement.Bottom;
        }

        /// <summary>
        /// Initializes the resources for each instance of the shape.
        /// </summary>
        protected override void InitializeInstanceResources()
        {
            base.InitializeInstanceResources();

            // Set tailorable colors
            NamedElementSchema element = this.Subject as NamedElementSchema;
            if ((element != null) && (element.IsInheritedFromBase))
            {
                // Set tailoring styles for this instance.
                this.SetShapeBrushColor(DiagramBrushes.ShapeBackground, this.TailoringColors.FillColor);
                this.SetShapeBrushColor(DiagramBrushes.ShapeText, this.TailoringColors.TextColor);
                this.SetShapePenColor(DiagramPens.ShapeOutline, this.TailoringColors.OutlineColor);
            }
            else
            {
                // Reset to orginally configured styles for this instance.
                this.InitializeResources(this.StyleSet);
            }
        }

        /// <summary>
        /// Performs initial layout of view.
        /// </summary>
        public void PerformInitialLayout()
        {
            if (!this.IsLayoutInitialized)
            {
                this.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    Layout();
                    this.IsLayoutInitialized = true;
                }, Resources.ViewShape_TransactionPerformInitialLayout);
            }
        }

        /// <summary>
        /// Performs custom layout for the view.
        /// </summary>
        public void Layout()
        {
            var pattern = (this.Subject as ViewSchema).Pattern;

            this.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    //Ensure position of pattern
                    var productShape = pattern.GetShape<PatternShape>();
                    if (productShape != null)
                    {
                        productShape.MoveTo(PatternShape.InitialPosition);
                    }

                    // Layout descendant shapes
                    LastShapeBottom = 0;
                    LayoutShape(this);
                    this.Diagram.Invalidate(true);

                }, Resources.ViewShape_TransactionLayout);
        }

        private static void LayoutShape(NodeShape parentShape)
        {
            Guard.NotNull(() => parentShape, parentShape);

            LastShapeBottom = parentShape.AbsoluteBounds.Bottom;

            // Get (linked) children shapes by y-pos
            var childrenShapes = GetChildrenShapes(parentShape);
            foreach (var childShape in childrenShapes)
            {
                if (childShape.CanMove)
                {
                    childShape.MoveTo(parentShape.AbsoluteBounds.Right + ShapeMarginX, LastShapeBottom + ShapeMarginY);

                    // Ensure connector points
                    var connector = GetConnectorToParent(childShape);
                    if (connector != null &&
                        connector.CanMoveAnchorPoints)
                    {
                        connector.FromEndPoint = new PointD(parentShape.AbsoluteBounds.Center.X, parentShape.AbsoluteBounds.Bottom);
                        connector.FixedFrom = VGFixedCode.Caller;
                        connector.ToEndPoint = new PointD(childShape.AbsoluteBounds.Left, childShape.AbsoluteBounds.Center.Y);
                        connector.FixedTo = VGFixedCode.Caller;
                    }
                }

                LayoutShape(childShape);
            }
        }

        private static IOrderedEnumerable<NodeShape> GetChildrenShapes(NodeShape shape)
        {
            var childrenElements = GetAllEmbeddedElements(shape.Subject);
            return childrenElements
                .Select(e => e.GetShape<NodeShape>())
                .Where(s => s != null)
                .OrderBy(s => s.AbsoluteBoundingBox.Top);
        }
        private static BinaryLinkShape GetConnectorToParent(NodeShape shape)
        {
            return shape.ToRoleLinkShapes.Cast<BinaryLinkShape>().FirstOrDefault();
        }
        private static HashSet<ModelElement> GetAllEmbeddedElements(ModelElement element)
        {
            HashSet<ModelElement> embeddedElements = new HashSet<ModelElement>();
            HashSet<DomainRoleInfo> embeddingRoles = GetAllEmbeddingRoles(element.GetDomainClass());

            foreach (DomainRoleInfo roleInfo in embeddingRoles)
            {
                // The role could have a multiplicity of 1 or many - which is it?  
                if (roleInfo.IsMany)
                {
                    // Fetch and add each of the elements to the set  
                    roleInfo.GetLinkedElements(element).ForEach(e => { embeddedElements.Add(e); });
                }
                else
                {
                    // Add the single element to the set  
                    ModelElement linkedElement = roleInfo.GetLinkedElement(element);
                    if (linkedElement != null)
                    {
                        embeddedElements.Add(linkedElement);
                    }
                }
            }

            return embeddedElements;
        }
        private static HashSet<DomainRoleInfo> GetAllEmbeddingRoles(DomainClassInfo classInfo)
        {
            HashSet<DomainRoleInfo> embeddedItemRoles = new HashSet<DomainRoleInfo>();
            ReadOnlyCollection<DomainRoleInfo> rolesPlayed = classInfo.AllDomainRolesPlayed;
            foreach (DomainRoleInfo roleInfo in rolesPlayed)
            {
                if (roleInfo.IsEmbedding)
                {
                    embeddedItemRoles.Add(roleInfo);
                }
            }
            return embeddedItemRoles;
        }
    }
}