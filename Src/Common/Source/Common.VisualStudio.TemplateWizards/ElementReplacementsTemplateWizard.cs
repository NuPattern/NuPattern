using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>ElementReplacementsTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class ElementReplacementsTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementReplacementsTemplateWizard"/> class.
        /// </summary>
        public ElementReplacementsTemplateWizard()
            : base(TemplateWizardInfo.ElementReplacementsTemplateWizardFullTypeName)
        {
        }
    }
}
