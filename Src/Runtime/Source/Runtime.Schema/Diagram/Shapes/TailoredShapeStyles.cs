using System.Drawing;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Defines the styles for tailoring shapes.
	/// </summary>
	internal class TailoredShapeStyles : ITailoredShapeElementStyles
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TailoredShapeStyles"/> class.
		/// </summary>
		public TailoredShapeStyles(Color fill, Color text, Color outline)
		{
			this.FillColor = fill;
			this.TextColor = text;
			this.OutlineColor = outline;
		}

		/// <summary>
		/// Gets the color of the tailorable fill.
		/// </summary>
		public Color FillColor { get; private set; }

		/// <summary>
		/// Gets the color of the tailorable text.
		/// </summary>
		public Color TextColor { get; private set; }

		/// <summary>
		/// Gets the color of the tailorable outline.
		/// </summary>
		public Color OutlineColor { get; private set; }
	}
}
