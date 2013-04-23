using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    //[TypeDescriptionProvider(typeof(CommandSettingsTypeDescriptionProvider))]
    partial class WizardSettings
    {
        /// <summary>
        /// Creates the runtime automation element for this setting element.
        /// </summary>
        public IAutomationExtension CreateAutomation(IProductElement owner)
        {
            return new WizardAutomation(owner, this);
        }

        /// <summary>
        /// Gets the classification of these settings.
        /// </summary>
        public AutomationSettingsClassification Classification
        {
            get
            {
                return AutomationSettingsClassification.General;
            }
        }
    }
}