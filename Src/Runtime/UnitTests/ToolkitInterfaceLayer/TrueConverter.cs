using System.ComponentModel;
using System.Globalization;

namespace Toolkit14
{
	public class TrueConverter : StringConverter
	{
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return true;
		}
	}
}
