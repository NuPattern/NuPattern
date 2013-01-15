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
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using NuPattern.Extensibility;
using NuPattern.Library;
using NuPattern.Runtime.Shell.OptionPages;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.Runtime.Store;
using NuPattern.Runtime.UI;
using Ole = Microsoft.VisualStudio.OLE.Interop;

namespace NuPattern.Runtime.Shell
{
    /// <summary>
    /// Represents the VS package for this assembly.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed on package dispose.")]
    [ProvideEditorExtension(typeof(ProductStateEditorFactory), NuPattern.Runtime.Constants.RuntimeStoreExtension, 8, DefaultName = NuPattern.Runtime.Constants.RuntimeStoreEditorDescription)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SolutionBuilderToolWindow), Window = ToolWindowGuids.Toolbox, Orientation = ToolWindowOrientation.Right, Style = VsDockStyle.Tabbed)]
    [ProvideDirectiveProcessor(typeof(ProductStateStoreDirectiveProcessor), ProductStateStoreDirectiveProcessor.ProductStateStoreDirectiveProcessorName, Constants.ProductStateStoreDirectiveProcessorDescription)]
    [Microsoft.VisualStudio.Modeling.Shell.ProvideBindingPath]
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
    [ProvideOptionPage(typeof(TraceOptionsPage), Constants.SettingsName, "TraceOptionsPage", 17131, 21356, true)]
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

#if VSVER11
        private ResolveEventHandler assemblyResolve = OnAssemblyResolve;
        [ThreadStatic]
        private static bool _isResolveAssemblyRunningOnThisThread = false;
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
            _isResolveAssemblyRunningOnThisThread = true;
#endif

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
            this.traceOutputWindowManager = new TraceOutputWindowManager(this, this.ShellEvents, OutputPaneGuid, Constants.OutputWindowTitle, sourceNames.ToArray());

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

#if VSVER11
                _isResolveAssemblyRunningOnThisThread = false;
                if (this.assemblyResolve != null)
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= this.assemblyResolve;
                }
#endif
            }
        }

        internal void AutoOpenSolutionBuilder()
        {
            SolutionBuilderToolWindow.AutoOpenWindow(this);
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
            CheckFertInstalled();

            SolutionBuilderToolWindow.InitializeWindowVisibility(this);
        }

        private void OnSolutionClosed(object sender, SolutionEventArgs e)
        {
            SolutionBuilderToolWindow.AutoHideWindow(this);
        }

        private void OnSolutionOpened(object sender, SolutionEventArgs e)
        {
            var pathExpression = string.Concat(@"\*", Runtime.Constants.RuntimeStoreExtension);

            // Ensure solution contains at least one toolkit definition file
            if (e.Solution != null)
            {
                var solutionFiles = e.Solution.Find<IItem>(pathExpression);
                if (solutionFiles.Any())
                {
                    this.AutoOpenSolutionBuilder();
                }
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

        /// <summary>
        /// Checks to see if an older version of the Feature Extension Runtime extension is installed.
        /// </summary>
        /// <remarks>
        /// Cannot tolerate either version of FERT being installed (i.e. FeatureExtensionUltimateRuntime or FeatureExtensionRuntime versions)
        /// </remarks>
        private void CheckFertInstalled()
        {
            var extensionManager = this.GetService<SVsExtensionManager, IVsExtensionManager>();
            if (extensionManager != null)
            {
                var enabledExtensions = extensionManager.GetEnabledExtensions();
                var fertExtension = enabledExtensions.FirstOrDefault(ext =>
                        Constants.FertVsixIdentifiers.Contains(ext.Header.Identifier, StringComparer.OrdinalIgnoreCase));
                if (fertExtension != null)
                {
                    //TODO: trace this to trace window

                    //Prompt user to manuall uninstall the FERT extension
                    this.UserMessageService.ShowWarning(
                        string.Format(CultureInfo.CurrentCulture,
                        Resources.RuntimeShellPackage_CheckFertInstalled_Enabled,
                        fertExtension.Header.Name,
                        Constants.ProductName));
                }
            }
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
            Assembly assembly = null;

            try
            {
                // Only process events from the thread that started it, not any other thread
                if (_isResolveAssemblyRunningOnThisThread)
                {
                    // Determine if lost assembly has a partial name 
                    // (only these are the ones that cause VS2012 difficulty in resolving)
                    var name = new AssemblyName(args.Name);
                    if (name.Version == null || name.CultureInfo == null)
                    {
                        var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();
                        var patternManager = componentModel.GetService<IPatternManager>();
                        if (patternManager != null)
                        {
                            // Get install paths of all (enabled) toolkits
                            var installedExtensionDirs = from installedToolkit
                                                             in patternManager.InstalledToolkits
                                                         where installedToolkit.Extension.State == EnabledState.Enabled
                                                         select new DirectoryInfo(installedToolkit.Extension.InstallPath);

                            // Filter only assemblies installed by toolkits
                            var comparer = new DirectoryInfoComparer();
                            var toolkitAssemblies = from appDomainAssembly
                                                        in ((AppDomain)sender).GetAssemblies()
                                                    where !appDomainAssembly.IsDynamic
                                                    let location = appDomainAssembly.Location
                                                    where !String.IsNullOrEmpty(location)
                                                        && File.Exists(location)
                                                        && installedExtensionDirs.Contains(Directory.GetParent(location), comparer)
                                                    select appDomainAssembly;
                            if (toolkitAssemblies.Any())
                            {
                                // Filter only assemblies matching lost name (by highest version)
                                var matchingAssemblies = from toolkitAssembly
                                                             in toolkitAssemblies
                                                         let assemblyName = toolkitAssembly.GetName()
                                                         let assemblyVersion = assemblyName.Version
                                                         where assemblyName.Name.Equals(args.Name, StringComparison.OrdinalIgnoreCase)
                                                         orderby (assemblyVersion != null) ? assemblyVersion.ToString(4) : new Version().ToString(4) descending
                                                         select toolkitAssembly;
                                if (matchingAssemblies.Any())
                                {
                                    var publicKeyToken = (name.KeyPair == null) ? string.Empty : name.GetPublicKeyTokenString();
                                    if (!String.IsNullOrEmpty(publicKeyToken))
                                    {
                                        // Match latest version by PublicKeyToken
                                        assembly = matchingAssemblies
                                            .Where(a => a.GetName().KeyPair != null)
                                            .FirstOrDefault(a => a.GetName().GetPublicKeyTokenString().Equals(publicKeyToken));
                                    }
                                    else
                                    {
                                        // Match latest version
                                        assembly = matchingAssemblies.FirstOrDefault();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return assembly;
        }
#endif
    }
}