using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Bindings;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Automation
{
    internal class DragDropAutomation : AutomationExtension<IDragDropSettings>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<DragDropAutomation>();

        private IDisposable dragEnterEventSubscription;
        private IDisposable dragLeaveEventSubscription;
        private IDisposable dropEventSubscription;
        private bool? conditionsResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="DragDropAutomation"/> class.
        /// </summary>
        public DragDropAutomation(IProductElement owner, IDragDropSettings settings)
            : base(owner, settings)
        {
            this.Conditions = new IDynamicBinding<ICondition>[0];
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), Import(typeof(SVsServiceProvider))]
        private IServiceProvider serviceProvider { get; set; }

        private IVsStatusbar statusBar { get; set; }

        /// <summary>
        /// Gets or sets the binding factory.
        /// </summary>
        [Import]
        public IBindingFactory BindingFactory { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IOnSolutionBuilderDragEnter DragEnterEvent { get; internal set; }

        [Import]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IOnSolutionBuilderDragLeave DragLeaveEvent { get; internal set; }

        [Import]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IOnSolutionBuilderDrop DropEvent { get; internal set; }

        /// <summary>
        /// Gets the conditions that will be evaluated when the event is raised.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<IDynamicBinding<ICondition>> Conditions { get; private set; }

        /// <summary>
        /// Finishes the initialization by subscribing to the source event and 
        /// creating the conditions.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "None.")]
        public override void EndInit()
        {
            base.EndInit();

            this.statusBar = serviceProvider.GetService<SVsStatusbar, IVsStatusbar>();

            //this.DragEnterEvent = GetObservableEvent(typeof(IOnSolutionBuilderDragEnter));
            //this.DragLeaveEvent = GetObservableEvent(typeof(IOnSolutionBuilderDragLeave));
            //this.DropEvent = GetObservableEvent(typeof(IOnSolutionBuilderDrop));

            this.dragEnterEventSubscription = this.DragEnterEvent.Subscribe(this.OnDragEnterEvent);
            this.dragLeaveEventSubscription = this.DragLeaveEvent.Subscribe(this.OnDragLeaveEvent);
            this.dropEventSubscription = this.DropEvent.Subscribe(this.OnDropEvent);

            if (this.BindingFactory != null)
            {
                try
                {
                    this.Conditions = this.Settings.ConditionSettings.Select(x => this.BindingFactory.CreateBinding<ICondition>(x)).ToArray();
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

        //private IObservable<IEvent<DragEventArgs>> GetObservableEvent(Type t)
        //{
        //    return (from observableEvent in this.AllEvents
        //            where observableEvent.Metadata.Id == t.ToString()
        //            select observableEvent.Value as IObservable<IEvent<DragEventArgs>>)
        //                                      .FirstOrDefault();
        //}

        /// <summary>
        /// Executes the automation extension.
        /// </summary>
        public override void Execute()
        {
            this.Execute(null);
        }

        public override void Execute(IDynamicBindingContext context)
        {
            this.Execute(null);
        }

        /// <summary>
        /// Removes the event subscription if necessary.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.dragEnterEventSubscription != null)
                    this.dragEnterEventSubscription.Dispose();
                if (this.dragLeaveEventSubscription != null)
                    this.dragLeaveEventSubscription.Dispose();
                if (this.dropEventSubscription != null)
                    this.dropEventSubscription.Dispose();
            }
        }

        private void Execute(IEvent<EventArgs> args)
        {
            using (tracer.StartActivity(Resources.EventAutomation_StartingExecution, this.Settings.Name))
            {
                using (var context = BindingFactory.CreateContext())
                {
                    AddEventArgsToBindingContext(args, context);

                    tracer.ShieldUI(() =>
                    {
                        if (!conditionsResult.HasValue)
                        {
                            conditionsResult = this.Conditions.All(condition => Evaluate(condition, args));
                        }
                        if (conditionsResult.Value)
                        {
                            using (var tx = this.Owner.BeginTransaction())
                            {
                                if (this.ExecuteWizard(context))
                                {
                                    using (new MouseCursor(Cursors.Wait))
                                    {
                                        if (this.ExecuteCommand(context))
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
        }

        private static void AddEventArgsToBindingContext(IEvent<EventArgs> args, IDynamicBindingContext context)
        {
            //context.AddInterfaceLayer(this.Owner);
            if (args != null)
            {
                // Export the entire event
                context.AddExport(args);
                if (args.EventArgs != null)
                {
                    // Export the args and all its concrete types in the inheritance
                    context.AddExportsFromInheritance(args.EventArgs);
                }
            }

        }

        /// <summary>
        /// Executes the wizard.
        /// </summary>
        /// <returns><b>true</b> if the wizard was not cancelled; otherwise <b>false</b>.</returns>
        internal bool ExecuteWizard(IDynamicBindingContext context)
        {
            if (this.Settings.WizardId != Guid.Empty)
            {
                var wizardAutomation = this.ResolveAutomationReference<IWizardAutomationExtension>(this.Settings.WizardId);
                if (wizardAutomation == null)
                {
                    tracer.TraceError(Resources.MenuAutomation_NoWizardFound, this.Settings.WizardId, this.Name);
                    return false;
                }

                wizardAutomation.Execute(context);
                return !wizardAutomation.IsCanceled;
            }

            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns><b>true</b> if the wizard was not cancelled; otherwise <b>false</b>.</returns>
        internal bool ExecuteCommand(IDynamicBindingContext context)
        {
            if (this.Settings.CommandId != Guid.Empty)
            {
                var commandAutomation = this.ResolveAutomationReference<IAutomationExtension>(this.Settings.CommandId);
                if (commandAutomation == null)
                {
                    tracer.TraceWarning(Resources.EventAutomation_NoCommandFound, this.Settings.CommandId, this.Name);
                    return false;
                }

                commandAutomation.Execute(context);
            }

            return true;
        }

        private bool Evaluate(IDynamicBinding<ICondition> binding, IEvent<EventArgs> args)
        {
            using (var context = binding.CreateDynamicContext(this))
            {
                AddEventArgsToBindingContext(args, context);

                var result = binding.Evaluate(context) && binding.Value.Evaluate();

                if (!result)
                {
                    tracer.TraceData(
                        TraceEventType.Verbose,
                        0,
                        new DictionaryTraceRecord(
                            TraceEventType.Verbose,
                            typeof(EventAutomation).FullName,
                            string.Format(CultureInfo.CurrentCulture,
                                Resources.MenuAutomation_ConditionEvaluatedFalse, this.Name, binding.UserMessage),
                            binding.EvaluationResults));
                }
                return result;
            }
        }

        private void OnDragEnterEvent(IEvent<DragEventArgs> args)
        {
            if (!conditionsResult.HasValue)
            {
                if (this.Owner != null && args.Sender == this.Owner)
                {
                    conditionsResult = this.Conditions.All(condition => Evaluate(condition, args));
                }
                else
                {
                    conditionsResult = false;
                }
            }

            if (conditionsResult.Value)
            {
                args.EventArgs.Effects = DragDropEffects.Link;

                if (!string.IsNullOrEmpty(this.Settings.StatusText))
                {
                    UpdateStatusBar(this.Settings.StatusText);
                }
            }
        }

        private void OnDragLeaveEvent(IEvent<DragEventArgs> args)
        {
            conditionsResult = null;
            UpdateStatusBar(string.Empty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.VisualStudio.Shell.Interop.IVsStatusbar.SetText(System.String)")]
        private void UpdateStatusBar(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                statusBar.SetText(Resources.DragDropAutomation_StatusBarEmpty);
            }
            else
            {
                statusBar.SetText(text);
            }
        }

        private void OnDropEvent(IEvent<DragEventArgs> args)
        {
            this.Execute(args);
        }
    }
}
