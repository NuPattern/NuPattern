using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Returns items that have been dropped in Solution Builder
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource(@"GenericDroppedItemValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"GenericDroppedItemValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    public class GenericDroppedItemValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GenericDroppedItemValueProvider>();

        /// <summary>
        /// Gets or sets the drag arguments.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Import(AllowDefault = true)]
        [Required]
        public DragEventArgs DragArgs { get; set; }

        /// <summary>
        /// The format of the item to return
        /// </summary>
        [Required]
        [DisplayNameResource(@"GenericDroppedItemValueProvider_Format_DisplayName", typeof(Resources))]
        [DescriptionResource(@"GenericDroppedItemValueProvider_Format_Description", typeof(Resources))]
        public string Format { get; set; }

        /// <summary>
        /// Runs the value provider, returning the dropped item
        /// </summary>
        /// <returns>The dropped item</returns>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.GenericDroppedItemValueProvider_TraceInitial);

            var result = DragArgs.Data.GetData(Format);

            tracer.TraceInformation(
                Resources.GenericDroppedItemValueProvider_TraceEvaluation, string.Join(@";", DragArgs.Data.GetFormats()));

            return result;
        }
    }
}
