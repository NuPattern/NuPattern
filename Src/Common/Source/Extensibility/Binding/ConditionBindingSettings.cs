using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.Patterning.Extensibility.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Binding
{
	/// <summary>
	/// Defines an <see cref="ICondition"/> implementation of <see cref="BindingSettings"/>.
	/// </summary>
	[DataContract]
	[TypeDescriptionProvider(typeof(BindingSettingsTypeDescriptionProvider<ICondition>))]
	public class ConditionBindingSettings : BindingSettings
	{
		/// <summary>
		/// Gets or sets the identifier for the runtime implementation type of the binding.
		/// </summary>
		[TypeConverter(typeof(FeatureComponentTypeConverter<ICondition>))]
		[DataMember]
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
				return Resources.ConditionBindingSettings_ElementName;
			}

			return this.TypeId.Split('.').Last();
		}
	}
}