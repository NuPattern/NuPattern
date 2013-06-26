using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>ReplacementTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class ReplacementTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplacementTemplateWizard"/> class.
        /// </summary>
        public ReplacementTemplateWizard()
            : base(TemplateWizardInfo.ReplacementTemplateWizardFullTypeName)
        {
        }
    }
}
