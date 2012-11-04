using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.Patterning.Extensibility.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	/// <summary>
	/// Default implementation of the <see cref="IValueProviderBindingSettings"/>.
	/// </summary>
	[DataContract]
	[TypeDescriptionProvider(typeof(BindingSettingsTypeDescriptionProvider<IValueProvider>))]
	public class ValueProviderBindingSettings : BindingSettings, IValueProviderBindingSettings
	{
		/// <summary>
		/// Gets or sets the identifier for the runtime implementation type of the binding.
		/// </summary>
		[DataMember]
		[TypeConverter(typeof(FeatureComponentTypeConverter<IValueProvider>))]
		public override string TypeId
		{
			get { return base.TypeId; }
			set { base.TypeId = value; this.NotifyChanged(this.PropertyChangedHandler, x => x.TypeId); }
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.TypeId))
			{
				return Resources.ValueProviderBindingSettings_ElementName;
			}

			return this.TypeId.Split('.').Last();
		}
	}
}