using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>VsRootSuffixTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class VsRootSuffixTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VsRootSuffixTemplateWizard"/> class.
        /// </summary>
        public VsRootSuffixTemplateWizard()
            : base(TemplateWizardInfo.VsRootSuffixTemplateWizardFullTypeName)
        {
        }
    }
}
