using System.Drawing;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
	/// <summary>
	/// Defines styles for tailored shapes and connectors.
	/// </summary>
	internal interface ITailoredShapeElementStyles
	{
		/// <summary>
		/// Gets the color of the tailorable fill.
		/// </summary>
		Color FillColor
		{
			get;
		}

		/// <summary>
		/// Gets the color of the tailorable text.
		/// </summary>
		Color TextColor
		{
			get;
		}

		/// <summary>
		/// Gets the color of the tailorable outline.
		/// </summary>
		Color OutlineColor
		{
			get;
		}
	}
}
