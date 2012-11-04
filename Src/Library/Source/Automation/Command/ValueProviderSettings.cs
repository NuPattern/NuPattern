using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Library.Automation
{
	/// <content>
	/// Adds the implementation of <see cref="IValueProviderBindingSettings"/>.
	/// </content>
	public partial class ValueProviderSettings : IValueProviderBindingSettings
	{
		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

		/// <summary>
		/// Gets the optional property bindings.
		/// </summary>
		IList<IPropertyBindingSettings> IBindingSettings.Properties
		{
			get
			{
				return new ReadOnlyCollection<IPropertyBindingSettings>(this.Properties
					.Cast<IPropertyBindingSettings>()
					.ToList());
			}
		}

		private PropertyChangeManager propertyChanges;

		/// <summary>
		/// Gets the manager for property change events.
		/// </summary>
		protected PropertyChangeManager PropertyChanges
		{
			get
			{
				if (this.propertyChanges == null)
				{
					this.propertyChanges = new PropertyChangeManager(this);
				}

				return this.propertyChanges;
			}
		}
	}
}
