using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    [ValueConversion(typeof(Enum), typeof(string))]
    internal class EnumToDescriptionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                var description = value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>();
                return description != null ? description.Description : value.ToString();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}