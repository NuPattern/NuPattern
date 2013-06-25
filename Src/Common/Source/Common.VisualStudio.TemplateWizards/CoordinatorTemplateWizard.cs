using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>CoordinatorTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class CoordinatorTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinatorTemplateWizard"/> class.
        /// </summary>
        public CoordinatorTemplateWizard()
            : base(TemplateWizardInfo.CoordinatorTemplateWizardFullTypeName)
        {
        }
    }
}
