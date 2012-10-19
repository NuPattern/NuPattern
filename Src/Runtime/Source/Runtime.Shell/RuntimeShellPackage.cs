using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.ComponentModel.Composition.Diagnostics;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library;
using Microsoft.VisualStudio.Patterning.Runtime.Shell.OptionPages;
using Microsoft.VisualStudio.Patterning.Runtime.Shell.Properties;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.Patterning.Runtime.UI;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudio.Patterning.Runtime.Shell
{
    /// <summary>
    /// Represents the VS package for this assembly.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed on package dispose.")]
    [ProvideEditorExtension(typeof(ProductStateEditorFactory), Microsoft.VisualStudio.Patterning.Runtime.Constants.RuntimeStoreExtension, 8, DefaultName = Microsoft.VisualStudio.Patterning.Runtime.Constants.RuntimeStoreEditorDescription)]
    //// TODO: see why we need to load on no solution.
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SolutionBuilderToolWindow), Window = ToolWindowGuids.Toolbox, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed)]
    [ProvideDirectiveProcessor(typeof(ProductStateStoreDirectiveProcessor), ProductStateStoreDirectiveProcessor.ProductStateStoreDirectiveProcessorName, Constants.ProductStateStoreDirectiveProcessorDescription)]
    [ProvideBindingPath]
    [Guid(Constants.RuntimeShellPkgGuid)]
    [CLSCompliant(false)]
    [ProvideService(typeof(IPlatuProjectTypeProvider), ServiceName = "IPlatuProjectTypeProvider")]
    [ProvideService(typeof(IPatternManager), ServiceName = "IPatternManager")]
    [ProvideService(typeof(IPackageToolWindow), ServiceName = "IPackageToolWindow")]
    [ProvideService(typeof(ISolutionEvents), ServiceName = "ISolutionEvents")]
    [ProvideService(typeof(IUserMessageService), ServiceName = "IUserMessageService")]
    [ProvideService(typeof(IBindingFactory), ServiceName = "IBindingFactory")]
    [ProvideService(typeof(IBindingCompositionService), ServiceName = "IBindingCompositionService")]
    [ProvideService(typeof(IToolkitInterfaceService), ServiceName = "IToolkitInferfaceService")]
    [ProvideOptionPageAttribute(typeof(TraceOptionsPage), Constants.SettingsName, "TraceOptionsPage", 17131, 21356, true)]
    [ProvideDirectiveProcessor(typeof(ModelElementDirectiveProcessor), ModelElementDirectiveProcessor.ProcessorName, Constants.LibraryDirectiveProcessorDescription)]
    public sealed class RuntimeShellPackage : Package
    {
        private static readonly Guid OutputPaneGuid = new Guid(Constants.VsOutputWindowPaneId);
        private ProductStateValidator productStateValidator;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not sure if it's OK to leave this container unreferenced by anyone.")]
        private TracingSettingsMonitor tracingMonitor;
        private TraceOutputWindowManager traceOutputWindowManager;

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IPatternManager PatternManager { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IPlatuProjectTypeProvider ProjectTypes { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IBindingFactory BindingFactory { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IBindingCompositionService BindingComposition { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private ISolutionEvents SolutionEvents { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IShellEvents ShellEvents { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private ISettingsManager SettingsManager { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IUserMessageService UserMessageService { get; set; }

        [Import]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "MEF")]
        private IToolkitInterfaceService ToolkitInterfaceService { get; set; }

        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            //Import all services
            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            try
            {
                componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

                var container = (CompositionContainer)componentModel.DefaultCompositionService;

                container.ComposeExportedValue<Func<ISolutionPicker>>(
                    () => new SolutionPicker());

                container.ComposeExportedValue<Func<IProductPicker>>(
                    () => new ProductPicker());
            }
            catch (CompositionContractMismatchException)
            {
                DumpMefLog(componentModel);
                throw;
            }
            catch (ImportCardinalityMismatchException)
            {
                DumpMefLog(componentModel);
                throw;
            }
            catch (CompositionException)
            {
                DumpMefLog(componentModel);
                throw;
            }

            // Add services to VS
            this.AddServices();

            // Monitors settings changes and applies them to underlying trace sources.
            this.tracingMonitor = new TracingSettingsMonitor(this.SettingsManager);

            var sourceNames = GetConfiguredSourceNames(this.SettingsManager.Read());
            this.traceOutputWindowManager = new TraceOutputWindowManager(this, this.ShellEvents, OutputPaneGuid, Resources.TraceOutput_WindowTitle, sourceNames.ToArray());

            // Monitor setting changes to refresh output window.
            this.SettingsManager.SettingsChanged += this.OnSettingsChanged;

            this.InitializeCommands();
            this.RegisterEditorFactory(new ProductStateEditorFactory(this.PatternManager));
            this.productStateValidator = new ProductStateValidator(this);

            this.ShellEvents.ShellInitialized += OnShellInitialized;
            this.SolutionEvents.SolutionOpened += OnSolutionOpened;
            this.SolutionEvents.SolutionClosed += OnSolutionClosed;

            EnsureVsHelperInitializedHack();
        }

        private void EnsureVsHelperInitializedHack()
        {
            var helperType = Type.GetType("Microsoft.VisualStudio.TeamArchitect.PowerTools.VsIde.VsHelper, Microsoft.VisualStudio.TeamArchitect.PowerTools");
            if (helperType != null)
            {
                var providerField = helperType.GetField("serviceProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (providerField != null)
                {
                    var value = providerField.GetValue(null);
                    if (value == null)
                    {
                        using (var provider = new ServiceProvider((Ole.IServiceProvider)this.GetService<SDTE, EnvDTE.DTE>()))
                        {
                            providerField.SetValue(null, provider);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">Specify <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (this.tracingMonitor != null)
                {
                    this.tracingMonitor.Dispose();
                }

                if (this.traceOutputWindowManager != null)
                {
                    this.traceOutputWindowManager.Dispose();
                }

                if (this.productStateValidator != null)
                {
                    this.productStateValidator.Dispose();
                }

                this.ShellEvents.ShellInitialized -= OnShellInitialized;
                this.SolutionEvents.SolutionOpened -= OnSolutionOpened;
                this.SolutionEvents.SolutionClosed -= OnSolutionClosed;
            }
        }

        internal void AutoOpenSolutionBuilder()
        {
            var packageToolWindow = this.GetService<IPackageToolWindow>();

            if (!packageToolWindow.IsWindowVisible<SolutionBuilderToolWindow>())
            {
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>();

                var settingsManager = new ShellSettingsManager(this);
                var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (!store.CollectionExists(Constants.SettingsName))
                {
                    store.CreateCollection(Constants.SettingsName);
                }

                store.SetString(Constants.SettingsName, "SolutionBuilderAutoOpened", bool.TrueString);
            }
        }

        private void AddServices()
        {
            var serviceContainer = (IServiceContainer)this;
            serviceContainer.AddService(typeof(RuntimeShellPackage), this, true);
            serviceContainer.AddService(typeof(IPackageToolWindow), new PackageToolWindow(this), true);
            serviceContainer.AddService(typeof(IPatternManager), new ServiceCreatorCallback((s, t) => this.PatternManager), true);
            serviceContainer.AddService(typeof(IPlatuProjectTypeProvider), new ServiceCreatorCallback((s, t) => this.ProjectTypes), true);
            serviceContainer.AddService(typeof(ISolutionEvents), new ServiceCreatorCallback((s, t) => this.SolutionEvents), true);
            serviceContainer.AddService(typeof(IUserMessageService), new ServiceCreatorCallback((s, t) => this.UserMessageService), true);
            serviceContainer.AddService(typeof(IBindingFactory), new ServiceCreatorCallback((s, t) => this.BindingFactory), true);
            serviceContainer.AddService(typeof(IBindingCompositionService), new ServiceCreatorCallback((s, t) => this.BindingComposition), true);
            serviceContainer.AddService(typeof(IToolkitInterfaceService), new ServiceCreatorCallback((s, t) => this.ToolkitInterfaceService), true);
        }

        private void OnShellInitialized(object sender, EventArgs e)
        {
            var settingsManager = new ShellSettingsManager(this);
            var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            var packageToolWindow = this.GetService<IPackageToolWindow>();

            if (!(store.CollectionExists(Constants.SettingsName) &&
                  store.PropertyExists(Constants.SettingsName, "FirstTimeInitialization")))
            {
                //First time after installation
                packageToolWindow.ShowWindow<SolutionBuilderToolWindow>();

                store.CreateCollection(Constants.SettingsName);
                store.SetString(Constants.SettingsName, "FirstTimeInitialization", bool.FalseString);
            }
            else
            {
                // Afterwards, we load the toolwindow so that the drag&drop events can get access to the 
                // toolwindow usercontrol that handles the operations.
                // Querying visibility will automatically create the control.
                packageToolWindow.IsWindowVisible<SolutionBuilderToolWindow>();
            }
        }

        private void OnSolutionClosed(object sender, SolutionEventArgs e)
        {
            var settingsManager = new ShellSettingsManager(this);
            var store = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

            if (store.CollectionExists(Constants.SettingsName) &&
                store.PropertyExists(Constants.SettingsName, "SolutionBuilderAutoOpened"))
            {
                var packageToolWindow = this.GetService<IPackageToolWindow>();

                packageToolWindow.HideWindow<SolutionBuilderToolWindow>();

                store.DeleteProperty(Constants.SettingsName, "SolutionBuilderAutoOpened");
            }
        }

        private void OnSolutionOpened(object sender, SolutionEventArgs e)
        {
            var pathExpression = string.Concat("*", Runtime.Constants.RuntimeStoreExtension);

            // Ensure solution contains at least one toolkit definition file
            if (e.Solution != null && e.Solution.Find<IItem>(pathExpression).Any())
            {
                this.AutoOpenSolutionBuilder();
            }
        }

        [Conditional("DEBUG")]
        private void DumpMefLog(IComponentModel componentModel)
        {
            PackageUtility.ShowError(this, string.Format(CultureInfo.InvariantCulture, Resources.RuntimeShellPackage_DumpMefLogs, Constants.ProductName));

            var tempFile = Path.Combine(Path.GetTempPath(), "mef.txt");
            using (var writer = new StreamWriter(tempFile, false))
            {
                CompositionInfoTextFormatter.Write(new CompositionInfo(componentModel.DefaultCatalog, componentModel.DefaultExportProvider), writer);
            }

            Process.Start(tempFile);

            tempFile = Path.Combine(Path.GetTempPath(), "mef-powertools.txt");
            using (var writer = new StreamWriter(tempFile, false))
            {
                CompositionInfoTextFormatter.Write(new CompositionInfo(componentModel.GetCatalog(
                    Microsoft.VisualStudio.TeamArchitect.PowerTools.Constants.CatalogName), componentModel.DefaultExportProvider), writer);
            }

            Process.Start(tempFile);
        }

        private static IEnumerable<string> GetConfiguredSourceNames(RuntimeSettings settings)
        {
            var sourceNames = settings.Tracing.TraceSources.Select(s => s.SourceName);
            sourceNames = sourceNames.Concat(new[] { TracingSettings.DefaultRootSourceName });
            sourceNames = sourceNames.Distinct();
            return sourceNames;
        }

        private void OnSettingsChanged(object sender, ChangedEventArgs<RuntimeSettings> e)
        {
            var sourceNames = GetConfiguredSourceNames(e.NewValue);
            this.traceOutputWindowManager.SetTraceSourceNames(sourceNames);
        }

        private void InitializeCommands()
        {
            var menuCommandService = this.GetService<IMenuCommandService, OleMenuCommandService>();
            if (menuCommandService != null)
            {
                menuCommandService.AddCommand(new OpenSolutionBuilderMenuCommand(this.GetService<IPackageToolWindow>()));
            }
        }
    }
}