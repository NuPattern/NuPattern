using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.Patterning.Runtime.Events;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Library.Conditions
{
    /// <summary>
    /// A <see cref="Condition"/> that evaluates to true if the <see cref="CurrentElement"/> equals the <see cref="Event"/> sender.
    /// </summary>
    [DisplayNameResource("EventSenderMatchesElementCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_Automation", typeof(Resources))]
    [DescriptionResource("EventSenderMatchesElementCondition_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class EventSenderMatchesElementCondition : Condition, IEventCondition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<EventSenderMatchesElementCondition>();

        /// <summary>
        /// Gets or sets the current element which is matched against the <see cref="Event"/> sender.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IInstanceBase CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the event to match the <see cref="CurrentElement"/> with.
        /// </summary>
        public IEvent<EventArgs> Event { get; set; }

        /// <summary>
        /// Evaluates the condition by matching the event sender and the element.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.EventSenderMatchesElementCondition_TraceInitial);

            // Ensure the sender is an element
            var result = false;
            if (this.Event == null
                || this.Event.Sender == null
                || this.CurrentElement == null)
            {
                result = false;
            }
            else
            {
                if (!(this.Event.Sender is IInstanceBase))
                {
                    tracer.TraceInformation(
                        Resources.EventSenderMatchesElementCondition_TraceNotAnElementSource);

                    // Ignore condition, as sender is not any element
                    result = true;
                }
                else
                {
                    result = this.Event.Sender == this.CurrentElement;
                }
            }

            tracer.TraceInformation(
                Resources.EventSenderMatchesElementCondition_TraceEvaluation,
                GetObjectInstanceName(this.CurrentElement), GetObjectInstanceName(this.Event != null ? this.Event.Sender : null), result);

            return result;
        }

        private string GetObjectInstanceName(object thing)
        {
            if (thing == null)
            {
                return string.Empty;
            }

            var asElement = thing as IProductElement;
            if (asElement != null)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}({1})",
                    asElement.InstanceName, asElement.DefinitionName);
            }
            else
            {
                var asInstance = thing as IInstanceBase;
                if (asInstance != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}",
                        asInstance.DefinitionName);
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, ".NETType:{0}",
                        thing.GetType().Name);
                }
            }
        }
    }
}
