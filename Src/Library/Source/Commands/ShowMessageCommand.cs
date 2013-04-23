using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Displays a message box
    /// </summary>
    [DisplayNameResource(@"ShowMessageCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ShowMessageCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    public class ShowMessageCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ShowMessageCommand>();

        /// <summary>
        /// Gets the severity level of the message
        /// </summary>
        [Required]
        [DefaultValue(MessageLevel.Information)]
        [DisplayNameResource(@"ShowMessageCommand_Level_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ShowMessageCommand_Level_Description", typeof(Resources))]
        public MessageLevel Level { get; set; }

        /// <summary>
        /// Gets the message to show
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [DisplayNameResource(@"ShowMessageCommand_Message_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ShowMessageCommand_Message_Description", typeof(Resources))]
        public string Message { get; set; }

        /// <summary>
        /// Gets the message service
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUserMessageService MessageService { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ShowMessageCommand_TraceInitial, this.Message, this.Level);

            var message = String.IsNullOrEmpty(this.Message) ? Resources.ShowMessageCommand_DefaultMessage : this.Message;

            switch (this.Level)
            {
                case MessageLevel.Error:
                    this.MessageService.ShowError(message);
                    break;

                case MessageLevel.Warning:
                    this.MessageService.ShowWarning(message);
                    break;

                default:
                    this.MessageService.ShowInformation(message);
                    break;
            }
        }
    }
}