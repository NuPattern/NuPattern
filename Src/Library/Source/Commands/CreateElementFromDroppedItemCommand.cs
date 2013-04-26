using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using NuPattern.Diagnostics;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each dropped item.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class CreateElementFromDroppedItemCommand : CreateElementFromItemCommand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private static readonly ITracer tracer = Tracer.Get<CreateElementFromDroppedItemCommand>();

        /// <summary>
        /// Gets or sets the drag arguments
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        protected DragEventArgs DragArgs
        {
            get;
            set;
        }
    }
}
