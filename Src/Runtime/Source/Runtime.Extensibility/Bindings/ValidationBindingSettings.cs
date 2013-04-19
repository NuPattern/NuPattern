using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using NuPattern.ComponentModel.Design;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.Runtime.Design;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.Validation;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// <see cref="IValidationRule"/> implementation of <see cref="IBindingSettings"/>.
    /// </summary>
    [DisplayNameResource("ValidationBindingSettings_DisplayName", typeof(Resources))]
    [DescriptionResource("ValidationBindingSettings_Description", typeof(Resources))]
    [DataContract]
    [TypeDescriptionProvider(typeof(BindingSettingsTypeDescriptionProvider<IValidationRule>))]
    public class ValidationBindingSettings : BindingSettings
    {
        /// <summary>
        /// Gets or sets the identifier for the runtime implementation type of the binding.
        /// </summary>
        [DataMember]
        [TypeConverter(typeof(NuPatternComponentTypeConverter<IValidationRule>))]
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
                return Resources.ValidationBindingSettings_EmptyBinding;
            }

            return this.TypeId.Split('.').Last();
        }
    }
}