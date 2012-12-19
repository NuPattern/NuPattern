using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Library.Automation;
using NuPattern.Library.Design;
using NuPattern.Library.Properties;
using NuPattern.Runtime;

namespace NuPattern.Library.Commands
{
    /// <summary>
    ///  Command that executes a list of commands sequentially.
    /// </summary>
    [DisplayNameResource("AggregatorCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("AggregatorCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class AggregatorCommand : FeatureCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AggregatorCommand>();

        /// <summary>
        /// Gets or sets the command reference list.
        /// </summary>
        /// <value>The command reference list.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists"), Required]
        [DisplayNameResource("AggregatorCommand_CommandReferences_DisplayName", typeof(Resources))]
        [DescriptionResource("AggregatorCommand_CommandReferences_Description", typeof(Resources))]
        [Editor(typeof(CommandReferencesEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(CommandReferencesConverter))]
        [DesignOnly(true)]
        public List<CommandReference> CommandReferenceList { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ICommandSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the message service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUserMessageService MessageService { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            using (tracer.StartActivity(
                Resources.AggregatorCommand_TraceExecutingCommands, this.CurrentElement.InstanceName))
            {
                var settings = this.Settings.Properties.FirstOrDefault(
                    p => p.Name == Reflector<AggregatorCommand>.GetPropertyName(x => x.CommandReferenceList));

                if (settings != null)
                {
                    var converter = new CommandReferencesConverter();

                    this.CommandReferenceList = (List<CommandReference>)converter.ConvertFrom(
                        new SimpleTypeDescriptorContext { Instance = this.Settings },
                        CultureInfo.CurrentCulture,
                        settings.Value);

                    using (new MouseCursor(Cursors.Wait))
                    {
                        foreach (var reference in this.CommandReferenceList)
                        {
                            var commandAutomation =
                                this.CurrentElement.AutomationExtensions.First().ResolveAutomationReference<IAutomationExtension>(reference.CommandId);

                            if (commandAutomation == null)
                            {
                                tracer.TraceWarning(
                                    Resources.AggregatorCommand_TraceNoCommandFound, this.CurrentElement.InstanceName, reference.CommandId);
                                continue;
                            }

                            tracer.TraceInformation(
                                Resources.AggregatorCommand_TraceExecutingCommand, this.CurrentElement.InstanceName, commandAutomation.Name);

                            try
                            {
                                commandAutomation.Execute();
                            }
                            catch (OperationCanceledException cancel)
                            {
                                this.MessageService.ShowError(cancel.Message);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}