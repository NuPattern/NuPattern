using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using NuPattern.Reflection;

namespace NuPattern.Runtime.Guidance.UI
{
    [ValueConversion(typeof(Enum), typeof(string))]
    internal class EnumToDescriptionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum)
            {
                var description = value.GetType().GetField(value.ToString()).GetCustomAttribute<DescriptionAttribute>(true);
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