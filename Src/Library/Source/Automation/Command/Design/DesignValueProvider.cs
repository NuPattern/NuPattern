using System.ComponentModel;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <summary>
	/// Defines a value provider wrapper for the property grid.
	/// </summary>
	[TypeDescriptionProvider(typeof(DesignValueProviderTypeDescriptionProvider))]
	public class DesignValueProvider
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DesignValueProvider"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="name">The name of the property.</param>
		public DesignValueProvider(DesignProperty property, string name)
		{
			this.DesignProperty = property;

			this.Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DesignValueProvider"/> class.
		/// </summary>
		/// <param name="property">The property.</param>
		/// <param name="valueProvider">The value provider.</param>
		public DesignValueProvider(DesignProperty property, ValueProviderSettings valueProvider)
			: this(property, valueProvider != null ? valueProvider.TypeId : string.Empty)
		{
			this.ValueProvider = valueProvider;
		}

		internal DesignProperty DesignProperty { get; private set; }

		internal ValueProviderSettings ValueProvider { get; set; }

		internal string Name { get; private set; }
	}
}