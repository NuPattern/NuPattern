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
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.ComponentModel.Composition.Diagnostics;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.IO;
using NuPattern.Library;
using NuPattern.Reflection;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Diagnostics;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.LaunchPoints;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.Runtime.Schema;
using NuPattern.Runtime.Settings;
using NuPattern.Runtime.Shell.Commands;
using NuPattern.Runtime.Shell.OptionPages;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.Runtime.Shell.ToolWindows;
using NuPattern.Runtime.Store;
using NuPattern.Runtime.ToolkitInterface;
using NuPattern.Runtime.UI;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Commands;
using NuPattern.VisualStudio.Extensions;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Represents the VS package for this assembly.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed on package dispose.")]
    [ProvideEditorExtension(typeof(ProductStateEditorFactory), NuPattern.Runtime.StoreConstants.RuntimeStoreExtension, 8, DefaultName = NuPattern.Runtime.StoreConstants.RuntimeStoreEditorDescription)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource(@"Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SolutionBuilderToolWindow), Window = ToolWindowGuids.Toolbox, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed)]
    [ProvideToolWindow(typeof(GuidanceExplorerToolWindow), Window = Constants.SolutionBuilderToolWindowGuid, Orientation = ToolWindowOrientation.Bottom, Style = VsDockStyle.Linked)]
    [ProvideToolWindow(typeof(GuidanceBrowserToolWindow), Window = ToolWindowGuids.TaskList, Style = VsDockStyle.Tabbed)]
    [ProvideDirectiveProcessor(typeof(ModelElementDirectiveProcessor), ModelElementDirectiveProcessor.ProcessorName, Constants.LibraryDirectiveProcessorDescription)]
    [ProvideDirectiveProcessor(typeof(ProductStateStoreDirectiveProcessor), ProductStateStoreDirectiveProcessor.ProductStateStoreDirectiveProcessorName, Constants.ProductStateStoreDirectiveProcessorDescription)]
    [ProvideDirectiveProcessor(typeof(PatternModelDirectiveProcessor), PatternModelDirectiveProcessor.PatternModelDirectiveProcessorName, Constants.PatternModelDirectiveProcessorDescription)]
    [Microsoft.VisualStudio.Modeling.Shell.ProvideBindingPath]
    [Guid(Constants.RuntimeShellPkgGuid)]
    [CLSCompliant(false)]
    [ProvideService(typeof(IUriReferenceService), ServiceName = @"IUriReferenceService")]
    [ProvideService(typeof(ITemplateService), ServiceName = @"TemplateService")]
    [ProvideService(typeof(ISolution), ServiceName = @"ISolution")]
    [ProvideService(typeof(IExtensionManager), ServiceName = @"IExtensionManager")]
    [ProvideService(typeof(IGuidanceManager), ServiceName = @"IGuidanceManager")]
    [ProvideService(typeof(INuPatternCompositionService), ServiceName = @"INuPatternCompositionService")]
    [ProvideService(typeof(IPatternWindows), ServiceName = @"IGuidanceWindowsService")]
    [ProvideService(typeof(INuPatternProjectTypeProvider), ServiceName = @"INuPatternProjectTypeProvider")]
    [ProvideService(typeof(IPatternManager), ServiceName = @"IPatternManager")]
    [ProvideService(typeof(IPackageToolWindow), ServiceName = @"IPackageToolWindow")]
    [ProvideService(typeof(ISolutionEvents), ServiceName = @"ISolutionEvents")]
    [ProvideService(typeof(IUserMessageService), ServiceName = @"IUserMessageService")]
    [ProvideService(typeof(IBindingFactory), ServiceName = @"IBindingFactory")]
    [ProvideService(typeof(IBindingCompositionService), ServiceName = @"IBindingCompositionService")]
    [ProvideService(typeof(IToolkitInterfaceService), ServiceName = @"IToolkitInferfaceService")]
    [ProvideOptionPage(typeof(TraceOptionsPage), Constants.SettingsName, @"TraceOptionsPage", 17131, 21356, true)]
    public sealed class RuntimeShellPackage : Package
    {
        private static readonly ITracer tracer = Tracer.Get<RuntimeShellPackage>();
        private const int IdleTimeout = 5000;
        private const int GuidanceEvalTimeGovernor = 1; // In Seconds
        private static readonly Guid OutputPaneGuid = new Guid(Constants.VsOutputWindowPaneId);
        private ProductStateValidator productStateValidator;
        private VsIdleTaskHost idleTaskHost;
        private bool showWindows = false;

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Not sure if it's OK to leave this container unreferenced by anyone.")]
        private TracingSettingsMonitor tracingMonitor;
        private TraceOutputWindowManager traceOutputWindowManager;

#pragma warning disable 0649
        [Import]
        private INuPatternCompositionService CompositionService { get; set; }
        [Import]
        private IGuidanceManager GuidanceManager { get; set; }
        [Import]
        private IUriReferenceService UriReferenceService { get; set; }
        [Import]
        private ITemplateService TemplateService { get; set; }
        [Import]
        private ISolution Solution { get; set; }
        [Import]
        private IExtensionManager ExtensionManager { get; set; }
        [Import]
        private IPatternWindows GuidanceWindowsService { get; set; }
        [ImportMany(typeof(ILaunchPoint))]
        private IEnumerable<Lazy<ILaunchPoint>> LaunchPoints { get; set; }
        [Import]
        private IPatternManager PatternManager { get; set; }
        [Import]
        private INuPatternProjectTypeProvider ProjectTypes { get; set; }
        [Import]
        private IBindingFactory BindingFactory { get; set; }
        [Import]
        private IBindingCompositionService BindingComposition { get; set; }
        [Import]
        private ISolutionEvents SolutionEvents { get; set; }
        [Import]
        private IShellEvents ShellEvents { get; set; }
        [Import]
        private ISettingsManager SettingsManager { get; set; }
        [Import]
        private IUserMessageService UserMessageService { get; set; }
        [Import]
        private IToolkitInterfaceService ToolkitInterfaceService { get; set; }
#pragma warning restore 0649

#if VSVER11
        private ResolveEventHandler assemblyResolve = OnAssemblyResolve;
        [ThreadStatic]
        private static bool isResolveAssemblyRunningOnThisThread = false;
#endif
        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

#if VSVER11
            //Register global assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += this.assemblyResolve;
            isResolveAssemblyRunningOnThisThread = true;
#endif

            //Import all services
            var componentModel = this.GetService<SComponentModel, IComponentModel>();
            try
            {
                componentModel.DefaultCompositionService.SatisfyImportsOnce(this);

                var container = (CompositionContainer)componentModel.DefaultCompositionService;

                container.ComposeExportedValue<Func<ISolutionPicker>>(
                    () => new SolutionPicker());

                container.ComposeExportedValue<Func<ISolutionSelector>>(
                    () => new SolutionSelector());

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
            this.traceOutputWindowManager = new TraceOutputWindowManager(this, this.ShellEvents, OutputPaneGuid, Constants.OutputWindowTitle, sourceNames.ToArray());

            // Monitor setting changes to refresh output window.
            this.SettingsManager.SettingsChanged += this.OnSettingsChanged;

            this.InitializeCommands();
            this.RegisterEditorFactory(new ProductStateEditorFactory(this.PatternManager));
            this.productStateValidator = new ProductStateValidator(this);

            this.ShellEvents.ShellInitialized += OnShellInitialized;
            this.SolutionEvents.SolutionOpened += OnSolutionOpened;
            this.SolutionEvents.SolutionClosed += OnSolutionClosed;

            // If initializing other packages launchpoints worked, 
            // they wouldn't need to do this themselves.
            this.RegisterRefreshGuidanceStates();
            this.GuidanceManager.InstantiatedExtensionsChanged += this.OnInstantiatedGuidanceExtensionChanged;
            this.GuidanceManager.ActiveExtensionChanged += this.OnActiveGuidanceExtensionChanged;

            this.InitializeVsLaunchPoints();
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

                if (this.idleTaskHost != null)
                {
                    this.idleTaskHost.Dispose();
                }

                if (this.ShellEvents != null)
                {
                    this.ShellEvents.ShellInitialized -= OnShellInitialized;
                }
                if (this.SolutionEvents != null)
                {
                    this.SolutionEvents.SolutionOpened -= OnSolutionOpened;
                    this.SolutionEvents.SolutionClosed -= OnSolutionClosed;
                }
#if VSVER11
                isResolveAssemblyRunningOnThisThread = false;
                if (this.assemblyResolve != null)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= this.assemblyResolve;
                }
#endif
            }
        }

        internal void AutoOpenToolWindows()
        {
            SolutionBuilderToolWindow.AutoOpenWindow(this);
            GuidanceExplorerToolWindow.AutoOpenWindow(this);
        }

        private void AddServices()
        {
            var serviceContainer = (IServiceContainer)this;
            serviceContainer.AddService(typeof(IUriReferenceService), new ServiceCreatorCallback((c, s) => this.UriReferenceService), true);
            serviceContainer.AddService(typeof(ITemplateService), new ServiceCreatorCallback((c, s) => this.TemplateService), true);
            serviceContainer.AddService(typeof(ISolution), new ServiceCreatorCallback((c, s) => this.Solution), true);
            serviceContainer.AddService(typeof(IExtensionManager), new ServiceCreatorCallback((c, s) => this.ExtensionManager), true);
            serviceContainer.AddService(typeof(IGuidanceManager), new ServiceCreatorCallback((c, s) => this.GuidanceManager), true);
            serviceContainer.AddService(typeof(INuPatternCompositionService), new ServiceCreatorCallback((c, s) => this.CompositionService), true);
            serviceContainer.AddService(typeof(IPatternWindows), new ServiceCreatorCallback((c, s) => this.GuidanceWindowsService), true);
            serviceContainer.AddService(typeof(RuntimeShellPackage), this, true);
            serviceContainer.AddService(typeof(IPackageToolWindow), new PackageToolWindow(this), true);
            serviceContainer.AddService(typeof(IPatternManager), new ServiceCreatorCallback((s, t) => this.PatternManager), true);
            serviceContainer.AddService(typeof(INuPatternProjectTypeProvider), new ServiceCreatorCallback((s, t) => this.ProjectTypes), true);
            serviceContainer.AddService(typeof(ISolutionEvents), new ServiceCreatorCallback((s, t) => this.SolutionEvents), true);
            serviceContainer.AddService(typeof(IUserMessageService), new ServiceCreatorCallback((s, t) => this.UserMessageService), true);
            serviceContainer.AddService(typeof(IBindingFactory), new ServiceCreatorCallback((s, t) => this.BindingFactory), true);
            serviceContainer.AddService(typeof(IBindingCompositionService), new ServiceCreatorCallback((s, t) => this.BindingComposition), true);
            serviceContainer.AddService(typeof(IToolkitInterfaceService), new ServiceCreatorCallback((s, t) => this.ToolkitInterfaceService), true);
        }

        private void OnShellInitialized(object sender, EventArgs e)
        {
            CheckFertNotInstalled();

            SolutionBuilderToolWindow.InitializeWindowVisibility(this);
            GuidanceExplorerToolWindow.InitializeWindowVisibility(this);
        }

        private void OnSolutionClosed(object sender, SolutionEventArgs e)
        {
            SolutionBuilderToolWindow.AutoHideWindow(this);
            GuidanceExplorerToolWindow.AutoHideWindow(this);
            GuidanceBrowserToolWindow.HideWindow(this);
        }

        private void OnSolutionOpened(object sender, SolutionEventArgs e)
        {
            var pathExpression1 = Path.Combine(SolutionExtensions.SolutionItemsFolderName, string.Concat(@"*", Runtime.StoreConstants.RuntimeStoreExtension));
            var pathExpression2 = string.Concat(@"*", Runtime.StoreConstants.RuntimeStoreExtension);

            // Ensure solution contains at least one state file
            if (e.Solution != null)
            {
                // Search Solution Items folder
                var solutionFiles = e.Solution.Find<IItem>(pathExpression1);
                if (solutionFiles.Any())
                {
                    SolutionBuilderToolWindow.AutoOpenWindow(this);
                }
                else
                {
                    // Search whole solution for state file.
                    solutionFiles = e.Solution.Find<IItem>(pathExpression2);
                    if (solutionFiles.Any())
                    {
                        SolutionBuilderToolWindow.AutoOpenWindow(this);
                    }
                }
            }

            if (!this.GuidanceManager.IsOpened)
            {
                this.GuidanceManager.Open(new SolutionDataState(this.Solution));

                // Open guidance windows
                GuidanceExplorerToolWindow.AutoOpenWindow(this);
                //GuidanceBrowserToolWindow.OpenWindow(this);
            }
        }

        [Conditional(@"DEBUG")]
        private void DumpMefLog(IComponentModel componentModel)
        {
            PackageUtility.ShowError(this, string.Format(CultureInfo.InvariantCulture, Resources.RuntimeShellPackage_DumpMefLogs, Constants.ProductName));

            var tempFile = string.Empty;
            try
            {
                // Write out the default VS catalog
                tempFile = Path.Combine(Path.GetTempPath(), "mef.txt");
                using (var writer = new StreamWriter(tempFile, false))
                {
                    CompositionInfoTextFormatter.Write(new CompositionInfo(componentModel.DefaultCatalog, componentModel.DefaultExportProvider), writer);
                }

                Process.Start(tempFile);
            }
            catch (IOException)
            {
                // Ignore writing issues
            }

            try
            {
                // Write out the NuPattern catalog
                tempFile = Path.Combine(Path.GetTempPath(), "mef-nupattern.txt");
                using (var writer = new StreamWriter(tempFile, false))
                {
                    CompositionInfoTextFormatter.Write(new CompositionInfo(componentModel.GetCatalog(
                        Catalog.DefaultCatalogName), componentModel.DefaultExportProvider), writer);
                }

                Process.Start(tempFile);
            }
            catch (IOException)
            {
                // Ignore writing issues
            }
        }

        private static IEnumerable<string> GetConfiguredSourceNames(IRuntimeSettings settings)
        {
            var sourceNames = settings.Tracing.TraceSources.Select(s => s.SourceName);
            sourceNames = sourceNames.Concat(new[] { TracingSettings.DefaultRootSourceName });
            sourceNames = sourceNames.Distinct();
            return sourceNames;
        }

        /// <summary>
        /// Reloads trace settings, when user changes settings in Options dialog
        /// </summary>
        private void OnSettingsChanged(object sender, ChangedEventArgs<IRuntimeSettings> e)
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
                menuCommandService.AddCommand(new OpenGuidanceExplorerMenuCommand(this.GetService<IPackageToolWindow>()));
                menuCommandService.AddCommand(new OpenGuidanceBrowserMenuCommand(this.GetService<IPackageToolWindow>()));
            }
        }

        /// <summary>
        /// Checks to see if an older version of the Feature Extension Runtime extension is installed.
        /// </summary>
        /// <remarks>
        /// Cannot tolerate either version of FERT being installed (i.e. FeatureExtensionUltimateRuntime or FeatureExtensionRuntime versions)
        /// </remarks>
        private void CheckFertNotInstalled()
        {
            if (this.ExtensionManager != null)
            {
                var enabledExtensions = this.ExtensionManager.GetEnabledExtensions();
                var fertExtension = enabledExtensions.FirstOrDefault(ext =>
                        Constants.FertVsixIdentifiers.Contains(ext.Header.Identifier, StringComparer.OrdinalIgnoreCase));
                if (fertExtension != null)
                {
                    tracer.Error(
                        Resources.RuntimeShellPackage_CheckFertInstalled_Enabled,
                        fertExtension.Header.Name,
                        Constants.ProductName);

                    //Prompt user to manually uninstall the FERT extension
                    this.UserMessageService.ShowWarning(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.RuntimeShellPackage_CheckFertInstalled_Enabled,
                        fertExtension.Header.Name,
                        Constants.ProductName));
                }
            }
        }

        /// <summary>
        /// Installs any VsMenuLaunchPoints defined by guidance extensions in environment
        /// </summary>
        private void InitializeVsLaunchPoints()
        {
            var menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (menuCommandService != null)
            {
                var lps = this.LaunchPoints
                    .Select(lazy => lazy.Value)
                    .OfType<VsLaunchPoint>();

                foreach (var launchPoint in lps)
                {
                    menuCommandService.AddCommand(launchPoint);
                }
            }
        }

        private void OnInstantiatedGuidanceExtensionChanged(object sender, EventArgs args)
        {
            this.showWindows = true;
        }

        /// <summary>
        /// Ensures guidance windows are displayed when the guidance is changed (i.e. programmatically)
        /// </summary>
        private void OnActiveGuidanceExtensionChanged(object sender, EventArgs args)
        {
            var activeExtension = this.GuidanceManager.ActiveGuidanceExtension;
            if (activeExtension != null && activeExtension.GuidanceWorkflow != null)
            {
                if (this.showWindows)
                {
                    this.GuidanceWindowsService.ShowGuidanceExplorer(this);
                    this.GuidanceWindowsService.ShowGuidanceBrowser(this);
                }
            }
        }

        private void RegisterRefreshGuidanceStates()
        {
            var conditionsEvaluator = new GuidanceConditionsEvaluator(this.GuidanceManager);
            this.idleTaskHost = new VsIdleTaskHost(this, conditionsEvaluator.EvaluateGraphs, TimeSpan.FromSeconds(GuidanceEvalTimeGovernor));
            this.idleTaskHost.Start(TimeSpan.FromMilliseconds(IdleTimeout));
        }

#if VSVER11
        /// <summary>
        /// Finds toolkit assemblies (in the current AppDomain) that are loaded with a partial name.
        /// A partial name omits either the Version, Culture or PublicKeyToken.
        /// Runtime loads many types dynamically using methods such as Type.GetType(string) with partial names, which aids versioning migrations.
        /// VS2012 for some reason does not resolve assemblies with partial names unless the assemblies are signed.
        /// </summary>
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            //TODO: Why do we get called twice to resolve any assembly, despite the fact that we resolved it the first time?

            Assembly assembly = null;

            try
            {
                // Only process events from the thread that started it, not any other thread
                if (isResolveAssemblyRunningOnThisThread)
                {
                    // Determine if lost assembly has a partial name 
                    // (only these are the ones that cause VS2012 difficulty in resolving)
                    var name = new AssemblyName(args.Name);
                    if (name.Version == null || name.CultureInfo == null)
                    {
                        tracer.Info(
                            Resources.RuntimeShellPackage_OnAssemblyResolved_ResolvingAssembly, args.Name);

                        var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();
                        var patternManager = componentModel.GetService<IPatternManager>();
                        if (patternManager != null)
                        {
                            // Match assemblies with lost name from loaded AppDomain assemblies
                            var loadedAssemblies = from appDomainAssembly
                                                         in ((AppDomain)sender).GetAssemblies()
                                                   let assemblyName = appDomainAssembly.GetName()
                                                   where assemblyName.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase)
                                                   select appDomainAssembly;
                            if (loadedAssemblies.Any())
                            {
                                tracer.Info(
                                    Resources.RuntimeShellPackage_OnAssemblyResolved_ResolvingAssemblyForLoadedAssembly, args.Name);

                                // Get install paths of all (enabled) toolkits
                                var installedExtensionDirs = from installedToolkit
                                                                 in patternManager.InstalledToolkits
                                                             where installedToolkit.Extension.State == EnabledState.Enabled
                                                             select new DirectoryInfo(installedToolkit.Extension.InstallPath);
                                if (installedExtensionDirs.Any())
                                {
                                    // Match only (physical) assemblies installed by toolkits (by highest version)
                                    var comparer = new DirectoryInfoComparer();
                                    var toolkitAssemblies = from toolkitAssembly
                                                                in loadedAssemblies
                                                            let location = toolkitAssembly.Location
                                                            where !toolkitAssembly.IsDynamic
                                                                && !String.IsNullOrEmpty(location)
                                                                && File.Exists(location)
                                                            let assemblyName = toolkitAssembly.GetName()
                                                            let assemblyVersion = assemblyName.Version
                                                            where installedExtensionDirs.Contains(Directory.GetParent(location), comparer)
                                                            orderby (assemblyVersion != null) ? assemblyVersion.ToString(4) : new Version().ToString(4) descending
                                                            select toolkitAssembly;
                                    if (toolkitAssemblies.Any())
                                    {
                                        tracer.Info(
                                            Resources.RuntimeShellPackage_OnAssemblyResolved_ResolvingAssemblyForToolkitAssembly, args.Name);

                                        if (name.KeyPair != null)
                                        {
                                            var publicKeyToken = name.GetPublicKeyTokenString();

                                            // Match latest version by PublicKeyToken
                                            var signedAssemblies = from signedAssembly
                                                           in toolkitAssemblies
                                                                   let assemblyName = signedAssembly.GetName()
                                                                   let assemblyVersion = assemblyName.Version
                                                                   where assemblyName.KeyPair != null
                                                                   where assemblyName.GetPublicKeyTokenString().Equals(publicKeyToken, StringComparison.OrdinalIgnoreCase)
                                                                   orderby (assemblyVersion != null) ? assemblyVersion.ToString(4) : new Version().ToString(4) descending
                                                                   select signedAssembly;
                                            if (signedAssemblies.Any())
                                            {
                                                assembly = signedAssemblies.FirstOrDefault();
                                            }
                                        }
                                        else
                                        {
                                            // Match latest version
                                            assembly = toolkitAssemblies.FirstOrDefault();
                                        }
                                    }
                                }
                            }
                        }

                        if (assembly == null)
                        {
                            tracer.Info(
                            Resources.RuntimeShellPackage_OnAssemblyResolved_ResolvingAssemblyFailed, args.Name);
                        }
                        else
                        {
                            tracer.Info(
                            Resources.RuntimeShellPackage_OnAssemblyResolved_ResolvingAssemblySucceeded, args.Name, assembly.GetName().FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracer.Info(
                    Resources.RuntimeShellPackage_OnAssemblyResolved_UnexpectedError, ex.Message);

                throw;
            }

            return assembly;
        }
#endif
    }
}
