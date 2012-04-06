using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the current value of a variable property of the current element in the pattern model.
    /// </summary>
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ElementPropertyValueProvider_Description", typeof(Resources))]
    [DisplayNameResource("ElementPropertyValueProvider_DisplayName", typeof(Resources))]
    [CLSCompliant(false)]
    public class ElementPropertyValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementPropertyValueProvider>();

        /// <summary>
        /// Gets or sets the name of the property of the current pattern/element to read.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("ElementPropertyValueProvider_PropertyName_DisplayName", typeof(Resources))]
        [DescriptionResource("ElementPropertyValueProvider_PropertyName_Description", typeof(Resources))]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Properties.Resources.ElementPropertyValueProvider_TraceInitial, this.PropertyName, this.CurrentElement);

            return new PropertyEvaluator().Evaluate(this.CurrentElement, this.PropertyName);
        }
    }
}
