using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Defines an <see cref="ICondition"/> implementation of <see cref="BindingSettings"/>.
    /// </summary>
    [DisplayNameResource("ConditionBindingSettings_DisplayName", typeof(Resources))]
    [DescriptionResource("ConditionBindingSettings_Description", typeof(Resources))]
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
                return Resources.ConditionBindingSettings_EmptyBinding;
            }

            return this.TypeId.Split('.').Last();
        }
    }
}