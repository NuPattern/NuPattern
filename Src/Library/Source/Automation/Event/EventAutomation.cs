using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Events;
using NuPattern.Runtime.ToolkitInterface;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Implements the runtime behavior of the event handler launch point.
    /// </summary>
    internal class EventAutomation : AutomationExtension<IEventSettings>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<EventAutomation>();

        private IDisposable eventSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAutomation"/> class.
        /// </summary>
        public EventAutomation(IProductElement owner, IEventSettings settings)
            : base(owner, settings)
        {
            this.Conditions = new ICondition[0];
        }

        /// <summary>
        /// Gets or sets all events exposed in the environment.
        /// </summary>
        [ImportMany]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<Lazy<IObservableEvent, IIdMetadata>> AllEvents { get; internal set; }

        /// <summary>
        /// Gets or sets the binding factory.
        /// </summary>
        [Import]
        public IBindingFactory BindingFactory { get; set; }

        /// <summary>
        /// Gets the conditions that will be evaluated when the event is raised.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<ICondition> Conditions { get; private set; }

        /// <summary>
        /// Finishes the initialization by subscribing to the source event and 
        /// creating the conditions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "None.")]
        public override void EndInit()
        {
            base.EndInit();

            if (this.AllEvents != null)
            {
                var sourceEvent = (from observableEvent in this.AllEvents
                                   where observableEvent.Metadata.Id == this.Settings.EventId
                                   select observableEvent.Value as IObservable<IEvent<EventArgs>>)
                                  .FirstOrDefault();

                if (sourceEvent == null)
                {
                    tracer.TraceWarning(Resources.EventAutomation_NoEventFound, this.Settings.EventId, this.Name);
                }
                else
                {
                    this.eventSubscription = sourceEvent.Subscribe(this.OnRaised);
                }
            }

            if (this.BindingFactory != null)
            {
                try
                {
                    this.Conditions = this.Settings.ConditionSettings
                        .Select(x => this.BindingFactory.CreateBinding<ICondition>(x))
                        .Select(x => EvaluateBinding(x))
                        .Where(c => c != null)
                        .ToArray();
                }
                catch (Exception e)
                {
                    tracer.TraceError(e, Resources.EventAutomation_FailedToParseConditions, this.Name);
                    if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(e))
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Executes the automation extension.
        /// </summary>
        public override void Execute()
        {
            this.Execute(null);
        }

        /// <summary>
        /// Removes the event subscription if necessary.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing && this.eventSubscription != null)
            {
                this.eventSubscription.Dispose();
            }
        }

        private void Execute(IEvent<EventArgs> args)
        {
            using (tracer.StartActivity(Resources.EventAutomation_StartingExecution, this.Settings.EventId))
            {
                tracer.ShieldUI(() =>
                {
                    var conditionsResult = this.Conditions.All(condition =>
                    {
                        // Optimized passing of current event value to the 
                        // conditions that want to receive it by explicitly 
                        // implementing IEventCondition. Avoid re-MEFing 
                        // conditions.
                        var eventCondition = condition as IEventCondition;
                        if (eventCondition != null)
                            eventCondition.Event = args;

                        return condition.Evaluate();
                    });

                    if (conditionsResult)
                    {
                        using (var tx = this.Owner.BeginTransaction())
                        {
                            if (this.ExecuteWizard())
                            {
                                using (new MouseCursor(Cursors.Wait))
                                {
                                    if (this.ExecuteCommand())
                                    {
                                        tx.Commit();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        tracer.TraceVerbose(Resources.EventAutomation_ConditionsPreventExecution, this.Name);
                    }
                },
                Resources.EventAutomation_FailedToExecute, this.Name);
            }
        }

        /// <summary>
        /// Executes the wizard.
        /// </summary>
        /// <returns><b>true</b> if the wizard was not cancelled; otherwise <b>false</b>.</returns>
        internal bool ExecuteWizard()
        {
            if (this.Settings.WizardId != Guid.Empty)
            {
                var wizardAutomation = this.ResolveAutomationReference<IWizardAutomationExtension>(this.Settings.WizardId);
                if (wizardAutomation == null)
                {
                    tracer.TraceError(Resources.MenuAutomation_NoWizardFound, this.Settings.WizardId, this.Name);
                    return false;
                }

                wizardAutomation.Execute();
                return !wizardAutomation.IsCanceled;
            }

            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns><b>true</b> if the wizard was not cancelled; otherwise <b>false</b>.</returns>
        internal bool ExecuteCommand()
        {
            if (this.Settings.CommandId != Guid.Empty)
            {
                var commandAutomation = this.ResolveAutomationReference<IAutomationExtension>(this.Settings.CommandId);
                if (commandAutomation == null)
                {
                    tracer.TraceWarning(Resources.EventAutomation_NoCommandFound, this.Settings.CommandId, this.Name);
                    return false;
                }

                commandAutomation.Execute();
            }

            return true;
        }

        private ICondition EvaluateBinding(IDynamicBinding<ICondition> binding)
        {
            using (var context = binding.CreateDynamicContext(this))
            {
                context.AddInterfaceLayer(this.Owner);

                if (!binding.Evaluate(context))
                {
                    tracer.TraceData(
                        TraceEventType.Warning,
                        0,
                        new DictionaryTraceRecord(
                            TraceEventType.Verbose,
                            typeof(EventAutomation).FullName,
                            string.Format(CultureInfo.CurrentCulture,
                                Resources.EventAutomation_ConditionBindingFailed, this.Name, binding.UserMessage),
                            binding.EvaluationResults));

                    return InvalidBindingCondition.Instance;
                }
                else
                {
                    return binding.Value;
                }
            }
        }

        private void OnRaised(IEvent<EventArgs> args)
        {
            this.Execute(args);
        }

        /// <summary>
        /// Executes the automation behavior on a pre-build context
        /// </summary>
        public override void Execute(IDynamicBindingContext context)
        {
            this.Execute();
        }

        private class InvalidBindingCondition : ICondition
        {
            static InvalidBindingCondition()
            {
                Instance = new InvalidBindingCondition();
            }

            private InvalidBindingCondition()
            {
            }

            public static ICondition Instance { get; private set; }

            public bool Evaluate()
            {
                return false;
            }
        }
    }
}