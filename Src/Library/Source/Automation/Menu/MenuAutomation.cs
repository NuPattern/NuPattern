using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Reflection;
using NuPattern.Runtime;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.ToolkitInterface;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Implements the runtime behavior of the menu launch point.
    /// </summary>
    internal class MenuAutomation : AutomationExtension<IMenuSettings>, IAutomationMenuCommand, ICommandStatus, INotifyPropertyChanged
    {
        private static readonly ITracer tracer = Tracer.Get<MenuAutomation>();

        private bool enabled;
        private string text;
        private bool visible;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuAutomation"/> class.
        /// </summary>
        public MenuAutomation(IProductElement owner, IMenuSettings settings)
            : base(owner, settings)
        {
            Guard.NotNull(() => owner, owner);
            Guard.NotNull(() => settings, settings);

            this.text = settings.Text;
            this.enabled = true;
            this.visible = true;
            this.Conditions = new IDynamicBinding<ICondition>[0];
            this.CommandStatus = new FixedBinding<ICommandStatus>(new DefaultCommandStatus());
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Gets or sets the binding factory.
        /// </summary>
        [Import]
        public IBindingFactory BindingFactory { get; set; }

        /// <summary>
        /// Gets the conditions that will be evaluated when the event is raised.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<IDynamicBinding<ICondition>> Conditions { get; private set; }

        /// <summary>
        /// Gets or sets custom the command status binding.
        /// </summary>
        /// <value></value>
        public IDynamicBinding<ICommandStatus> CommandStatus { get; private set; }

        /// <summary>
        /// Gets or sets if the menu item is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                if (value != this.enabled)
                {
                    this.enabled = value;
                    this.RaisePropertyChanged(x => x.Enabled);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text for the menu item.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (value != this.text)
                {
                    this.text = value;
                    this.RaisePropertyChanged(x => x.Text);
                }
            }
        }

        /// <summary>
        /// Gets or sets if the menu item is visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                if (value != this.visible)
                {
                    this.visible = value;
                    this.RaisePropertyChanged(x => x.Visible);
                }
            }
        }

        /// <summary>
        /// Gets the path to the icon in the menu, in pack:// format.
        /// </summary>
        public string IconPath
        {
            get
            {
                return this.Settings.Icon;
            }
        }

        /// <summary>
        /// Gets the ordering for sorting.
        /// </summary>
        public long SortOrder
        {
            get
            {
                return this.Settings.SortOrder;
            }
        }

        /// <summary>
        /// Finishes initialization, resolving the conditions and custom status bindings.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            if (this.BindingFactory != null)
            {
                try
                {
                    this.Conditions = this.Settings.ConditionSettings.Select(x => this.BindingFactory.CreateBinding<ICondition>(x)).ToArray();
                }
                catch (Exception e)
                {
                    tracer.Error(e, Resources.MenuAutomation_FailedToParseConditions, this.Name);
                    if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(e))
                    {
                        throw;
                    }
                }

                if (!string.IsNullOrEmpty(this.Settings.CustomStatus))
                {
                    try
                    {
                        var bindingSettings = BindingSerializer.Deserialize<BindingSettings>(this.Settings.CustomStatus);
                        if (!string.IsNullOrEmpty(bindingSettings.TypeId))
                            this.CommandStatus = this.BindingFactory.CreateBinding<ICommandStatus>(bindingSettings);
                    }
                    catch (Exception e)
                    {
                        tracer.Error(e, Resources.MenuAutomation_FailedToParseCustomStatus, this.Name);
                        if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(e))
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes the automation extension.
        /// </summary>
        public override void Execute()
        {
            this.Execute(this);
        }

        /// <summary>
        /// Run the command after verifying the <see cref="IMenuCommand.Enabled"/> is 
        /// <see langword="true"/>,
        /// </summary>
        public void Execute(IMenuCommand command)
        {
            Guard.NotNull(() => command, command);

            using (tracer.StartActivity(Resources.MenuAutomation_StartingExecution, this.Name))
            {
                tracer.ShieldUI(() =>
                {
                    this.QueryStatus(command);
                    if (!command.Enabled)
                    {
                        tracer.Warn(Resources.MenuAutomation_ConditionsPreventExecution, this.Name);
                        return;
                    }

                    using (var tx = this.Owner.BeginTransaction())
                    {
                        if (this.ExecuteWizard() && this.ExecuteCommand())
                        {
                            tx.Commit();
                        }
                    }
                },
                Resources.MenuAutomation_FailedToExecute, this.Name);
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
                    tracer.Error(Resources.MenuAutomation_NoWizardFound, this.Settings.WizardId, this.Name);
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
                    tracer.Warn(Resources.MenuAutomation_NoCommandFound, this.Settings.CommandId, this.Name);
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
        /// Decide dynamically whether to show this menu command and enable it.
        /// </summary>
        public void QueryStatus(IMenuCommand menu)
        {
            Guard.NotNull(() => menu, menu);

            menu.Enabled = menu.Visible = this.Conditions.All(condition => Evaluate(condition));

            // Invoke custom query status.
            using (var context = this.CommandStatus.CreateDynamicContext(this))
            {
                context.AddInterfaceLayer(this.Owner);

                if (this.CommandStatus.Evaluate(context))
                {
                    this.CommandStatus.Value.QueryStatus(menu);
                }
                else
                {
                    tracer.Error(Resources.MenuAutomation_CustomStatusEvaluationFailed,
                        this.Name,
                        this.CommandStatus.UserMessage, 
                        ObjectDumper.ToString(this.CommandStatus.EvaluationResults, 5));
                }
            }
        }

        private bool Evaluate(IDynamicBinding<ICondition> binding)
        {
            using (var context = binding.CreateDynamicContext(this))
            {
                context.AddInterfaceLayer(this.Owner);
                context.AddInterfaceLayer(this.Owner);
                var isBindingValid = binding.Evaluate(context);
                var isValid = false;

                if (!isBindingValid)
                {
                    tracer.Warn(Resources.MenuAutomation_ConditionBindingFailed, 
                        this.Name, 
                        binding.UserMessage,
                        ObjectDumper.ToString(binding.EvaluationResults, 5));
                }
                else
                {
                    isValid = binding.Value.Evaluate();
                    if (!isValid)
                    {
                        tracer.Verbose(Resources.MenuAutomation_ConditionEvaluatedFalse,
                            this.Name,
                            binding.UserMessage,
                            ObjectDumper.ToString(binding.EvaluationResults, 5));
                    }
                }

                return isBindingValid && isValid;
            }
        }

        private void RaisePropertyChanged<TResult>(Expression<Func<MenuAutomation, TResult>> property)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(Reflector<MenuAutomation>.GetProperty(property).Name));
        }

        private class DefaultCommandStatus : ICommandStatus
        {
            public void QueryStatus(IMenuCommand menu)
            {
            }
        }

    }
}