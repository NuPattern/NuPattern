using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Guidance.Extensions;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Sets a value in the current guidance using the <see cref="BlackboardManager"/>
    /// </summary>
    [DisplayNameResource("SetBlackboardValueCommand_DisplayName", typeof(Resources))]
    [DescriptionResource("SetBlackboardValueCommand_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_Guidance", typeof(Resources))]
    public class SetBlackboardValueCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SetBlackboardValueCommand>();

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Required]
        [DisplayNameResource("SetBlackboardValueCommand_Key_DisplayName", typeof(Resources))]
        [DescriptionResource("SetBlackboardValueCommand_Key_Description", typeof(Resources))]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [Required]
        [DisplayNameResource("SetBlackboardValueCommand_Value_DisplayName", typeof(Resources))]
        [DescriptionResource("SetBlackboardValueCommand_Value_Description", typeof(Resources))]
        public string Value { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.SetBlackboardValueCommand_TraceInitial, this.Key, this.Value);

            BlackboardManager.Current.Set(Key, Value);
        }
    }
}