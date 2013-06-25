using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>SolutionNameSetTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class SolutionNameSetTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionNameSetTemplateWizard"/> class.
        /// </summary>
        public SolutionNameSetTemplateWizard()
            : base(TemplateWizardInfo.SolutionNameSetTemplateWizardFullTypeName)
        {
        }
    }
}
