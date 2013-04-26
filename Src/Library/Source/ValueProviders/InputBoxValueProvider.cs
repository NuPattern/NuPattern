using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// Returns the value from the user.
    /// </summary>
    [DisplayNameResource(@"InputBoxValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"InputBoxValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class InputBoxValueProvider : ValueProvider
    {
        private static readonly ITracer tracer = Tracer.Get<InputBoxValueProvider>();

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        [DisplayNameResource(@"InputBoxValueProvider_Message_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InputBoxValueProvider_Message_Description", typeof(Resources))]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the defult value
        /// </summary>
        [DisplayNameResource(@"InputBoxValueProvider_DefaultValue_DisplayName", typeof(Resources))]
        [DescriptionResource(@"InputBoxValueProvider_DefaultValue_Description", typeof(Resources))]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets the message service
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUserMessageService MessageService { get; set; }

        /// <summary>
        /// Evaluates the provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.Info(
                Resources.InputBoxValueProvider_TraceInitial, this.Message, this.DefaultValue);

            var result = this.MessageService.PromptInput(this.Message, this.DefaultValue);

            tracer.Info(
                Resources.InputBoxValueProvider_TraceEvaluation, this.Message, this.DefaultValue, result);

            return result;
        }
    }
}
