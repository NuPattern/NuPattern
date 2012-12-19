using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true if a reference of the specified Reference Kind exists on the current element.
    /// </summary>
    [DisplayNameResource("ElementReferenceExistsCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ElementReferenceExistsCondition_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ElementReferenceExistsCondition : Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementReferenceExistsCondition>();

        /// <summary>
        /// Gets or sets the kind of reference to verify.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("ElementReferenceExistsCondition_Kind_DisplayName", typeof(Resources))]
        [DescriptionResource("ElementReferenceExistsCondition_Kind_Description", typeof(Resources))]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existance of the reference of the given kind.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ElementReferenceExistsCondition_TraceInitial, this.CurrentElement.InstanceName, this.Kind);

            var result = (this.CurrentElement.References.FirstOrDefault(reference => reference.Kind.Equals(this.Kind, StringComparison.OrdinalIgnoreCase)) != null);

            tracer.TraceInformation(
                Resources.ElementReferenceExistsCondition_TraceEvaluation, this.CurrentElement.InstanceName, this.Kind, result);

            return result;
        }
    }
}
