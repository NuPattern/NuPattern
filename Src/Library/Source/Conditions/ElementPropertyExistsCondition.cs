using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true if the specified <see cref="PropertyName"/>, exists on the current element,
    /// and optionally, if the property has a value.
    /// </summary>
    [DisplayNameResource("ElementPropertyExistsCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("ElementPropertyExistsCondition_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ElementPropertyExistsCondition : Condition
    {
        private const bool DefaultMustHaveValue = false;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ElementPropertyExistsCondition>();

        /// <summary>
        /// Creates a new instance of the <see cref="ElementPropertyExistsCondition"/> class.
        /// </summary>
        public ElementPropertyExistsCondition()
        {
            this.MustHaveValue = DefaultMustHaveValue;
        }

        /// <summary>
        /// Gets or sets the name of the property of the current pattern/element to read.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("ElementPropertyExistsCondition_PropertyName_DisplayName", typeof(Resources))]
        [DescriptionResource("ElementPropertyExistsCondition_PropertyName_Description", typeof(Resources))]
        public string PropertyName { get; set; }

        /// <summary>
        /// Gets or sets whether the property must have a value.
        /// </summary>
        [Required]
        [DefaultValue(DefaultMustHaveValue)]
        [DisplayNameResource("ElementPropertyExistsCondition_MustHaveValue_DisplayName", typeof(Resources))]
        [DescriptionResource("ElementPropertyExistsCondition_MustHaveValue_Description", typeof(Resources))]
        public bool MustHaveValue { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Evaluates the condition by verifying the existance of the property and whether it has a value.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ElementPropertyExistsCondition_TraceInitial, this.CurrentElement.InstanceName, this.PropertyName, this.MustHaveValue);

            var property = this.CurrentElement.Properties.FirstOrDefault(
                prop => prop.Info.Name == this.PropertyName);
            if (property != null)
            {
                var result = this.MustHaveValue ? !string.IsNullOrEmpty(property.RawValue) : true;

                tracer.TraceInformation(
                    Resources.ElementPropertyExistsCondition_TraceEvaluation, this.CurrentElement.InstanceName, this.PropertyName, this.MustHaveValue, result);

                return result;
            }

            tracer.TraceInformation(
                Resources.ElementPropertyExistsCondition_TraceNoProperty, this.PropertyName, this.CurrentElement.Info.Name);

            return false;
        }
    }
}
