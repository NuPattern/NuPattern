using System.Drawing;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace NuPattern.Authoring.WorkflowDesign
{
    /// <summary>
    /// Diagram extension methods.
    /// </summary>
    internal static class DiagramExtensions
    {
        /// <summary>
        /// Sets the given brush with the new given color.
        /// </summary>
        internal static void SetShapeBrushColor(this ShapeElement shape, StyleSetResourceId resourceId, Color color)
        {
            BrushSettings brushSettings = shape.StyleSet.GetOverriddenBrushSettings(resourceId);
            if (brushSettings == null)
            {
                brushSettings = new BrushSettings();
            }

            brushSettings.Color = color;
            shape.StyleSet.OverrideBrush(resourceId, brushSettings);
            shape.Invalidate();
        }

        /// <summary>
        /// Sets the given pen with the new given color.
        /// </summary>
        internal static void SetShapePenColor(this ShapeElement shape, StyleSetResourceId resourceId, Color color)
        {
            PenSettings penSettings = shape.StyleSet.GetOverriddenPenSettings(resourceId);
            if (penSettings == null)
            {
                penSettings = new PenSettings();
            }

            penSettings.Color = color;
            shape.StyleSet.OverridePen(resourceId, penSettings);
            shape.Invalidate();
        }
    }
}