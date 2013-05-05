using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Checks if the dragged data contains solution items that can be dropped
    /// </summary>
    [DisplayNameResource(@"DropSolutionItemCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"DropSolutionItemCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class DropSolutionItemCondition : DropFileCondition
    {
        private static readonly ITracer tracer = Tracer.Get<DropSolutionItemCondition>();

        /// <summary>
        /// Returns the solution items that can be dragged over the current element.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetDraggedFiles()
        {
            tracer.Info(
                Resources.DropSolutionItemCondition_TraceGettingFiles, this.Extension);

            var items = this.DragArgs.GetVSProjectItemsPaths();
            if (!items.Any())
            {
                return Enumerable.Empty<string>();
            }

            // NOTE: we don't assume an extension has been specified.
            if (!string.IsNullOrEmpty(this.Extension))
            {
                return items.GetPathsEndingWithExtensions(this.Extension);
            }

            return items;
        }
    }
}
