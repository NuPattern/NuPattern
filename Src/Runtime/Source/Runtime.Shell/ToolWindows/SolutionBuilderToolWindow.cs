using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using NuPattern.Presentation.VsIde;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.Runtime.UI.Views;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Pattern Explorer tool window.
    /// </summary>
    internal partial class SolutionBuilderToolWindow
    {
        private const string SolutionBuilderVisibilitySetting = "FirstTimeInitialization";
        private const string SolutionBuilderAutoOpenedSetting = "SolutionBuilderAutoOpened";
        private SelectionContainer selectionContainer;
        private SolutionBuilderViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        [Import]
        private IBindingFactory BindingFactory { get; set; }

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IPatternManager PatternManager { get; set; }

        [Import(typeof(SVsServiceProvider))]
        private IServiceProvider ServiceProvider { get; set; }

        public override void OnToolWindowCreated()
        {
            base.OnToolWindowCreated();

            this.selectionContainer = new SelectionContainer();
            this.viewModel.CurrentNodeChanged += this.OnCurrentNodeChanged;
        }

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
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>(true);

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
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<SolutionBuilderToolWindow>())
            {
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>(true);

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
            SelectItem(this.viewModel.CurrentNode == null ? new object[0] : new[] { this.viewModel.CurrentNode.Model });
        }

        private void SelectItem(object[] selectedObjects)
        {
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