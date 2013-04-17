using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Returns items dragged from Solution Explorer to Solution Builder
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("DroppedItemContainerValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("DroppedItemContainerValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    public class DroppedItemContainerValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DroppedItemContainerValueProvider>();

        /// <summary>
        /// Gets or sets the drag arguments.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [Import(AllowDefault = true)]
        [Required]
        public DragEventArgs DragArgs { get; set; }

        /// <summary>
        /// Evaluates the dragged items from Solution Explorer
        /// </summary>
        /// <returns></returns>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.DroppedItemContainerValueProvider_TraceInitial);

            List<string> paths = new List<string>();
            paths.AddRange(DragArgs.GetVSProjectItemsPaths());
            paths.AddRange(DragArgs.GetVSProjectsPaths());

            tracer.TraceInformation(
                Resources.DroppedItemContainerValueProvider_TraceEvaluation, string.Join(";", paths.ToArray<string>()));

            return paths;
        }
    }
}
