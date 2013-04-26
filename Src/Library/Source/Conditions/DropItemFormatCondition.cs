using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Checks if the specified format exists in the drag event's data collection
    /// </summary>
    [DisplayNameResource(@"DropItemFormatCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"DropItemFormatCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class DropItemFormatCondition : Condition
    {
        private static readonly ITracer tracer = Tracer.Get<DropItemFormatCondition>();

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public System.Windows.DragEventArgs DragArgs { get; set; }

        /// <summary>
        /// The format to check for
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"DropItemFormatCondition_Format_DisplayName", typeof(Resources))]
        [DescriptionResource(@"DropItemFormatCondition_Format_Description", typeof(Resources))]
        public string Format { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existence of any data object of the specified format.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.DropItemFormatCondition_TraceInitial, this.Format);

            var result = DragArgs.Data.GetDataPresent(Format);

            tracer.Info(
                Resources.DropItemFormatCondition_TraceEvaluation, this.Format, result);

            return result;
        }
    }
}
