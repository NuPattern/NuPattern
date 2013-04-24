using System;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.Automation;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.ToolkitInterface;
using Application = System.Windows.Application;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Defines a wizard automation.
    /// </summary>
    internal class WizardAutomation : AutomationExtension<IWizardSettings>, IWizardAutomationExtension
    {
        private static readonly ITracer tracer = Tracer.Get<WizardAutomation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardAutomation"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="settings">The settings.</param>
        public WizardAutomation(IProductElement owner, IWizardSettings settings)
            : base(owner, settings)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is canceled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is canceled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCanceled { get; private set; }

        /// <summary>
        /// Gets or sets the feature composition.
        /// </summary>
        [Import]
        [Required]
        public INuPatternCompositionService CompositionService { get; set; }

        [Import(typeof(SVsServiceProvider))]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public override void Execute()
        {
            using (tracer.StartActivity(Resources.WizardAutomation_StartingExecution, this.Name, this.Settings.TypeName))
            {
                this.ValidateObject();

                var wizardType = Type.GetType(this.Settings.TypeName);
                if (wizardType == null)
                {
                    throw new TypeLoadException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.WizardAutomation_CannotLoadWizardType,
                        this.Settings.TypeName));
                }

                if (!typeof(Window).IsAssignableFrom(wizardType))
                {
                    throw new InvalidOperationException(string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.WizardAutomation_WizardTypeMustBeWindow,
                        this.Settings.TypeName));
                }

                var wizard = (Window)Activator.CreateInstance(wizardType, this.GetArguments(wizardType));
                this.CompositionService.SatisfyImportsOnce(wizard);

                if (Application.Current != null)
                {
                    wizard.Owner = Application.Current.MainWindow;
                    wizard.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                }
                else
                {
                    wizard.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }

                wizard.DataContext = this.Owner;

                using (new MouseCursor(System.Windows.Input.Cursors.Arrow))
                {
                    this.IsCanceled = !wizard.ShowDialog().GetValueOrDefault();
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

        private object[] GetArguments(Type wizardType)
        {
            if (wizardType.GetConstructor(new[] { typeof(IServiceProvider) }) != null)
            {
                var proxyProvider = new ProxyServiceProvider(this.ServiceProvider);
                proxyProvider.AddService(typeof(IWindowsFormsEditorService), new WindowsFormsEditorService());
                return new object[] { proxyProvider };
            }

            return new object[0];
        }

        private class WindowsFormsEditorService : IWindowsFormsEditorService
        {
            public void CloseDropDown()
            {
                throw new NotImplementedException();
            }

            public void DropDownControl(Control control)
            {
                throw new NotImplementedException();
            }

            public DialogResult ShowDialog(Form dialog)
            {
                Guard.NotNull(() => dialog, dialog);

                return dialog.ShowDialog();
            }
        }
    }
}