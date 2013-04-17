using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Invertable Condition
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Invertable"), DisplayNameResource("InvertableCondition_DisplayName", typeof(Resources))]
    [DescriptionResource("InvertableCondition_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public abstract class InvertableCondition : Condition
    {
        private const bool DefaultIsInverted = false;

        /// <summary>
        /// Creates a new instance of the <see cref="InvertableCondition"/> class.
        /// </summary>
        protected InvertableCondition()
        {
            this.IsInverted = DefaultIsInverted;
        }

        /// <summary>
        /// Gets or sets whether to reverse the evaluation.
        /// </summary>
        [Required]
        [DefaultValue(DefaultIsInverted)]
        [DisplayNameResource("InvertableCondition_IsInverted_DisplayName", typeof(Resources))]
        [DescriptionResource("InvertableCondition_IsInverted_Description", typeof(Resources))]
        internal bool IsInverted { get; set; }

        /// <summary>
        /// Evaluates the derived condition and reverses the evaluation.
        /// </summary>
        public override sealed bool Evaluate()
        {
            this.ValidateObject();

            var result = Evaluate2();
            return (this.IsInverted ? !result : result);
        }

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        protected abstract bool Evaluate2();
    }
}
