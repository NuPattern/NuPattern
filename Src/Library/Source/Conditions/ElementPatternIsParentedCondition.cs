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
    /// Condition to test if the element pattern is parented
    /// </summary>
    [DisplayNameResource("ElementPatternIsParentedCondition_DisplayName", typeof(Resources))]
    [DescriptionResource("ElementPatternIsParentedCondition_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class ElementPatternIsParentedCondition : InvertableCondition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementPatternIsParentedCondition>();

        /// <summary>
        /// Gets or sets the current pattern element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentPattern { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying is the parent pattern is parented.
        /// </summary>
        protected override bool Evaluate2()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ElementPatternIsParentedCondition_TraceInitial, this.CurrentPattern.InstanceName);

            var result = this.CurrentPattern.IsProductParented();

            tracer.TraceInformation(
                Resources.ElementPatternIsParentedCondition_TraceEvaluation, this.CurrentPattern.InstanceName, result);

            return result;
        }
    }
}
