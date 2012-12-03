using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Microsoft.VisualStudio.Patterning.Common.Presentation.Data
{
	/// <summary>
	/// Converts an image <see cref="BitmapSource"/> into a grayscale version of the same image as an <see cref="Image"/>.
	/// </summary>
	[ValueConversion(typeof(BitmapImage), typeof(Image))]
	public sealed class GrayscaleImageConverter : PlatformUI.GrayscaleImageConverter
    {	}
}