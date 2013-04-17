using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance.Diagnostics;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Represent a base abstract class for a Feature Command Extension launch point 
    /// that is shown on modeling diagrams (DSL or UML).
    /// </summary>
    internal abstract class ModelingLaunchPoint : ICommandExtension, ILaunchPoint
    {
        protected ModelingLaunchPoint(IFeatureManager featureManager)
        {
            Guard.NotNull(() => featureManager, featureManager);

            this.FeatureManager = FeatureManager;
            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
            this.FeatureInstanceLocator = new DefaultFeatureInstanceLocator(featureManager, this.GetType());
        }

        /// <summary>
        /// Gets the command Text.
        /// </summary>
        [Browsable(false)]
        public virtual string Text
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Gets the binding name of the command.
        /// </summary>
        protected abstract string BindingName { get; }

        protected virtual IFeatureManager FeatureManager { get; private set; }
        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }
        protected virtual IFeatureInstanceLocator FeatureInstanceLocator { get; set; }

        /// <summary>
        /// Optionally implements additional logic to determine if the command can be executed, 
        /// by default checks that the feature is not null.
        /// </summary>
        public virtual bool CanExecute(IFeatureExtension feature)
        {
            if (feature != null)
            {
                var commandBinding = feature.Commands.FindByName(this.BindingName);
                if (commandBinding != null)
                {
                    return commandBinding.Evaluate();
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

            var tracer = FeatureTracer.GetSourceFor(this, feature.FeatureId, feature.InstanceName);

            using (tracer.StartActivity("Executing command {0}", BindingName))
            {
                var commandBinding = feature.Commands.FindByName(this.BindingName);
                commandBinding.Value.Execute();
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

            var canExecute = CanExecute(feature);
            command.Enabled = status.Enabled && canExecute;
            command.Visible = status.Visible && canExecute;

            // Remove this line when the DSL runtime is fixed.
            // see connect on FeedbackID=499548
            command.Visible = true;

            return feature;
        }

        void ICommandExtension.QueryStatus(IMenuCommand command)
        {
            this.OnQueryStatus(command);
        }

        void ICommandExtension.Execute(IMenuCommand command)
        {
            var feature = this.OnQueryStatus(command);
            if (command.Enabled)
            {
                this.Execute(feature);
            }
            else
            {
                var tracer = feature != null ?
                    FeatureTracer.GetSourceFor<VsLaunchPoint>(feature.FeatureId) :
                    Tracer.GetSourceFor(this.GetType());

                tracer.TraceWarning("Attempted to execute launch point {0} but its querystatus did not return Enabled.", this);
            }
        }
    }
}