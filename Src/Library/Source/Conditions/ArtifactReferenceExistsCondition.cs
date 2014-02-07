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
    /// A <see cref="Condition"/> that evaluates to true if a solution artifact reference exists on the current element.
    /// </summary>
    [DisplayNameResource(@"ArtifactReferenceExistsCondition_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ArtifactReferenceExistsCondition_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class ArtifactReferenceExistsCondition : Condition
    {
        private static readonly ITracer tracer = Tracer.Get<ArtifactReferenceExistsCondition>();

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets an optional tag to filter which solution items get considered
        /// </summary>
        [DisplayNameResource(@"ArtifactReferenceExistsCondition_Tag_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ArtifactReferenceExistsCondition_Tag_Description", typeof(Resources))]
        [DefaultValue("")]
        public string Tag { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existence of any artifact references.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.ArtifactReferenceExistsCondition_TraceInitial, this.CurrentElement.InstanceName);

            var tagFilter = new Func<IReference, bool>(x => true);
            if (!string.IsNullOrEmpty(this.Tag))
            {
                tagFilter = r => r.ContainsTag(this.Tag);
            }

            var result = SolutionArtifactLinkReference.GetReferenceValues(this.CurrentElement, tagFilter)
                .Any();

            tracer.Info(
                Resources.ArtifactReferenceExistsCondition_TraceEvaluation, this.CurrentElement.InstanceName, result);

            return result;
        }
    }
}
