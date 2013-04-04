using System.ComponentModel;
using System.Globalization;

namespace NuPattern.Runtime.UnitTests.ToolkitInterface
{
    public class TrueConverter : System.ComponentModel.StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return true;
        }
    }
}
