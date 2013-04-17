using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that returns a new randomly created GUID.
    /// </summary>
    [DisplayNameResource("RandomGuidValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("RandomGuidValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class RandomGuidValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<RandomGuidValueProvider>();

        /// <summary>
        /// Creates a new instance of the <see cref="RandomGuidValueProvider"/> class.
        /// </summary>
        public RandomGuidValueProvider()
        {
            this.Format = GuidFormat.JustDigitsWithHyphens;
        }

        /// <summary>
        /// The format of the returned guid.
        /// </summary>
        [DisplayNameResource("RandomGuidValueProvider_Format_DisplayName", typeof(Resources))]
        [DescriptionResource("RandomGuidValueProvider_Format_Description", typeof(Resources))]
        [DefaultValue(GuidFormat.JustDigitsWithHyphens)]
        [Required]
        public GuidFormat Format { get; set; }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.RandomGuidValueProvider_TraceInitial, this.Format);

            var result = Guid.NewGuid().ToString(GuidHelper.GetFormat(this.Format));

            tracer.TraceInformation(
                Resources.RandomGuidValueProvider_TraceEvaluation, this.Format, result);

            return result;
        }
    }
}
