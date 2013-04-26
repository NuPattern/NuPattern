using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each dropped file.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class CreateElementFromDroppedFileCommand : CreateElementFromFileCommand
    {
        private static readonly ITracer tracer = Tracer.Get<CreateElementFromDroppedFileCommand>();

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
        [DisplayNameResource(@"CreateElementFromDroppedFileCommand_Extension_DisplayName", typeof(Resources))]
        [DescriptionResource(@"CreateElementFromDroppedFileCommand_Extension_Description", typeof(Resources))]
        public virtual string Extension { get; set; }

        /// <summary>
        /// Returns the files from the dragged data.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<string> GetFilePaths()
        {
            tracer.Info(
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
