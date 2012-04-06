using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	/// <summary>
	/// Defines a value provider wrapper for the property grid.
	/// </summary>
	[TypeDescriptionProvider(typeof(DesignValueProviderTypeDescriptionProvider))]
	internal class DesignValueProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DesignValueProvider"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="valueProvider">The value provider.</param>
		public DesignValueProvider(DesignProperty property, IValueProviderBindingSettings valueProvider)
		{
			this.DesignProperty = property;
			this.ValueProvider = valueProvider;
			this.Name = valueProvider.TypeId;
		}

		internal DesignProperty DesignProperty { get; private set; }

		internal IValueProviderBindingSettings ValueProvider { get; set; }

		internal string Name { get; private set; }
	}
}