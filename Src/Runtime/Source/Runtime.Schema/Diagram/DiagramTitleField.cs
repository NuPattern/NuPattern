using System.Drawing;
using Microsoft.VisualStudio.Modeling.Diagrams;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// A field for the title of the diagram.
	/// </summary>
	internal class DiagramTitleField : TextField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DiagramTitleField"/> class.
		/// </summary>
		/// <param name="fieldName">Unique identifier for this ShapeField.</param>
		private DiagramTitleField(string fieldName)
			: base(fieldName)
		{
		}

		/// <summary>
		/// Initializes the resources for all fields of this type.
		/// </summary>
		public static void InitializeInstanceResources(StyleSet classStyleSet, float fontSize, Color fontColor)
		{
			FontSettings fontSettings = new FontSettings();
			fontSettings.Style = FontStyle.Bold;
			fontSettings.Size = fontSize;
			classStyleSet.OverrideFont(DiagramFonts.ShapeTitle, fontSettings);
			BrushSettings brushSettings = new BrushSettings();
			brushSettings.Color = fontColor;
			classStyleSet.OverrideBrush(DiagramBrushes.ShapeText, brushSettings);
		}

		/// <summary>
		/// Creates and returns a new configured instance of this shape.
		/// </summary>
		public static DiagramTitleField CreateDiagramTitleField(string fieldName)
		{
			DiagramTitleField textField = new DiagramTitleField(fieldName);
			textField.DefaultText = string.Empty;
			textField.DefaultVisibility = true;
			textField.DefaultAutoSize = true;
			textField.DefaultFontId = DiagramFonts.ShapeTitle;
			textField.DefaultFocusable = true;
			textField.AnchoringBehavior.MinimumHeightInLines = 1;
			textField.AnchoringBehavior.MinimumWidthInCharacters = 1;
			textField.DefaultAccessibleState = System.Windows.Forms.AccessibleStates.Invisible;

			return textField;
		}
	}
}
