using System.Drawing;

namespace Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign
{
	/// <summary>
	/// Defines styles for tailored backgrounds.
	/// </summary>
	internal interface ITailoredBackgroundStyles
	{
		/// <summary>
		/// Gets the color of the background fill.
		/// </summary>
		Color BackgroundFillColor
		{
			get;
		}

		/// <summary>
		/// Gets the color of the title gradient fill color.
		/// </summary>
		Color TitleGradientFillColor
		{
			get;
		}

		/// <summary>
		/// Gets the color of the title text color.
		/// </summary>
		Color TitleTextColor
		{
			get;
		}
	}
}
