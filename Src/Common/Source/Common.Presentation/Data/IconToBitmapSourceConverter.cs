using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NuPattern.Common.Presentation.Data
{
	/// <summary>
	/// Provides a converter to convert an <see cref="Icon"/> to a <see cref="BitmapSource"/>.
	/// </summary>
	[ValueConversion(typeof(Icon), typeof(BitmapSource))]
	public sealed class IconToBitmapSourceConverter : IValueConverter
	{
		/// <summary>
		/// Converts an <see cref="Icon"/> to a <see cref="BitmapSource"/>.
		/// </summary>
		/// <param name="value">The icon to convert.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The parameter (Not used).</param>
		/// <param name="culture">The culture.</param>
		/// <returns>The icon converted to bitmap source.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var icon = value as Icon;
			if (icon != null)
			{
				var image = Imaging.CreateBitmapSourceFromHBitmap(
					icon.ToBitmap().GetHbitmap(),
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions());
				image.Freeze();
				return image;
			}

			return DependencyProperty.UnsetValue;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}