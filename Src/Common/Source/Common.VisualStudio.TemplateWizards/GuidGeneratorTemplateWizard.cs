using Microsoft.VisualStudio.TemplateWizard;
using System;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// Proxy for the <c>GuidGeneratorTemplateWizard</c> class.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class GuidGeneratorTemplateWizard : WizardProxy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidGeneratorTemplateWizard"/> class.
        /// </summary>
        public GuidGeneratorTemplateWizard()
            : base(TemplateWizardInfo.GuidGeneratorTemplateWizardFullTypeName)
        {
        }
    }
}
