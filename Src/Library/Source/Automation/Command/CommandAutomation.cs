using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a command automation.
    /// </summary>
    internal class CommandAutomation : AutomationExtension<ICommandSettings>
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CommandAutomation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandAutomation"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="settings">The settings.</param>
        public CommandAutomation(IProductElement owner, ICommandSettings settings)
            : base(owner, settings)
        {
        }

        /// <summary>
        /// Gets the command binding.
        /// </summary>
        internal IDynamicBinding<IFeatureCommand> CommandBinding { get; private set; }

        /// <summary>
        /// Gets or sets the binding factory.
        /// </summary>
        /// <value>The binding factory.</value>
        [Import]
        internal IBindingFactory BindingFactory { get; set; }

        /// <summary>
        /// Finishes initialization by creating the binding using the factory.
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            this.CommandBinding = this.BindingFactory.CreateBinding<IFeatureCommand>(this.Settings);
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            using (tracer.StartActivity(Resources.CommandAutomation_StartingExecution, this.Name, this.Settings.TypeId))
            using (var context = this.CommandBinding.CreateDynamicContext(this))
            {
                Execute(context, false);
            }
        }

        /// <summary>
        /// Executes the automation behavior on a pre-build context
        /// </summary>
        public override void Execute(IDynamicBindingContext context)
        {
            Execute(context, true);
        }

        private void Execute(IDynamicBindingContext context, bool withAutomation)
        {
            if (withAutomation)
            {
                context.AddAutomation(this);
            }

            context.AddExport(this.Settings);
            context.AddInterfaceLayer(this.Owner);

            var isValid = this.CommandBinding.Evaluate(context);

            if (isValid)
            {
                tracer.TraceVerbose(Resources.CommandAutomation_BindingEvaluatedTrue);
                this.CommandBinding.Value.Execute();
            }
            else
            {
                tracer.TraceRecord(this, TraceEventType.Warning,
                    this.CommandBinding.EvaluationResults,
                    string.Format(CultureInfo.CurrentCulture,
                        Resources.CommandAutomation_BindingEvaluatedFalse, this.Name));
            }
        }
    }
}