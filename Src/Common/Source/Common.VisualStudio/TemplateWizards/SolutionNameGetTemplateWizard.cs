using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Diagnostics;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.TemplateWizards
{
    /// <summary>
    /// An extension that gets the replacement keys $solutionname$
    /// and $safesolutionname$ from the top-level 
    /// $projectname$ and $safeprojectname$.
    /// </summary>
    [CLSCompliant(false)]
    public class SolutionNameGetTemplateWizard : TemplateWizard
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SolutionNameGetTemplateWizard>();

        private const string SolutionNameKey = "$solutionname$";
        private const string SafeSolutionNameKey = "$safesolutionname$";

        /// <summary>
        /// Executes when the wizard starts.
        /// </summary>
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            Guard.NotNull(() => replacementsDictionary, replacementsDictionary);
            Guard.NotNull(() => customParams, customParams);

            object contextSolutionName = CallContext.LogicalGetData(SolutionNameKey);
            object contextSafeSolutionName = CallContext.LogicalGetData(SafeSolutionNameKey);

            if (contextSolutionName != null)
            {
                tracer.TraceVerbose(Resources.SolutionNameTemplateWizard_ContextSolutionName, contextSolutionName);

                replacementsDictionary[SolutionNameKey] = (string)contextSolutionName;

                if (contextSafeSolutionName != null)
                {
                    replacementsDictionary[SafeSolutionNameKey] = (string)contextSafeSolutionName;
                }
            }
        }
    }
}