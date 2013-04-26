using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Modeling.ExtensionEnablement;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;

namespace NuPattern.Runtime.Guidance.LaunchPoints
{
    /// <summary>
    /// Represents the base implementation of a Command Extension launch point
    /// that is shown at arbitrary menu locations in Visual Studio menu system.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class VsLaunchPoint : OleMenuCommand, ILaunchPoint
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VsLaunchPoint"/> class.
        /// </summary>
        /// <param name="guidanceManager"></param>
        /// <param name="id"></param>
        protected VsLaunchPoint(IGuidanceManager guidanceManager, CommandID id)
            : base(OnExecute, id)
        {
            Guard.NotNull(() => guidanceManager, guidanceManager);

            this.BeforeQueryStatus += this.OnBeforeQueryStatus;
            this.GuidanceManager = guidanceManager;
            this.QueryStatusStrategy = new DefaultQueryStatusStrategy(this.GetType().Name);
            this.GuidanceInstanceLocator = new DefaultGuidanceInstanceLocator(guidanceManager, this.GetType());
        }

        /// <summary>
        /// Gets the name of the binding.
        /// </summary>
        protected abstract string BindingName { get; }

        /// <summary>
        /// Gets the instance locator.
        /// </summary>
        protected virtual IGuidanceInstanceLocator GuidanceInstanceLocator { get; set; }

        /// <summary>
        /// Gets the <see cref="IGuidanceManager"/>.
        /// </summary>
        protected virtual IGuidanceManager GuidanceManager { get; private set; }

        /// <summary>
        /// Gets the <see cref="IQueryStatusStrategy"/>.
        /// </summary>
        protected virtual IQueryStatusStrategy QueryStatusStrategy { get; set; }

        /// <summary>
        /// Determines whether the launch point can execute.
        /// </summary>
        public virtual bool CanExecute(IGuidanceExtension extension)
        {
            if (extension != null)
            {
                var commandBinding = extension.Commands.FindByName(this.BindingName);
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
        /// Executes the launch point.
        /// </summary>
        public virtual void Execute(IGuidanceExtension extension)
        {
            if (!this.CanExecute(extension))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.VsLaunchPoint_ErrorCommandCannotExecute,
                    this.BindingName));
            }

            var commandBindingForEval = extension.Commands.FindByName(this.BindingName);
            if (commandBindingForEval != null)
            {
                commandBindingForEval.Evaluate();
            }

            var tracer = Tracer.Get(extension.GetType());

            using (tracer.StartActivity(Resources.VsLaunchPoint_TraceExecute, BindingName))
            {
                var commandBinding = extension.Commands.FindByName(this.BindingName);
                commandBinding.Value.Execute();
            }
        }

        private static void OnExecute(object sender, EventArgs e)
        {
            var launchPoint = (VsLaunchPoint)sender;
            var command = new VsMenuCommand { Enabled = launchPoint.Enabled, Text = launchPoint.Text, Visible = launchPoint.Visible };
            var extension = launchPoint.OnQueryStatus(command);

            if (command.Enabled)
            {
                launchPoint.Execute(extension);
            }
            else
            {
                var tracer = Tracer.Get<VsLaunchPoint>();

                tracer.Warn(Resources.VsLaunchPoint_TraceNotEnabled, launchPoint);
            }
        }

        /// <summary>
        /// Returns the guidance extension that owns the given command to execute.
        /// </summary>
        protected virtual IGuidanceExtension OnQueryStatus(IMenuCommand command)
        {
            var extension = this.GuidanceInstanceLocator.LocateInstance();
            var status = this.QueryStatusStrategy.QueryStatus(extension);

            var canExecute = this.CanExecute(extension);
            command.Enabled = status.Enabled && canExecute;
            command.Visible = status.Visible && canExecute;

            return extension;
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
