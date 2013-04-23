using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.UI;
using NuPattern.Runtime.Guidance.UI.ViewModels;
using NuPattern.Runtime.Guidance.UI.Views;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.ToolWindows
{
    /// <summary>
    /// Provides the Guidance Explorer tool window.
    /// </summary>
    internal partial class GuidanceExplorerToolWindow
    {
        private const string VisibilitySetting = "FirstTimeInitialization";
        private const string GuidanceExplorerAutoOpenedSetting = "GuidanceExplorerAutoOpened";
        private SelectionContainer selectionContainer;
        private GuidanceExplorerViewModel viewModel;
        private IVsTrackSelectionEx trackSelection;

        [Import]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        private IGuidanceManager GuidanceManager { get; set; }

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

            var context = new GuidanceWorkflowContext
            {
                GuidanceManager = this.GuidanceManager,
                Extension = this.GuidanceManager.ActiveGuidanceExtension,
                UserMessageService = this.UserMessageService,
            };

            this.viewModel = new GuidanceExplorerViewModel(context, this.ServiceProvider);
            this.Content = new GuidanceExplorerView { DataContext = this.viewModel };
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
                  store.PropertyExists(Constants.SettingsName, VisibilitySetting)))
            {
                //First time after installation
                packageToolWindow.ShowWindow<GuidanceExplorerToolWindow>(true);

                store.CreateCollection(Constants.SettingsName);
                store.SetString(Constants.SettingsName, VisibilitySetting, bool.FalseString);
            }
            else
            {
                // Afterwards, we load the toolwindow so that the drag&drop events can get access to the 
                // toolwindow usercontrol that handles the operations.
                // Querying visibility will automatically create the control.
                packageToolWindow.IsWindowVisible<GuidanceExplorerToolWindow>();
            }
        }

        /// <summary>
        /// Automatically opens the window, if not already opened.
        /// </summary>
        internal static void AutoOpenWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<GuidanceExplorerToolWindow>())
            {
                var settingsManager = new ShellSettingsManager(serviceProvider);
                var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (!store.CollectionExists(Constants.SettingsName))
                {
                    store.CreateCollection(Constants.SettingsName);
                }

                store.SetString(Constants.SettingsName, GuidanceExplorerAutoOpenedSetting, bool.TrueString);
            }

            packageToolWindow.ShowWindow<GuidanceExplorerToolWindow>(true);
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
                store.PropertyExists(Constants.SettingsName, GuidanceExplorerAutoOpenedSetting))
            {
                var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

                packageToolWindow.HideWindow<GuidanceExplorerToolWindow>();

                store.DeleteProperty(Constants.SettingsName, GuidanceExplorerAutoOpenedSetting);
            }
        }

        /// <summary>
        /// Opens the window, if not already opened.
        /// </summary>
        internal static void OpenWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();
            packageToolWindow.ShowWindow<GuidanceExplorerToolWindow>(true);
        }

        /// <summary>
        /// Hides the window, if it was automatically opened.
        /// </summary>
        internal static void HideWindow(IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);

            var packageToolWindow = serviceProvider.GetService<IPackageToolWindow>();

            if (packageToolWindow.IsWindowVisible<GuidanceExplorerToolWindow>())
            {
                packageToolWindow.HideWindow<GuidanceExplorerToolWindow>();
            }
        }

        private void OnCurrentNodeChanged(object sender, NodeChangedEventArgs e)
        {
            var currentAction = this.viewModel.CurrentWorkflow == null ? null : this.viewModel.CurrentWorkflow.Model.FocusedAction;
            if (currentAction != null)
            {
                SelectItem(new[] { currentAction });
                BrowseNode(currentAction);
            }

            //Ensure GuidanceBrowser tool window is open
            GuidanceBrowserToolWindow.OpenWindow(this);
        }

        private void SelectItem(object[] selectedObjects)
        {
            this.selectionContainer.SelectableObjects = selectedObjects;
            this.selectionContainer.SelectedObjects = selectedObjects;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.trackSelection.OnSelectChange(this.selectionContainer));
        }

        private void BrowseNode(INode node)
        {
            var packageToolWindow = this.ServiceProvider.GetService<IPackageToolWindow>();
            var toolWindow = packageToolWindow.GetWindow<GuidanceBrowserToolWindow>();
            if (toolWindow != null)
            {
                var browser = toolWindow as GuidanceBrowserToolWindow;
                if (browser != null)
                {
                    browser.SelectCurrentNode(node);
                }
            }
        }
    }
}
