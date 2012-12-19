using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NuPattern.Common.Presentation.Data
{
	/// <summary>
	/// Converts an <see cref="ImageSource"/> into an <see cref="Image"/>.
	/// </summary>
	[ValueConversion(typeof(BitmapImage), typeof(Image))]
	public sealed class ImageConverter : IValueConverter
	{
		/// <summary>
		/// Converts an <see cref="ImageSource"/> into an <see cref="Image"/>.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var inputImage = value as ImageSource;
			if (inputImage != null)
			{
				var image = new Image();
				image.BeginInit();
				image.Source = inputImage;
				image.EndInit();
				return image;
			}

			return DependencyProperty.UnsetValue;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}