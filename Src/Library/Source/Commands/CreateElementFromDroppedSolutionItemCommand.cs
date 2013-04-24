using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each dropped solution item.
    /// </summary>
    [DisplayNameResource(@"CreateElementFromDroppedSolutionItemCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"CreateElementFromDroppedSolutionItemCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class CreateElementFromDroppedSolutionItemCommand : CreateElementFromDroppedFileCommand
    {
        private static readonly ITracer tracer = Tracer.Get<CreateElementFromDroppedSolutionItemCommand>();

        /// <summary>
        /// Returns the files from the dragged data.
        /// </summary>
        protected override IEnumerable<string> GetFilePaths()
        {
            tracer.Info(
                Resources.CreateElementFromDroppedSolutionItemCommand_TraceGettingFiles, this.Extension);

            var items = this.DragArgs.GetVSProjectItemsPaths();
            if (items.Any())
            {
                // Ensure correct file extensions
                var matchingFiles = items.GetPathsEndingWithExtensions(this.Extension);
                if (matchingFiles.Any())
                {
                    return matchingFiles;
                }
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Adds the dropped file to the solution.
        /// </summary>
        protected override bool AddFileToSolution(string filePath)
        {
            // Do nothing, the file was dragged from the solution.
            return true;
        }
    }
}
