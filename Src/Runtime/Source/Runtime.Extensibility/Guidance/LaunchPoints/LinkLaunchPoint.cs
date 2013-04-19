using System;
using System.Globalization;
using System.Windows;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance.Diagnostics;
using NuPattern.Runtime.Guidance.Workflow;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    internal abstract class LinkLaunchPoint : ILaunchPoint
    {
        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }
        protected IGuidanceManager GuidanceManager;

        protected LinkLaunchPoint(IGuidanceManager guidanceManager)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);
            GuidanceManager = guidanceManager;
            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
        }

        protected abstract string BindingName { get; }

        public virtual bool CanExecute(IGuidanceExtension extension)
        {
            if (extension != null && this.QueryStatusStrategy.QueryStatus(extension).Enabled)
            {
                //
                // Ok, the default strategy said we're go.  Let's look at the active node
                // in the workflow
                //
                IGuidanceWorkflow activeWorkflow = extension.GuidanceWorkflow != null ? extension.GuidanceWorkflow : GuidanceManager.ActiveGuidanceExtension.GuidanceWorkflow;
                if (activeWorkflow != null)
                {
                    INode node = activeWorkflow.FocusedAction as INode;
                    if (node.State == NodeState.Enabled)
                        return true;
                }
            }

            NotifyUser();

            return false;
        }

        /// <summary>
        /// This method is called by CanExecute when a link is clicked but can't execute.
        /// Authors can override this method to provide custom notification
        /// </summary>
        public virtual void NotifyUser()
        {
            MessageBox.Show("Unable to execute command because the associated guidance action is not in the Enabled (green) state.");
        }

        public virtual void Execute(IGuidanceExtension extension)
        {
            if (!this.CanExecute(extension))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Command {0}' can not be executed.",
                    this.BindingName));
            }

            var tracer = GuidanceExtensionTracer.GetSourceFor(this, extension.ExtensionId, extension.InstanceName);

            using (tracer.StartActivity("Executing command {0}", BindingName))
            {
                var commandBinding = extension.Commands.FindByName(this.BindingName);
                if (commandBinding != null)
                {
                    commandBinding.Evaluate();
                    commandBinding.Value.Execute();
                }
            }
        }
    }
}