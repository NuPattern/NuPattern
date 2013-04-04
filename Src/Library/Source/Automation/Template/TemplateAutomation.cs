using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Library.Commands;
using NuPattern.Library.Events;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Bindings;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Implements the runtime automation behavior for both project and
    /// item template automation.
    /// </summary>
    internal class TemplateAutomation : AutomationExtension<ITemplateSettings>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<EventAutomation>();

        private IDisposable eventSubscription;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateAutomation"/> class.
        /// </summary>
        public TemplateAutomation(IProductElement owner, ITemplateSettings settings)
            : base(owner, settings)
        {
        }

        /// <summary>
        /// Gets the instantiation event.
        /// </summary>
        [Import]
        public IOnElementInstantiatedEvent OnInstantiated { get; internal set; }

        /// <summary>
        /// Gets the uri references service.
        /// </summary>
        [Import]
        public IFxrUriReferenceService UriService { get; internal set; }

        /// <summary>
        /// Gets the solution.
        /// </summary>
        [Import]
        public ISolution Solution { get; internal set; }

        /// <summary>
        /// Gets or sets the binding factory to use for evaluating target filename and path.
        /// </summary>
        [Import]
        public IBindingFactory BindingFactory { get; internal set; }

        /// <summary>
        /// Gets the VS service proider
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Finishes the initialization by subscribing to the events.
        /// </summary>
        public override void EndInit()
        {
            // Do not even subscribe to the OnInstatiated event if not setup so.
            if (this.Settings.UnfoldOnElementCreated && this.OnInstantiated != null)
            {
                this.eventSubscription = this.OnInstantiated.Subscribe(this.OnInstantiatedHandler);
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            if (!UnfoldScope.IsActive ||
                !UnfoldScope.Current.HasUnfolded(this.Settings.TemplateUri))
            {
                if (this.Settings.SyncName && this.Settings.TargetFileName.ValueProvider != null)
                    throw new NotSupportedException(Resources.TemplateAutomation_ValueProviderUnsupportedForSyncNames);

                var tag = new ReferenceTag
                {
                    SyncNames = this.Settings.SyncName,
                    TargetFileName = this.Settings.TargetFileName.Value,
                    Id = this.Settings.Id,
                };

                using (new UnfoldScope(this, tag, this.Settings.TemplateUri))
                {
                    // Run the wizard so that the template can do replacements if necessary.
                    if (ExecuteWizard())
                    {
                        if (this.Settings.UnfoldOnElementCreated)
                        {
                            // In this case the template will control execution of the wizard
                            // and command at the right times.
                            using (tracer.StartActivity(Resources.TemplateAutomation_TraceUnfoldingAsset, this.Settings.TemplateUri))
                            {
                                NuPattern.VisualStudio.TraceSourceExtensions.ShieldUI(tracer, () =>
                                {
                                    Action<IDynamicBindingContext> initializer = context =>
                                    {
                                        context.AddExport<ITemplateSettings>(this.Settings);
                                        context.AddExportsFromInterfaces(this.Owner);
                                    };

                                    var fileName = PropertyBindingSettingsExtensions.Evaluate(this.Settings.TargetFileName, this.BindingFactory, tracer, initializer);
                                    var targetPath = PropertyBindingSettingsExtensions.Evaluate(this.Settings.TargetPath, this.BindingFactory, tracer, initializer);

                                    UnfoldVsTemplateCommand.UnfoldTemplate(this.Solution, this.UriService, this.ServiceProvider, this.Owner,
                                        new UnfoldVsTemplateCommand.UnfoldVsTemplateSettings
                                            {
                                                TemplateUri = this.Settings.TemplateUri,
                                                TargetFileName = fileName,
                                                TargetPath = targetPath,
                                                SyncName = this.Settings.SyncName,
                                                SanitizeName = this.Settings.SanitizeName,
                                                Tag = this.Settings.Tag,
                                                Id = this.Settings.Id
                                            }, true);
                                },
                                Resources.InstantiationTemplateWizard_InstantiationFailed, this.Settings.TemplateUri, this.Owner.InstanceName);
                            }
                        }

                        this.ExecuteCommand();
                    }
                    else
                    {
                        // Delete the element from the state, as the user cancelled the action.
                        this.Owner.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Executes the automation behavior on a pre-build context
        /// </summary>
        public override void Execute(IDynamicBindingContext context)
        {
            this.Execute();
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
                    tracer.TraceError(Resources.TemplateAutomation_NoWizardFound, this.Settings.WizardId, this.Name);
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
        /// <returns><b>true</b> if the command could run successfully; otherwise <b>false</b>.</returns>
        internal bool ExecuteCommand()
        {
            if (this.Settings.CommandId != Guid.Empty)
            {
                var commandAutomation = this.ResolveAutomationReference<IAutomationExtension>(this.Settings.CommandId);
                if (commandAutomation == null)
                {
                    tracer.TraceWarning(Resources.TemplateAutomation_NoCommandFound, this.Settings.CommandId, this.Name);
                    return false;
                }

                using (new MouseCursor(Cursors.Wait))
                {
                    commandAutomation.Execute();
                }
            }

            return true;
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

        private void OnInstantiatedHandler(IEvent<EventArgs> args)
        {
            // If the instantiated pattern is not our owner, skip.
            if (args.Sender == this.Owner &&
                (!UnfoldScope.IsActive || UnfoldScope.Current.Automation != this))
            {
                // We only run if the event was not fired by our own execution.
                this.Execute();
            }
        }
    }
}
