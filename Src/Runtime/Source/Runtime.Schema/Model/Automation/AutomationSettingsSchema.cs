using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime.Schema.Properties;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema
{
    /// <summary>
    /// Customizations for the <see cref="AutomationSettingsSchema"/> class.
    /// </summary>
    [TypeDescriptionProvider(typeof(AutomationSettingsTypeDescriptorProvider))]
    public partial class AutomationSettingsSchema
    {
        /// <summary>
        /// Tries to convert this settings element to the given typed automation 
        /// extension setting class.
        /// </summary>
        /// <typeparam name="TSettings">The type of the setting.</typeparam>
        public TSettings As<TSettings>() where TSettings : IAutomationSettings
        {
            return this.GetExtensions<TSettings>().SingleOrDefault();
        }

        /// <summary>
        /// Returns the value of the AutomationTypeId property.
        /// </summary>
        internal string GetAutomationTypeIdValue()
        {
            // TODO: add code here to return the selected automation type id for the current automation extension.
            return string.Empty;
        }

        /// <summary>
        /// Returns the displayed text of the automation
        /// </summary>
        /// <returns></returns>
        internal string GetDisplayText()
        {
            return string.Format(CultureInfo.CurrentCulture, Resources.AutomationSettingsSchema_DisplayTextFormat, this.Name, this.AutomationType);
        }
    }
}
