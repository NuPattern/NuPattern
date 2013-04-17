using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance.Diagnostics;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Represents the base implementation of a Feature Command Extension launch point
    /// that is shown at arbitrary menu locations in Visual Studio menu system.
    /// </summary>
    internal abstract class VsLaunchPoint : OleMenuCommand, ILaunchPoint
    {
        protected VsLaunchPoint(IFeatureManager featureManager, CommandID id)
            : base(OnExecute, id)
        {
            Guard.NotNull(() => featureManager, featureManager);

            this.BeforeQueryStatus += this.OnBeforeQueryStatus;
            this.FeatureManager = featureManager;
            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
            this.FeatureInstanceLocator = new DefaultFeatureInstanceLocator(featureManager, this.GetType());
        }

        protected abstract string BindingName { get; }
        protected virtual IFeatureInstanceLocator FeatureInstanceLocator { get; set; }
        protected virtual IFeatureManager FeatureManager { get; private set; }
        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }

        /// <summary>
        /// Optionally implements additional logic to determine if the command can be executed 
        /// for the given feature.
        /// </summary>
        public virtual bool CanExecute(IFeatureExtension feature)
        {
            if (feature != null)
            {
                var commandBinding = feature.Commands.FindByName(this.BindingName);
                if (commandBinding != null)
                {
                    return true;
                    // used to be return commandBinding.Evaluate();
                    // but this cannot be done because evaluating a commandBinding, evaluates all its
                    // arguments as well which breaks the concept of a value provider running a wizard
                    // Evaluation is now deferred to the actual call to Execute
                }
            }

            return false;
        }

        /// <summary>
        /// Implements the execution logic for the launch point.
        /// </summary>
        public virtual void Execute(IFeatureExtension feature)
        {
            if (!this.CanExecute(feature))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Command binding '{0}' can not be executed.",
                    this.BindingName));
            }

            var commandBindingForEval = feature.Commands.FindByName(this.BindingName);
            if (commandBindingForEval != null)
            {
                commandBindingForEval.Evaluate();
            }

            var tracer = FeatureTracer.GetSourceFor(this, feature.FeatureId, feature.InstanceName);

            using (tracer.StartActivity("Executing command {0}", BindingName))
            {
                var commandBinding = feature.Commands.FindByName(this.BindingName);
                commandBinding.Value.Execute();
            }
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            var launchPoint = (VsLaunchPoint)sender;
            var command = new VsMenuCommand { Enabled = launchPoint.Enabled, Text = launchPoint.Text, Visible = launchPoint.Visible };
            var feature = launchPoint.OnQueryStatus(command);

            if (command.Enabled)
            {
                launchPoint.Execute(feature);
            }
            else
            {
                var tracer = feature != null ?
                    FeatureTracer.GetSourceFor<VsLaunchPoint>(feature.FeatureId) :
                    Tracer.GetSourceFor<VsLaunchPoint>();

                tracer.TraceWarning("Attempted to execute launch point {0} but its querystatus did not return Enabled.", launchPoint);
            }
        }

        /// <summary>
        /// Implements the query status behavior, and determines which feature 
        /// owns the given command for the purposes of execution.
        /// </summary>
        protected virtual IFeatureExtension OnQueryStatus(IMenuCommand command)
        {
            var feature = this.FeatureInstanceLocator.LocateInstance();
            var status = this.QueryStatusStrategy.QueryStatus(feature);

            var canExecute = this.CanExecute(feature);
            command.Enabled = status.Enabled && canExecute;
            command.Visible = status.Visible && canExecute;

            return feature;
        }

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            var launchPoint = (VsLaunchPoint)sender;
            var command = new VsMenuCommand
            {
                Enabled = launchPoint.Enabled,
                Text = launchPoint.Text,
                Visible = launchPoint.Visible
            };

            launchPoint.OnQueryStatus(command);

            launchPoint.Enabled = command.Enabled;
            launchPoint.Visible = command.Visible;
            launchPoint.Text = command.Text;
        }

        private class VsMenuCommand : IMenuCommand
        {
            public bool Enabled { get; set; }
            public string Text { get; set; }
            public bool Visible { get; set; }
        }
    }
}