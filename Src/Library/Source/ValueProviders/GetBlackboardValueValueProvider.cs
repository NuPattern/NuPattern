using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance.Extensions;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Gets teh value of the current guidance workflow using the <see cref="BlackboardManager"/>.
    /// </summary>
    [DisplayNameResource("GetBlackboardValueValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("GetBlackboardValueValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Guidance", typeof(Resources))]
    public class GetBlackboardValueValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<GetBlackboardValueValueProvider>();

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [DisplayNameResource("GetBlackboardValueValueProvider_Key_DisplayName", typeof(Resources))]
        [DescriptionResource("GetBlackboardValueValueProvider_Key_Description", typeof(Resources))]
        public string Key { get; set; }

        /// <summary>
        /// Evaluates the provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.GetBlackboardValueValueProvider_TraceInitial, this.Key);

            var result = (BlackboardManager.Current.Get(Key));

            tracer.TraceInformation(
                Resources.GetBlackboardValueValueProvider_TraceEvaluation, this.Key, result);

            return result;
        }
    }
}
