using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using NuPattern.Extensibility.Properties;
using NuPattern.Runtime;

namespace NuPattern.Extensibility.Binding
{
	/// <summary>
	/// <see cref="IValidationRule"/> implementation of <see cref="IBindingSettings"/>.
	/// </summary>
	[DataContract]
	[TypeDescriptionProvider(typeof(BindingSettingsTypeDescriptionProvider<IValidationRule>))]
	public class ValidationBindingSettings : BindingSettings
	{
		/// <summary>
		/// Gets or sets the identifier for the runtime implementation type of the binding.
		/// </summary>
		[DataMember]
		[TypeConverter(typeof(FeatureComponentTypeConverter<IValidationRule>))]
		public override string TypeId
		{
			get { return base.TypeId; }
			set { base.TypeId = value; }
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
				return Resources.ValidationBindingSettings_ElementName;
			}

			return this.TypeId.Split('.').Last();
		}
	}
}