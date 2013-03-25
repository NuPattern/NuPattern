using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Invokes the validation of the current element.
    /// </summary>
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ValidateElementCommand_Description", typeof(Resources))]
    [DisplayNameResource("ValidateElementCommand_DisplayName", typeof(Resources))]
    [CLSCompliant(false)]
    public class ValidateElementCommand : FeatureCommand
    {
        private const bool DefaultValidateDescendants = true;

        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ValidateElementCommand>();

        /// <summary>
        /// Initializes a new instance fo the <see cref="ValidateElementCommand"/> class.
        /// </summary>
        public ValidateElementCommand()
        {
            this.ValidateDescendants = DefaultValidateDescendants;
        }

        /// <summary>
        /// Gets or sets whether to validate the descendants of the current element.
        /// </summary>
        [DefaultValue(DefaultValidateDescendants)]
        [DisplayNameResource("ValidateElementCommand_ValidateDescendantsDisplayName", typeof(Resources))]
        [DescriptionResource("ValidateElementCommand_ValidateDescendantsDescription", typeof(Resources))]
        public bool ValidateDescendants { get; set; }

        /// <summary>
        /// The current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// The pattern manager.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IPatternManager PatternManager { get; set; }

        /// <summary>
        /// Executes the validation behavior.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ValidateElementCommand_TraceInitial, this.CurrentElement.InstanceName, this.ValidateDescendants);

            var instances = this.ValidateDescendants ? this.CurrentElement.Traverse().OfType<IInstanceBase>() : new[] { this.CurrentElement };

            var elements = instances.Concat(instances.OfType<IProductElement>().SelectMany(e => e.Properties));

            var result = this.PatternManager.Validate(elements);

            tracer.TraceInformation(
                Resources.ValidateElementCommand_TraceResult, this.CurrentElement.InstanceName, this.ValidateDescendants, result);
        }
    }
}