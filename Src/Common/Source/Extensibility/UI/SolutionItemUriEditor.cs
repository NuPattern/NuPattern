using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Representes the editor to pick a solution item and return its Uri.
	/// </summary>
	[CLSCompliant(false)]
	public class SolutionItemUriEditor : SolutionItemEditor
	{
		/// <summary>
		/// Converts the value to the destination type.
		/// </summary>
		protected override object ConvertValue(ITypeDescriptorContext context, IServiceProvider provider, IItemContainer value)
		{
			if (value != null)
			{
				var uriService = provider.GetService<IFxrUriReferenceService>();
				return uriService.CreateUri(value, "solution");
			}

			return null;
		}
	}
}