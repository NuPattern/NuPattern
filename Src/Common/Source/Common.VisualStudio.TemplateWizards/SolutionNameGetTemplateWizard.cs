using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>SolutionNameGetTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SolutionNameGetTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionNameGetTemplateWizard"/> class.
        /// </summary>
        public SolutionNameGetTemplateWizard()
            : base(TemplateWizardInfo.SolutionNameGetTemplateWizardFullTypeName)
        {
        }
    }
}
