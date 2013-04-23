using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the current value of a variable property of the current element in the pattern model.
    /// </summary>
    [DisplayNameResource(@"CurrentStoreFileValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"CurrentStoreFileValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class CurrentStoreFileValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CurrentStoreFileValueProvider>();

        /// <summary>
        /// The pattern manager.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IPatternManager PatternManager
        {
            get;
            set;
        }

        /// <summary>
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CurrentStoreFileValueProvider_TraceInitial);

            if (this.PatternManager.IsOpen)
            {
                var result = this.PatternManager.StoreFile;

                tracer.TraceInformation(
                    Resources.CurrentStoreFileValueProvider_TraceEvaluation, result);

                return result;
            }

            return null;
        }
    }
}
