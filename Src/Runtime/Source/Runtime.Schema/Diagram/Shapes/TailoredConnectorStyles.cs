using System.Drawing;

namespace NuPattern.Runtime.Schema
{
	/// <summary>
	/// Defines the styles for tailoring connectors.
	/// </summary>
	internal class TailoredConnectorStyles : TailoredShapeStyles
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TailoredConnectorStyles"/> class.
		/// </summary>
		public TailoredConnectorStyles(Color text, Color outline)
			: base(Color.Empty, text, outline)
		{
		}
	}
}
