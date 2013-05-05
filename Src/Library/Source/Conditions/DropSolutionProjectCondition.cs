using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Checks if the dragged data contains solution items that can be dropped
    /// </summary>
    [DisplayNameResource(@"DropSolutionProjectCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"DropSolutionProjectCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class DropSolutionProjectCondition : DropFileCondition
    {
        private static readonly ITracer tracer = Tracer.Get<DropSolutionProjectCondition>();

        /// <summary>
        /// Returns the solution items that can be dragged over the current element.
        /// </summary>
        protected override IEnumerable<string> GetDraggedFiles()
        {
            tracer.Info(
                Resources.DropSolutionProjectCondition_TraceGettingFiles, this.Extension);

            var projects = this.DragArgs.GetVSProjectsPaths();
            if (!projects.Any())
            {
                return Enumerable.Empty<string>();
            }

            // NOTE: we don't assume an extension has been specified.
            // For this to work, we need another change on the DropFileCondition 
            // to make the Extension property non-required.
            if (!string.IsNullOrEmpty(this.Extension))
            {
                return projects.GetPathsEndingWithExtensions(this.Extension);
            }

            return projects;
        }
    }
}
