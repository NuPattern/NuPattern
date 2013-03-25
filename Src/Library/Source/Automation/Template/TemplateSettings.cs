using System.ComponentModel;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    [TypeDescriptionProvider(typeof(TemplateSettingsDescriptionProvider))]
    partial class TemplateSettings
    {
        private BoundProperty targetFileNameProperty;
        private BoundProperty targetPathProperty;

        /// <summary>
        /// Creates the runtime automation element for this setting element.
        /// </summary>
        public IAutomationExtension CreateAutomation(IProductElement owner)
        {
            return new TemplateAutomation(owner, this);
        }

        /// <summary>
        /// Gets the classification of these settings.
        /// </summary>
        public AutomationSettingsClassification Classification
        {
            get { return AutomationSettingsClassification.LaunchPoint; }
        }

        /// <summary>
        /// Gets or sets the target filename settings.
        /// </summary>
        public IPropertyBindingSettings TargetFileName
        {
            get { return this.TargetFileNameProperty.Settings; }
            set { this.TargetFileNameProperty.Settings = value; }
        }

        /// <summary>
        /// Gets or sets the target path settings.
        /// </summary>
        public IPropertyBindingSettings TargetPath
        {
            get { return this.TargetPathProperty.Settings; }
            set { this.TargetPathProperty.Settings = value; }
        }

        private BoundProperty TargetFileNameProperty
        {
            get
            {
                return this.targetFileNameProperty ??
                    (this.targetFileNameProperty = new BoundProperty(
                        Reflector<ITemplateSettings>.GetPropertyName(x => x.TargetFileName),
                        () => this.RawTargetFileName,
                        s => this.RawTargetFileName = s));
            }
        }

        private BoundProperty TargetPathProperty
        {
            get
            {
                return this.targetPathProperty ??
                    (this.targetPathProperty = new BoundProperty(
                        Reflector<ITemplateSettings>.GetPropertyName(x => x.TargetPath),
                        () => this.RawTargetPath,
                        s => this.RawTargetPath = s));
            }
        }
    }
}
