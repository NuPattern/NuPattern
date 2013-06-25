using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>FixLinkPathTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class FixLinkPathTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixLinkPathTemplateWizard"/> class.
        /// </summary>
        public FixLinkPathTemplateWizard()
            : base(TemplateWizardInfo.FixLinkPathTemplateWizardFullTypeName)
        {
        }
    }
}
