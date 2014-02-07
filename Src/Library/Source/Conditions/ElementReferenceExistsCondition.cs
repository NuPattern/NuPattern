using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.References;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true if a reference of the specified Reference Kind exists on the current element.
    /// </summary>
    [DisplayNameResource(@"ElementReferenceExistsCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ElementReferenceExistsCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class ElementReferenceExistsCondition : Condition
    {
        private static readonly ITracer tracer = Tracer.Get<ElementReferenceExistsCondition>();

        /// <summary>
        /// Gets or sets the kind of reference to verify.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ElementReferenceExistsCondition_Kind_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ElementReferenceExistsCondition_Kind_Description", typeof(Resources))]
        public string Kind { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets an optional tag to filter which solution items get considered
        /// </summary>
        [DisplayNameResource(@"ElementReferenceExistsCondition_Tag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ElementReferenceExistsCondition_Tag_Description", typeof(Resources))]
        [DefaultValue("")]
        public string Tag { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existance of the reference of the given kind.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ElementReferenceExistsCondition_TraceInitial, this.CurrentElement.InstanceName, this.Kind);

            // Get the first reference
            var reference = this.CurrentElement.References
                .FirstOrDefault(r => 
                    r.Kind.Equals(this.Kind, StringComparison.OrdinalIgnoreCase)
                    && (!string.IsNullOrEmpty(this.Tag)) ? r.ContainsTag(this.Tag) : true);

            var result = (reference != null);

            tracer.Info(
                Resources.ElementReferenceExistsCondition_TraceEvaluation, this.CurrentElement.InstanceName, this.Kind, result);

            return result;
        }
    }
}
