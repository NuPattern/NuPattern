using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Conditions
{
    /// <summary>
    /// Checks if the dragged data contains items that can be dropped
    /// </summary>
    [CLSCompliant(false)]
    public abstract class DropItemCondition : Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DropItemCondition>();

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

        /// <summary>
        /// Evaluates the condition
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.DropItemCondition_TraceInitial);

            //Get draggable items
            var draggedItems = GetDraggedItems();
            if (draggedItems != null && draggedItems.Any())
            {
                return true;
            }
            else
            {
                tracer.TraceInformation(
                    Resources.DropItemCondition_TraceEvaluationNoItems);

                return false;
            }
        }

        /// <summary>
        /// Gets the dragged items to process.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected abstract IEnumerable<string> GetDraggedItems();

    }
}
