using System.Drawing;

namespace NuPattern.Authoring.WorkflowDesign
{
	/// <summary>
	/// Defines the styles for tailored backgrounds.
	/// </summary>
	internal class TailoredBackgroundStyles : ITailoredBackgroundStyles
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TailoredBackgroundStyles"/> class.
		/// </summary>
		public TailoredBackgroundStyles(Color background, Color titleGradient, Color titleText)
		{
			this.BackgroundFillColor = background;
			this.TitleGradientFillColor = titleGradient;
			this.TitleTextColor = titleText;
		}

		/// <summary>
		/// Gets the color of the background fill.
		/// </summary>
		/// <value></value>
		public Color BackgroundFillColor { get; private set; }

		/// <summary>
		/// Gets the color of the title gradient fill color.
		/// </summary>
		/// <value></value>
		public Color TitleGradientFillColor { get; private set; }

		/// <summary>
		/// Gets the color of the title text color.
		/// </summary>
		/// <value></value>
		public Color TitleTextColor { get; private set; }
	}
}
