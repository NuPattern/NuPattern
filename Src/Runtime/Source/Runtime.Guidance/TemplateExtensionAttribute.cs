using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.ComponentModel.Composition;
using NuPattern.VisualStudio.TemplateWizards;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Specifies that the component is a provided <see cref="IWizard"/>.
    /// </summary>
    internal class TemplateExtensionAttribute : FeatureComponentAttribute
    {
        /// <summary>
        /// Initializes the attribute.
        /// </summary>
        public TemplateExtensionAttribute()
            : base(typeof(ITemplateWizard))
        {
        }
    }
}