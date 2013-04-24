using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using NuPattern.Diagnostics;
using NuPattern.Reflection;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Guidance.Extensions;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Manages guidance workflows and their lifetime.
    /// </summary>
    [Export(typeof(IGuidanceManager))]
    internal class GuidanceManager : IGuidanceManager
    {
        private static readonly ITracer tracer = Tracer.Get<GuidanceManager>();

        private const string ExtensionsDir = @"Extensions";
        private const string ExtensionsFileFilter = @"*.vsixmanifest";
        private const string ExtensionManifestFileName = @"source.extension.vsixmanifest";
        private const string CommandsExtension = @"*.commands";
        private CompositionContainer extensionsGlobalContainer;
        private Func<IGuidanceExtensionRegistration, IGuidanceExtension> extensionFactory;
        private List<Tuple<IGuidanceExtension, INuPatternCompositionService>> instantiatedGuidanceExtensions = new List<Tuple<IGuidanceExtension, INuPatternCompositionService>>();
        private ISolutionState solutionState;
        private IGuidanceExtension activeExtension;
        private static string FeatureBuilderDSLPath = string.Empty;

#pragma warning disable 0414
        private int inTemplateWizard = 0;
#pragma warning restore 0414

        /// <summary>
        /// Gets the InTemplateWzard
        /// </summary>
        public int InTemplateWizard { get; set; }

        /// <summary>
        /// Raises when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        /// <summary>
        /// Raised when the instantiated extension is changed.
        /// </summary>
        public event EventHandler InstantiatedExtensionsChanged = (sender, args) => { };

        /// <summary>
        /// Raised when the state is opened.
        /// </summary>
        public event EventHandler IsOpenedChanged = (sender, args) => { };

        /// <summary>
        /// Raised when the active extension is changed.
        /// </summary>
        public event EventHandler ActiveExtensionChanged = (sender, args) => { };

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public GuidanceManager(
            [Import(typeof(NuPatternGlobalContainer))]
            CompositionContainer extensionsGlobalContainer,
            IEnumerable<IGuidanceExtensionRegistration> installedExtensions,
            Func<IGuidanceExtensionRegistration, IGuidanceExtension> extensionFactory)
        {
            // Not sure it still makes much sense to have the Func factory abstraction 
            // over MEF if we are dependent on a global container anyways, and 
            // now the manager is fully aware of MEF as it creates child 
            // containers for extensions...

            Guard.NotNull(() => extensionsGlobalContainer, extensionsGlobalContainer);
            Guard.NotNull(() => installedExtensions, installedExtensions);
            Guard.NotNull(() => extensionFactory, extensionFactory);

            this.extensionsGlobalContainer = extensionsGlobalContainer;
            this.InstalledGuidanceExtensions = installedExtensions.ToList();
            this.extensionFactory = extensionFactory;
        }

        /// <summary>
        /// Installed guidance extensions.
        /// </summary>
        /// <value></value>
        public IEnumerable<IGuidanceExtensionRegistration> InstalledGuidanceExtensions { get; private set; }

        /// <summary>
        /// Whether the manager has been initialized for a solution.
        /// See <see cref="Open(ISolutionState)"/>.
        /// </summary>
        /// <value></value>
        public bool IsOpened
        {
            get { return solutionState != null; }
        }

        /// <summary>
        /// Gets or sets the active guidance extension.
        /// </summary>
        public IGuidanceExtension ActiveGuidanceExtension
        {
            get
            {
                return this.activeExtension;
            }
            set
            {
                if (value != this.activeExtension)
                {
                    this.activeExtension = value;
                    this.ActiveExtensionChanged(this, EventArgs.Empty);
                    this.RaisePropertyChanged(x => x.ActiveGuidanceExtension);
                }
            }
        }

        /// <summary>
        /// Force the active guidance extension to change causing the workflow to refresh
        /// </summary>
        public void ForceActiveExtensionChanged()
        {
            this.ActiveExtensionChanged(this, EventArgs.Empty);
            this.RaisePropertyChanged(x => x.ActiveGuidanceExtension);
        }

        /// <summary>
        /// Instantiated extensions in the initialized solution state, if any.
        /// </summary>
        /// <value></value>
        public IEnumerable<IGuidanceExtension> InstantiatedGuidanceExtensions
        {
            get { return this.instantiatedGuidanceExtensions.Select(tuple => tuple.Item1); }
        }

        /// <summary>
        /// Release state and resources for all instantiated guidance extensions in the current solution state.
        /// </summary>
        public void Close()
        {
            //
            // If we're really in the RunStarted portion of a template wizard, we ignore
            // Close calls which are generated by VS events
            //
            if (InTemplateWizard > 0)
                return;

            ClearAllExtensions();

            this.solutionState = null;
            this.ActiveGuidanceExtension = null;

            if (BlackboardManager.Current != null)
                BlackboardManager.Current.Clear();

            InstantiatedExtensionsChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);
            IsOpenedChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.IsOpened);
        }

        /// <summary>
        /// Initializes the manager with a given solution state.
        /// </summary>
        public void Open(ISolutionState solutionState)
        {
            Guard.NotNull(() => solutionState, solutionState);

            var theSolution = ((SolutionDataState)solutionState).Solution;
            SetDslPathIfPresent();

            //
            // Ok, now let's load any guidance extensions we find from the "Solution State" which is
            // stored in the .SLN file's Solution Globals
            //
            this.Close();
            this.solutionState = solutionState;

            IsOpenedChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.IsOpened);


            using (tracer.StartActivity(Resources.GuidanceManager_TraceOpenFromSolutionState))
            {
                // Ignore guidance extensions that are not installed.
                var availableSolutionExtensions = solutionState
                    .InstantiatedGuidanceExtensions
                    .Where(state => this.IsInstalled(state.ExtensionId))
                    .Select(state => new
                    {
                        Registration = this.InstalledGuidanceExtensions.First(reg => reg.ExtensionId == state.ExtensionId),
                        State = state
                    });

                try
                {
                    if (availableSolutionExtensions.ToArray<object>().Length > 0)
                    {
                        InTemplateWizard = InTemplateWizard + 1;
                        ForceActiveExtensionChanged();
                        InTemplateWizard = InTemplateWizard - 1;
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    InTemplateWizard = 0;
                }

                if (BlackboardManager.Current != null)
                    BlackboardManager.Current.Initialize();

                foreach (var extension in availableSolutionExtensions)
                {
                    using (tracer.StartActivity(Resources.GuidanceManager_TraceInitializeExtension, extension.State.InstanceName, extension.State.ExtensionId))
                    {
                        try
                        {
                            var ext = this.InitializeExtension(extension.State.ExtensionId,
                                extension.State.InstanceName,
                                extension.Registration,
                                extension.State.Version,
                                false);  // isInstantiation
                            ext.PostInitialize();
                            this.ActiveGuidanceExtension = ext;

                            // If extension initialized fine and there was a version mismatch, consider it upgraded.
                            if (extension.State.Version != extension.Registration.ExtensionManifest.Header.Version)
                                solutionState.Update(extension.State.ExtensionId, extension.State.InstanceName, extension.Registration.ExtensionManifest.Header.Version);
                        }
                        catch (Exception ex)
                        {
                            tracer.Error(ex, Resources.GuidanceManager_TraceFailedExtensionInitialization);
                        }
                    }
                }

                if (this.instantiatedGuidanceExtensions.Count == 1)
                    this.ActiveGuidanceExtension = this.InstantiatedGuidanceExtensions.First();
            }
        }

        /// <summary>
        /// Instantiates the given extension in the current solution.
        /// </summary>
        /// <param name="extensionId">Identifier for the extension type.</param>
        /// <param name="instanceName">Name to assign to the newly created extension instance.</param>
        /// <returns>The instantiated extension.</returns>
        public IGuidanceExtension Instantiate(string extensionId, string instanceName)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            using (tracer.StartActivity(Resources.GuidanceManager_TraceInstantiateExtension, instanceName, extensionId))
            {
                try
                {
                    //this.ThrowIfNoSolutionState();
                    this.ThrowIfExtensionNotInstalled(extensionId);
                    this.ThrowIfAlreadyInstantiated(extensionId, instanceName);

                    // Gets the installed version of the extension
                    var registration = this.InstalledGuidanceExtensions.First(f => f.ExtensionId == extensionId);

                    var extension = this.InitializeExtension(extensionId,
                        instanceName,
                        registration,
                        registration.ExtensionManifest.Header.Version,
                        true);

                    if (registration != null && extension != null)
                    {
                        extension = InitializeExtension(registration.ExtensionId,
                            instanceName,
                            registration,
                            registration.ExtensionManifest.Header.Version,
                            false);
                        extension.PostInitialize();

                        //
                        // Finally, find the instance and fixup the workflow
                        //
                        foreach (var x in instantiatedGuidanceExtensions.ToList())
                        {
                            if (x.Item1.InstanceName == extension.InstanceName)
                            {
                                GuidanceExtension fx = x.Item1 as GuidanceExtension;
                                fx.GuidanceWorkflow = extension.GuidanceWorkflow;
                                break;
                            }
                        }
                        //
                        // Update the InstantiatedExtensions list which will get the workflow into the
                        // Workflow Explorer
                        InstantiatedExtensionsChanged(this, EventArgs.Empty);
                        RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);
                    }
                    return extension;
                }
                catch (Exception ex)
                {
                    tracer.Error(ex, Resources.GuidanceManager_TraceFailedInstantiate);
                    throw;
                }
            }
        }

        internal IGuidanceExtension InstantiateButNotInitialize(string extensionId, string instanceName)
        {
            Guard.NotNullOrEmpty(() => extensionId, extensionId);
            Guard.NotNullOrEmpty(() => instanceName, instanceName);

            using (tracer.StartActivity(Resources.GuidanceManager_TraceInstantiateExtension, instanceName, extensionId))
            {
                try
                {
                    //this.ThrowIfNoSolutionState();
                    this.ThrowIfExtensionNotInstalled(extensionId);
                    this.ThrowIfAlreadyInstantiated(extensionId, instanceName);

                    // Gets the installed version of the extension
                    var registration = this.InstalledGuidanceExtensions.First(f => f.ExtensionId == extensionId);

                    var extension = this.InitializeExtension(extensionId,
                        instanceName,
                        registration,
                        registration.ExtensionManifest.Header.Version,
                        true);

                    if (registration != null && extension != null)
                    {
                        extension = InitializeExtension(registration.ExtensionId,
                            instanceName,
                            registration,
                            registration.ExtensionManifest.Header.Version,
                            false);
                    }

                    return extension;
                }
                catch (Exception ex)
                {
                    tracer.Error(ex, Resources.GuidanceManager_TraceFailedInstantiate);
                    throw;
                }
            }
        }

        /// <summary>
        /// Initiailizes the given extension
        /// </summary>
        /// <remarks>Semi-public version which allows control over call to postInitialize</remarks>
        protected IGuidanceExtension InitializeExtension(string extensionId,
            string instanceName,
            IGuidanceExtensionRegistration registration,
            Version version,
            bool isInstantiation
            )
        {
            var extensionInstance = this.CreateExtension(extensionId);
            var composition = this.CreateCompositionService(extensionInstance, extensionId);

            SetDslPathIfPresent();

            //
            // Let's check to see if the instantiation should be registered with the
            // solution
            if (extensionInstance.PersistInstanceInSolution)
            {
                solutionState.AddExtension(extensionId, instanceName, version);
            }

            if (isInstantiation)
            {
                extensionInstance.Instantiate(registration, instanceName, this);
            }
            else
            {
                extensionInstance.Initialize(registration, instanceName, version, this);
                instantiatedGuidanceExtensions.Add(new Tuple<IGuidanceExtension, INuPatternCompositionService>(extensionInstance, composition));
                InstantiatedExtensionsChanged(this, EventArgs.Empty);
                RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);
            }

            return extensionInstance;
        }

        internal void CompleteInitializationOfUnfoldedExtension(string extensionId,
            IGuidanceExtension extensionInstance)
        {
            //
            // First, find the instance and fixup the workflow
            //
            foreach (var x in instantiatedGuidanceExtensions.ToList())
            {
                if (x.Item1.InstanceName == extensionInstance.InstanceName)
                {
                    GuidanceExtension fx = x.Item1 as GuidanceExtension;
                    fx.GuidanceWorkflow = extensionInstance.GuidanceWorkflow;
                    break;
                }
            }

            //
            // Then add this FX to the solution (if desired)
            //
            if (extensionInstance.PersistInstanceInSolution)
            {
                var registration = this.InstalledGuidanceExtensions.First(f => f.ExtensionId == extensionId);
                solutionState.AddExtension(extensionId, extensionInstance.InstanceName, registration.ExtensionManifest.Header.Version);
            }

            //
            // Update the InstantiatedExtensions list which will get the workflow into the
            // Workflow Explorer
            InstantiatedExtensionsChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);
        }

        private void RaisePropertyChanged(Expression<Func<IGuidanceManager, object>> propertyExpresion)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(
                Reflect<IGuidanceManager>.GetProperty(propertyExpresion).Name));
        }

        private INuPatternCompositionService CreateCompositionService(IGuidanceExtension extension, string extensionId)
        {
            var extensionContainer = new CompositionContainer(extensionsGlobalContainer);

            // Expose IGuidanceExtension to the container
            extensionContainer.ComposeExportedValue(extension);
            extensionContainer.ComposeExportedValue(InstalledGuidanceExtensions.First(r => r.ExtensionId == extensionId));

            extensionContainer.ComposeExportedValue<ExportProvider>(extensionContainer);

            var compositionService = new NuPatternCompositionService(extensionContainer);
            // Expose IGuidanceExtensionCompositionService to the container
            extensionContainer.ComposeExportedValue<INuPatternCompositionService>(compositionService);

            // Satisfy imports at this level, so that the right guidance extension-level stuff is injected instead 
            // (i.e. the guidance extension might have specified an import of the IGudianceExtensionCompositionService and 
            // would have gotten the global one.
            compositionService.SatisfyImportsOnce(extension);

            return compositionService;
        }

        /// <summary>
        /// Refreshes the guidance workflow
        /// </summary>
        /// <param name="extension"></param>
        public void RefreshWorkflow(IGuidanceExtension extension)
        {
            var realExt = extension as GuidanceExtension;

            //
            // First, remove our guidance extension from the instantiated list
            // and raise the event (which will cause it to be removed from the UX)
            //
            extension.Finish();
            instantiatedGuidanceExtensions.RemoveAll(state => state.Item1.ExtensionId == extension.ExtensionId && state.Item1.InstanceName == extension.InstanceName);
            InstantiatedExtensionsChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);

            realExt.GuidanceWorkflow = realExt.CreateWorkflow();
            if (realExt.GuidanceWorkflow != null)
                realExt.GuidanceWorkflow.OwningExtension = extension;

            //
            // Now put it back and make it active
            //
            instantiatedGuidanceExtensions.Add(new Tuple<IGuidanceExtension, INuPatternCompositionService>(extension, realExt.GuidanceComposition));
            InstantiatedExtensionsChanged(this, EventArgs.Empty);
            RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);
            this.ActiveGuidanceExtension = extension;
        }

        /// <summary>
        /// Removes the given guidance workflow
        /// </summary>
        /// <param name="extension"></param>
        public void RemoveWorkflow(IGuidanceExtension extension)
        {
            extension.Finish();
            instantiatedGuidanceExtensions.RemoveAll(state => state.Item1.ExtensionId == extension.ExtensionId && state.Item1.InstanceName == extension.InstanceName);
        }

        /// <summary>
        /// Finishes the given guidance extension.
        /// </summary>
        /// <param name="instanceName">Name of the guidance extension instance to finish.</param>
        public void Finish(string instanceName)
        {
            //ThrowIfNoSolutionState();

            var extension = instantiatedGuidanceExtensions.FirstOrDefault(f => f.Item1.InstanceName == instanceName);
            if (extension != null)
            {
                using (tracer.StartActivity(Resources.GuidanceManager_TraceFinishExtension, instanceName, extension.Item1.ExtensionId))
                {
                    try
                    {
                        extension.Item1.Finish();
                    }
                    catch (Exception ex)
                    {
                        tracer.Warn(Resources.GuidanceManager_TraceFailedFinish, ex);
                    }

                    var disposable = extension.Item1 as IDisposable;
                    if (disposable != null)
                        disposable.Dispose();

                    solutionState.RemoveExtension(extension.Item1.ExtensionId, instanceName);

                    InstantiatedExtensionsChanged(this, EventArgs.Empty);
                    RaisePropertyChanged(x => x.InstantiatedGuidanceExtensions);

                    tracer.Info(Resources.GuidanceManager_TraceFinishCOmplete);
                }
            }
        }

        private IGuidanceExtension CreateExtension(string extensionId)
        {
            return this.extensionFactory(this.InstalledGuidanceExtensions.First(r => r.ExtensionId == extensionId));
        }

        private void ClearAllExtensions()
        {
            foreach (var extension in instantiatedGuidanceExtensions)
            {
                var disposable = extension.Item1 as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            instantiatedGuidanceExtensions.Clear();
        }

        private void ThrowIfNoSolutionState()
        {
            if (this.solutionState == null)
            {
                throw new InvalidOperationException(Resources.GuidanceManager_MustInitializeSolutionState);
            }
        }

        private void ThrowIfExtensionNotInstalled(string extensionId)
        {
            if (!this.InstalledGuidanceExtensions.Any(r => r.ExtensionId == extensionId))
            {
                throw new NotSupportedException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.GuidanceManager_ExtensionNotInstalled,
                    extensionId));
            }
        }

        private void ThrowIfAlreadyInstantiated(string extensionId, string instanceName)
        {
            if (this.instantiatedGuidanceExtensions.Any(tuple => tuple.Item1.InstanceName == instanceName))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.GuidanceManager_ExtensionAlreadyInstantiated,
                    extensionId,
                    instanceName));
            }
        }

        /// <summary>
        /// Finds the installed extension associated with the file
        /// </summary>
        /// <param name="filename"></param>
        public IGuidanceExtensionRegistration FindGuidanceExtension(string filename)
        {
            string extensionId = null;

            var currentDir = Path.GetDirectoryName(filename);
            while (string.IsNullOrEmpty(extensionId) && !string.IsNullOrEmpty(currentDir) && Path.GetFileName(currentDir) != ExtensionsDir)
            {
                var manifestFile = Directory.EnumerateFiles(currentDir, ExtensionsFileFilter, SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (manifestFile != null && !manifestFile.ToLower().EndsWith(ExtensionManifestFileName))
                {
                    extensionId = ReadVsixIdentifier(manifestFile);
                    if (string.IsNullOrEmpty(extensionId))
                        return null;
                }
                else
                {
                    // When it's already a folder, it removes the parent path.
                    currentDir = Path.GetDirectoryName(currentDir);
                }
            }

            return this.InstalledGuidanceExtensions.FirstOrDefault(extension => extension.ExtensionId == extensionId);
        }

        private string ReadVsixIdentifier(string vsixManifest)
        {
            using (var reader = XmlReader.Create(vsixManifest))
            {
                reader.MoveToContent();

                if (reader.ReadToDescendant("Identifier", "http://schemas.microsoft.com/developer/vsx-schema/2010"))
                {
                    return reader.GetAttribute("Id");
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the installed extension associated with the type
        /// </summary>
        public IGuidanceExtensionRegistration FindGuidanceExtension(Type type)
        {
            if (type == null)
            {
                return null;
            }

            var extensionAttribute = type.Assembly.GetCustomAttribute<GuidanceExtensionAttribute>(true);
            if (extensionAttribute != null)
            {
                return this.InstalledGuidanceExtensions.FirstOrDefault(extension => extension.ExtensionId == extensionAttribute.ExtensionId);
            }

            return this.FindGuidanceExtension(type.Assembly.Location);
        }


        /// <summary>
        /// Returns the first installed extension when the predicate function returns true
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IGuidanceExtensionRegistration FindGuidanceExtension(Func<IGuidanceExtensionRegistration, bool> predicate)
        {
            return this.InstalledGuidanceExtensions
                .FirstOrDefault(predicate);
        }

        /// <summary>
        /// If this a Feature Builder Solution,
        /// save the absolute path to the Feature.command file into the
        /// FeatureCallContext so we can use it in the IElementExtensions and in the
        /// GuidanceWorkflow.t4
        /// </summary>
        public void SetDslPathIfPresent()
        {
            if (solutionState != null)
            {
                ISolution theSolution = ((SolutionDataState)solutionState).Solution;
                var dslFile = theSolution.Find(CommandsExtension).FirstOrDefault();
                if (dslFile != null)
                    FeatureBuilderDSLPath = dslFile.PhysicalPath;
                else
                    FeatureBuilderDSLPath = null;
            }
        }

    }
}
