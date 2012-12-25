using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.PatternToolkit;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Extensibility;

namespace NuPattern.Authoring.PatternToolkit.Automation.Conditions
{
    /// <summary>
    /// A condition that evaluates if the View has more than one element
    /// </summary>
    [CLSCompliant(false)]
    [DisplayNameResource("LastViewCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("LastViewCondition_Description", typeof(Resources))]
    public class LastViewCondition : Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<LastViewCondition>();

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IViewModel CurrentElement { get; set; }

        /// <summary>
        /// Evaluate if the view has more than one element
        /// </summary>
        /// <returns>True if the view has more than one element; otherwise, false</returns>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.LastViewCondition_TraceInitial, this.CurrentElement.InstanceName);

            var views = this.CurrentElement.Parent.AsCollection(); //TODO: FIX

            var result = views.Elements.Count() > 1;

            tracer.TraceInformation(
                Resources.LastViewCondition_TraceEvaluation, this.CurrentElement.InstanceName, result);

            return result;
        }
    }
}
