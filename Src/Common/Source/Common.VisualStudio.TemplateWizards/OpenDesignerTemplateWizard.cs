using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>OpenDesignerTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class OpenDesignerTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDesignerTemplateWizard"/> class.
        /// </summary>
        public OpenDesignerTemplateWizard()
            : base(TemplateWizardInfo.OpenDesignerTemplateWizardFullTypeName)
        {
        }
    }
}
