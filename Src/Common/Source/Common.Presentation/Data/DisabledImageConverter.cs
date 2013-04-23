using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.PlatformUI;

namespace NuPattern.Presentation.Data
{
#if VSVER10
    /// <summary>
    /// Converts an image <see cref="BitmapSource"/> into a disabled version of the same image as an <see cref="Image"/>.
    /// </summary>
    [ValueConversion(typeof(BitmapImage), typeof(Image))]
    public sealed class DisabledImageConverter : Microsoft.VisualStudio.PlatformUI.GrayscaleImageConverter
    {	}
#endif
#if VSVER11
    /// <summary>
    /// Converts an image <see cref="BitmapSource"/> into a disabled version of the same image as an <see cref="Image"/>.
    /// </summary>
    [ValueConversion(typeof(BitmapImage), typeof(Image))]
    public sealed class DisabledImageConverter : ValueConverter<BitmapSource, Image>
    {
        private string BiasColor = @"#40FFFFFF";
        private ThemedImageConverter innerConverter;

        /// <summary>
        /// Creates a new instance of the <see cref="DisabledImageConverter"/> class.
        /// </summary>
        public DisabledImageConverter()
        {
            this.innerConverter = new ThemedImageConverter();
        }

        /// <summary>
        /// Converts the image to a disabled image
        /// </summary>
        /// <remarks>This is based on <see cref="ThemedImageConverter"/></remarks>
        protected override Image Convert(BitmapSource value, object parameter, System.Globalization.CultureInfo culture)
        {
            var backgroundColor = Colors.Transparent; // Background color of all commandbar images

            if (parameter == null)
            {
                parameter = (Color)ColorConverter.ConvertFromString(BiasColor);
            }

            if (value == null)
            {
                return null;
            }
            BitmapSource source;
            Color grayscaleBiasColor = Colors.White;
            if (parameter is Color)
            {
                grayscaleBiasColor = (Color)parameter;
            }
            source = ImageThemingUtilities.GetOrCreateThemedBitmapSource(value, backgroundColor, false, grayscaleBiasColor);

            Image image = new Image();
            image.BeginInit();
            image.Source = source;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// Reverts the converted image
        /// </summary>
        protected override BitmapSource ConvertBack(Image value, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#endif
}