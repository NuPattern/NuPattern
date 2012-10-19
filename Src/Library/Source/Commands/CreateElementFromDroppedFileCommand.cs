using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each dropped file.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class CreateElementFromDroppedFileCommand : CreateElementFromFileCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateElementFromDroppedFileCommand>();

        /// <summary>
        /// Gets or sets the drag arguments
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        protected DragEventArgs DragArgs { get; set; }

        /// <summary>
        /// The file extensions which are supported.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("CreateElementFromDroppedFileCommand_Extension_DisplayName", typeof(Resources))]
        [DescriptionResource("CreateElementFromDroppedFileCommand_Extension_Description", typeof(Resources))]
        public virtual string Extension { get; set; }

        /// <summary>
        /// Returns the files from the dragged data.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetFilePaths()
        {
            tracer.TraceInformation(
                Resources.CreateElementFromDroppedFileCommand_TraceGettingFiles, this.Extension);

            var items = this.DragArgs.GetWindowsFilePaths();
            if (items.Any())
            {
                // Ensure correct file extensions
                var matchingfiles = items.GetPathsEndingWithExtensions(this.Extension);
                if (matchingfiles.Any())
                {
                    return matchingfiles;
                }
            }

            return Enumerable.Empty<string>();
        }
    }
}
