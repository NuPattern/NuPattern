using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>InstantiationTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class InstantiationTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstantiationTemplateWizard"/> class.
        /// </summary>
        public InstantiationTemplateWizard()
            : base(TemplateWizardInfo.InstantiationTemplateWizardFullTypeName)
        {
        }
    }
}
