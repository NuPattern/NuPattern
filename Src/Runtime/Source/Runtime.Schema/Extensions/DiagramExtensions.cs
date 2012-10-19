using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Diagram extension methods.
	/// </summary>
	internal static class DiagramExtensions
	{
		/// <summary>
		/// Determines whether [is view represented] [the specified diagram].
		/// </summary>
		/// <param name="diagram">The diagram.</param>
		/// <param name="view">The view to verify.</param>
		/// <returns>
		/// 	<c>true</c> if [is view represented] [the specified diagram]; otherwise, <c>false</c>.
		/// </returns>
		internal static bool IsViewRepresented(this PatternModelSchemaDiagram diagram, ViewSchema view)
		{
			return diagram.Id.ToString().Equals(view.DiagramId, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Gets the represented view.
		/// </summary>
		/// <param name="diagram">The diagram.</param>
		/// <returns>The view represented by the diagram.</returns>
        internal static ViewSchema GetRepresentedView(this PatternModelSchemaDiagram diagram)
		{
			return diagram.Store.GetViews().Single(view => diagram.IsViewRepresented(view));
		}

		/// <summary>
		/// Gets the shapes.
		/// </summary>
		/// <param name="diagram">The diagram.</param>
        internal static IEnumerable<TShape> GetShapes<TShape>(this PatternModelSchemaDiagram diagram) where TShape : ShapeElement
		{
			return diagram.Store.ElementDirectory.AllElements.OfType<TShape>()
				.Where(s => s.Diagram.Id.Equals(diagram.Id));
		}

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