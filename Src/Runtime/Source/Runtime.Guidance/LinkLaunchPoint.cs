using System;
using System.Globalization;
using System.Windows;
using NuPattern;
using NuPattern.Diagnostics;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal abstract class LinkLaunchPoint : ILaunchPoint
    {
        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }
        protected IFeatureManager ourFeatureManager;

        protected LinkLaunchPoint(IFeatureManager featureManager)
        {
            Guard.NotNull(() => featureManager, featureManager);
            ourFeatureManager = featureManager;
            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
        }

        protected abstract string BindingName { get; }

        public virtual bool CanExecute(IFeatureExtension feature)
        {
            if (feature != null && this.QueryStatusStrategy.QueryStatus(feature).Enabled)
            {
                //
                // Ok, the default strategy said we're go.  Let's look at the active node
                // in the workflow
                //
                IGuidanceWorkflow activeWorkflow = feature.GuidanceWorkflow != null ? feature.GuidanceWorkflow : ourFeatureManager.ActiveFeature.GuidanceWorkflow;
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

        public virtual void Execute(IFeatureExtension feature)
        {
            if (!this.CanExecute(feature))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Command {0}' can not be executed.",
                    this.BindingName));
            }

            var tracer = FeatureTracer.GetSourceFor(this, feature.FeatureId, feature.InstanceName);

            using (tracer.StartActivity("Executing command {0}", BindingName))
            {
                var commandBinding = feature.Commands.FindByName(this.BindingName);
                if (commandBinding != null)
                {
                    commandBinding.Evaluate();
                    commandBinding.Value.Execute();
                }
            }
        }
    }
}