using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.VisualStudio.TemplateWizards;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Runtime.Shell.TemplateWizards
{
    /// <summary>
    /// Custom template wizard extension that opens the solution builder toolwindow.
    /// </summary>
    [CLSCompliant(false)]
    public class OpenSolutionBuilderTemplateWizard : TemplateWizard
    {
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public override void RunFinished()
        {
            base.RunFinished();

            var package = this.serviceProvider.GetService<RuntimeShellPackage>();

            if (package != null)
            {
                package.AutoOpenSolutionBuilder();
            }
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Never dispose the service provider.")]
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            var vs = (automationObject as DTE);
            this.serviceProvider = new ServiceProvider((Ole.IServiceProvider)vs);
        }
    }
}