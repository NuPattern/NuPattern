using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	public partial interface IPropertySchema
	{
		/// <summary>
		/// Gets the validation settings.
		/// </summary>
		IEnumerable<IBindingSettings> ValidationSettings { get; set; }

		/// <summary>
		/// Gets the default value settings. This property never returns <see langword="null"/>.
		/// </summary>
		IPropertyBindingSettings DefaultValue { get; }

		/// <summary>
		/// Gets the value provider settings. This property never returns <see langword="null"/>.
		/// </summary>
		IValueProviderBindingSettings ValueProvider { get; }
	}
}
