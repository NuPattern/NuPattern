using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>CustomToolRunnerTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class CustomToolRunnerTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomToolRunnerTemplateWizard"/> class.
        /// </summary>
        public CustomToolRunnerTemplateWizard()
            : base(TemplateWizardInfo.CustomToolRunnerTemplateWizardFullTypeName)
        {
        }
    }
}
