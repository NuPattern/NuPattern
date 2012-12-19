using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.ComponentModelHost;
using NuPattern.Extensibility;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.Runtime.UI;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Provides the Pattern Explorer tool window.
    /// </summary>
    [Guid("c44b2e95-86f4-40dd-8fc8-bbc9725ea86b")]
    [CLSCompliant(false)]
    public class SolutionBuilderToolWindow : ToolWindowPane
    {
        private const string SolutionBuilderVisibilitySetting = "FirstTimeInitialization";
        private const string SolutionBuilderAutoOpenedSetting = "SolutionBuilderAutoOpened";
        private SelectionContainer selectionContainer;
        private SolutionBuilderViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionBuilderToolWindow"/> class.
        /// </summary>
        public SolutionBuilderToolWindow() :
            base(null)
        {
            this.Caption = Resources.ExplorerWindowTitle;

            this.BitmapResourceID = 301;
            this.BitmapIndex = 0;
        }

        [Import]
        private IBindingFactory BindingFactory { get; set; }

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IPatternManager PatternManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// This method can be overridden by the derived class to execute any code that must run after the creation of
        /// <see cref="IVsWindowFrame"/>.
        /// </summary>
        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            this.selectionContainer = new SelectionContainer();
            this.viewModel.CurrentNodeChanged += this.OnCurrentNodeChanged;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

            this.trackSelection = this.GetService<SVsTrackSelectionEx, IVsTrackSelectionEx>();
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));

            var shell = this.GetService<SVsUIShell, IVsUIShell>();

            var context = new SolutionBuilderContext
            {
                PatternManager = this.PatternManager,
                UserMessageService = this.UserMessageService,
                BindingFactory = this.BindingFactory,
                NewProductDialogFactory = ctx => shell.CreateDialog<AddNewProductView>(ctx),
                NewNodeDialogFactory = ctx => shell.CreateDialog<AddNewNodeView>(ctx),
                ShowProperties = this.ShowProperties
            };

            this.viewModel = new SolutionBuilderViewModel(context, this.ServiceProvider);
            this.Content = new SolutionBuilderView { DataContext = this.viewModel };
        }

        /// <summary>
        /// Displays the tool window the first time the environment is used after installation.
        /// </summary>
        internal static void InitializeWindowVisibility(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var settingsManager = new ShellSettingsManager(serviceProvider);
            var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!(store.CollectionExists(Constants.SettingsName) &&
                  store.PropertyExists(Constants.SettingsName, SolutionBuilderVisibilitySetting)))
            {
                //First time after installation
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>();

                store.CreateCollection(Constants.SettingsName);
                store.SetString(Constants.SettingsName, SolutionBuilderVisibilitySetting, bool.FalseString);
            }
            else
            {
                // Afterwards, we load the toolwindow so that the drag&drop events can get access to the 
                // toolwindow usercontrol that handles the operations.
                // Querying visibility will automatically create the control.
                packageToolWindow.IsWindowVisible<SolutionBuilderToolWindow>();
            }
        }

        /// <summary>
        /// Automatically opens the window, if not already opened.
        /// </summary>
        internal static void AutoOpenWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(()=> serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<SolutionBuilderToolWindow>())
            {
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>();

                var settingsManager = new ShellSettingsManager(serviceProvider);
                var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (!store.CollectionExists(Constants.SettingsName))
                {
                    store.CreateCollection(Constants.SettingsName);
                }

                store.SetString(Constants.SettingsName, SolutionBuilderAutoOpenedSetting, bool.TrueString);
            }
        }

        /// <summary>
        /// Automatically hides the window, if it was automatically opened.
        /// </summary>
        internal static void AutoHideWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var settingsManager = new ShellSettingsManager(serviceProvider);
            var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (store.CollectionExists(Constants.SettingsName) &&
                store.PropertyExists(Constants.SettingsName, SolutionBuilderAutoOpenedSetting))
            {
                var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

                packageToolWindow.HideWindow<SolutionBuilderToolWindow>();

                store.DeleteProperty(Constants.SettingsName, SolutionBuilderAutoOpenedSetting);
            }
        }

        private void OnCurrentNodeChanged(object sender, EventArgs e)
        {
            var selectedObjects = this.viewModel.CurrentNode == null ? new object[0] : new[] { this.viewModel.CurrentNode.Model };

            this.selectionContainer.SelectableObjects = selectedObjects;
            this.selectionContainer.SelectedObjects = selectedObjects;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));
        }

        private void ShowProperties()
        {
            var dte = this.GetService<EnvDTE.DTE>();

            var window = dte.Windows.Item(EnvDTE.Constants.vsWindowKindProperties);
            if (window != null)
            {
                window.Activate();
            }
        }
    }
}