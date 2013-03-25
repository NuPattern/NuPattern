using System.ComponentModel;
using System.Globalization;

namespace NuPattern.Extensibility.UnitTests.ToolkitInterfaceLayer
{
    public class TrueConverter : System.ComponentModel.StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return true;
        }
    }
}
