using System;
using System.Collections.Generic;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TemplateWizard;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Custom template wizard extension that opens the solution builder toolwindow.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Not Applicable")]
    internal class OpenSolutionBuilderTemplateWizard : Extensibility.TemplateWizard
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