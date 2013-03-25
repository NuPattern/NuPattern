using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.Modeling.Diagrams;
using NuPattern.Authoring.WorkflowDesign.Properties;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// Class that represents the main diagram.
    /// </summary>
    partial class WorkflowDesignDiagram
    {
        internal const string DiagramTitleShapeName = "Title";
        internal const string DiagramBackgroundShapeName = "Background";
        private const float TitleFontSize = 14 / 72.0F;
        private const double GradientHeight = 1.0d;
        private static readonly PointD titleLocation = new PointD(0.33, 0.50);

        private IList<ShapeField> fields;

        /// <summary>
        /// Gets whether the background has a gradient.
        /// </summary>
        public override bool HasBackgroundGradient
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the margin between this Diagram's bounding box
        /// perimeter and its nested node shapes.  The margin is in
        /// world units.
        /// This margin is to provide a region along the perimeter
        /// of this Diagram where connection lines can route through.
        /// </summary>
        public override SizeD NestedShapesMargin
        {
            get { return new SizeD(0.1, 0.1); }
        }

        /// <summary>
        /// Per-class ShapeFields for this shape.
        /// </summary>
        public override IList<ShapeField> ShapeFields
        {
            get
            {
                if (this.fields == null)
                {
                    this.fields = CreateShapeFields();
                }

                return this.fields;
            }
        }

        /// <summary>
        /// Returns the background styles for the diagram.
        /// </summary>
        internal virtual ITailoredBackgroundStyles GetBackgroundStyles()
        {
            //if (this.Store.GetRootElement().IsInTailorMode())
            //{
            //    return new TailoredBackgroundStyles(
            //        this.TailoringBackgroundColor,
            //        this.TailoringGradientColor,
            //        this.TailoringTitleTextColor);
            //}
            //else
            //{
            return new TailoredBackgroundStyles(
                this.AuthoringBackgroundColor,
                this.AuthoringGradientColor,
                this.AuthoringTitleTextColor);
            //}
        }

        /// <summary>
        /// Initialize the collection of shape fields associated with this shape type.
        /// </summary>
        protected override void InitializeShapeFields(IList<ShapeField> shapeFields)
        {
            Guard.NotNull(() => shapeFields, shapeFields);

            base.InitializeShapeFields(shapeFields);

            shapeFields.Add(this.CreateDiagramTitleField());
        }

        /// <summary>
        /// Creates the frame label.
        /// </summary>
        protected virtual TextField CreateDiagramTitleField()
        {
            var field = DiagramTitleField.CreateDiagramTitleField(DiagramTitleShapeName);
            field.DefaultText = Resources.DiagramTitleFieldText_WorkflowView;

            return field;
        }

        /// <summary>
        /// Initialize the collection of decorators associated with this shape type.  This method also
        /// creates shape fields for outer decorators, because these are not part of the shape fields collection
        /// associated with the shape, so they must be created here rather than in InitializeShapeFields.
        /// </summary>
        protected override void InitializeDecorators(IList<ShapeField> shapeFields, IList<Decorator> decorators)
        {
            Guard.NotNull(() => shapeFields, shapeFields);
            Guard.NotNull(() => decorators, decorators);

            base.InitializeDecorators(shapeFields, decorators);

            var frameField = ShapeElement.FindShapeField(shapeFields, DiagramTitleShapeName);
            decorators.Add(new ShapeDecorator(frameField, ShapeDecoratorPosition.InnerTopLeft, titleLocation));
        }

        /// <summary>
        /// Initializes the resources for this instance of the diagram.
        /// </summary>
        protected override void InitializeInstanceResources()
        {
            base.InitializeInstanceResources();

            this.InitializeBackgroundResources(this.ClassStyleSet);
        }

        /// <summary>
        /// Initializes the Resources for the background.
        /// </summary>
        private void InitializeBackgroundResources(StyleSet styleSet)
        {
            ITailoredBackgroundStyles styles = this.GetBackgroundStyles();

            Color gradientStartColor = styles.TitleGradientFillColor;
            Color gradientEndColor = Color.Transparent;
            Color selectedColor = gradientStartColor;
            Color selectedInnactiveColor = gradientStartColor;

            // Title Text
            DiagramTitleField.InitializeInstanceResources(styleSet, TitleFontSize, styles.TitleTextColor);

            // Background color
            BrushSettings backgroundBrush = new BrushSettings();
            backgroundBrush.Color = styles.BackgroundFillColor;
            styleSet.OverrideBrush(DiagramBrushes.DiagramBackground, backgroundBrush);

            // Start of title gradient
            BrushSettings titleGradientBrush = new BrushSettings();
            titleGradientBrush.Color = gradientStartColor;
            styleSet.OverrideBrush(DiagramBrushes.ShapeBackground, titleGradientBrush);

            // Selected state
            titleGradientBrush = new BrushSettings();
            titleGradientBrush.Color = selectedColor;
            styleSet.OverrideBrush(DiagramBrushes.ShapeBackgroundSelected, titleGradientBrush);

            // SelectedInactive state
            titleGradientBrush = new BrushSettings();
            titleGradientBrush.Color = selectedInnactiveColor;
            styleSet.OverrideBrush(DiagramBrushes.ShapeBackgroundSelectedInactive, titleGradientBrush);

            // Find the field for the background
            AreaField background = this.FindShapeField(DiagramBackgroundShapeName) as AreaField;
            if (background != null)
            {
                background.DefaultReflectParentSelectedState = true;
                background.GradientEndingColor = gradientEndColor;

                // Constrain the height of the shape
                background.AnchoringBehavior.SetBottomAnchor(AnchoringBehavior.Edge.Bottom, (this.MaximumSize.Height - GradientHeight));
            }
        }
    }
}
