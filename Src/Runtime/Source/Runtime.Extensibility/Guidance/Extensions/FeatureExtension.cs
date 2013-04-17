using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.Guidance.Diagnostics;
using NuPattern.Runtime.Guidance.LaunchPoints;
using NuPattern.Runtime.Guidance.Workflow;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio.Solution;
using NuPattern.VisualStudio.Solution.Templates;

namespace NuPattern.Runtime.Guidance.Extensions
{
    /// <summary>
    /// Default implementation of <see cref="IFeatureExtension"/>.
    /// </summary>
    /// <remarks>
    /// Features can declare imports and exports as any other 
    /// MEF component. They will be instantiated using 
    /// the PowerTools catalog.
    /// </remarks>
    public abstract class FeatureExtension : IFeatureExtension
    {
        private IFeatureManager featureManager;

        /// <summary>
        /// Gets the feature manager
        /// </summary>
        public IFeatureManager FeatureManager { get { return featureManager; } }

        /// <summary>
        /// Gets the commands included in this feature.
        /// </summary>
        public abstract IEnumerable<ICommandBinding> Commands { get; }

        /// <summary>
        /// Gets the feature type identifier.
        /// </summary>
        public virtual string FeatureId { get; private set; }

        /// <summary>
        /// Gets the process guidance workflow instance.
        /// </summary>
        public virtual IGuidanceWorkflow GuidanceWorkflow { get; set; }

        /// <summary>
        /// Gets the name of a concrete instance of a feature.
        /// </summary>
        public virtual string InstanceName { get; private set; }

        /// <summary>
        /// Gets the flag which indicates if the Blackboard
        /// state is persisted
        /// </summary>
        public virtual bool PersistStateInSolution { get { return true; } }

        /// <summary>
        /// Gets the flag which indicates if the instantiation of
        /// this feature is stored in the Solution and thus is
        /// re-initialize when the solution is re-opened.
        /// </summary>
        public virtual bool PersistInstanceInSolution { get { return true; } }

        /// <summary>
        /// Gets the composition service for the feature.
        /// </summary>
        [Import]
        public virtual IFeatureCompositionService FeatureComposition { get; private set; }

        /// <summary>
        /// Gets the tracer for this component. Should not be used by other components, which 
        /// should get their own tracer.
        /// </summary>
        /// <remarks>
        /// See also <see cref="FeatureTracer"/>.
        /// </remarks>
        protected internal virtual ITraceSource Trace { get; private set; }

        /// <summary>
        /// Gets the registration of this feature.
        /// </summary>
        protected virtual IFeatureRegistration Registration { get; private set; }

        /// <summary>
        /// Gets the solution which this feature was unfolded.
        /// </summary>
        [Import]
        protected virtual ISolution Solution { get; private set; }

        /// <summary>
        /// Gets the template service.
        /// </summary>
        [Import]
        protected virtual IFxrTemplateService Templates { get; private set; }

        /// <summary>
        /// Instantiates the feature in a solution.
        /// </summary>
        /// <param name="registration">The feature registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="featureManager">An instance of the feature manager</param>
        /// <remarks>
        /// This method is called when a feature is first instantiated in a solution.
        /// </remarks>
        public void Instantiate(IFeatureRegistration registration, string instanceName, IFeatureManager featureManager)
        {
            using (new FeatureInstantiationScope())
            {
                this.FeatureId = registration.FeatureId;
                this.featureManager = featureManager;
                this.InstanceName = instanceName;
                this.Registration = registration;
                this.Trace = FeatureTracer.GetSourceFor(this, registration.FeatureId, instanceName);

                this.OnInstantiate();

                if (!DefaultTemplateInstantiationScope.IsActive)
                {
                    var launchPoint = this.FindLaunchPoint();
                    if (launchPoint != null)
                    {
                        // There is an issue in all versions of VS up to and including VS2008 SP1
                        // that won't allow the expansion of a multi-project template unless the Solution node in the Solution explorer
                        // is selected.  The code below works for any solution with less than 100,000 items
                        this.Solution.SelectUp();

                        this.UnfoldDefaultTemplate(launchPoint);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the feature.
        /// </summary>
        /// <param name="registration">The feature registration information, which includes its identifier and manifest information..</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="version">The version of hte feature to intialize</param>
        /// <param name="featureManager">An instance of the feature manager</param>
        /// <remarks>
        /// This method is called after <see cref="Instantiate"/>
        /// when a feature is first initialized in a solution,
        /// or after reopening a solution where the feature
        /// had been previously instantiated.
        /// </remarks>
        public void Initialize(IFeatureRegistration registration, string instanceName, Version version, IFeatureManager featureManager)
        {
            this.FeatureId = registration.FeatureId;
            this.InstanceName = instanceName;
            this.Registration = registration;
            //
            // note we do this here and in Instantiate because when re-opening a solution, we never call Instantiate
            //
            this.featureManager = featureManager;

            this.Trace = FeatureTracer.GetSourceFor(this, registration.FeatureId, instanceName);

            this.GuidanceWorkflow = this.CreateWorkflow();
            if (this.GuidanceWorkflow != null)
                this.GuidanceWorkflow.OwningFeature = this;

            this.OnInitialize(version);
        }

        /// <summary>
        /// Called after the extension is initialized
        /// </summary>
        public void PostInitialize()
        {
            this.OnPostInitialize();
        }

        /// <summary>
        /// Finishes the feature.
        /// </summary>
        public void Finish()
        {
            this.OnFinish();
        }

        /// <summary>
        /// Creates the guidance workflow to assign to the <see cref="GuidanceWorkflow"/> property.
        /// </summary>
        public abstract IGuidanceWorkflow CreateWorkflow();

        /// <summary>
        /// When overriden by a derived class, implements the feature instantiation behavior.
        /// </summary>
        protected internal virtual void OnInstantiate()
        {
        }

        /// <summary>
        /// When overriden by a derived class, implements the feature instantiation behavior.
        /// </summary>
        protected internal virtual void OnInitialize(Version persistedVersion)
        {
        }

        /// <summary>
        /// When overriden by a derived class, 
        /// can determine if the Blackboard is persistent.
        /// </summary>
        protected internal virtual void OnPostInitialize()
        {
        }

        /// <summary>
        /// When overriden by a derived class, implements the feature instantiation behavior.
        /// </summary>
        protected internal virtual void OnFinish()
        {
        }

        private string GenerateProjectName(string directory, string baseProjectName)
        {
            var projectName = baseProjectName;
            var path = Path.Combine(directory, projectName);

            var index = 1;
            while (this.Solution.Find<IProject>(projectName).Any())
            {
                projectName = baseProjectName + index++;
                path = Path.Combine(directory, projectName);
            }

            return projectName;
        }

        private VsTemplateLaunchPoint FindLaunchPoint()
        {
            return this.FeatureComposition
                .GetExports<ILaunchPoint, IFeatureComponentMetadata>()
                .FromFeaturesCatalog()
                .Where(export => export.Metadata.ExportingType.Assembly.Location.StartsWith(
                    this.Registration.InstallPath,
                    StringComparison.OrdinalIgnoreCase))
                .Select(export => export.Value)
                .OfType<VsTemplateLaunchPoint>()
                .FirstOrDefault(lp => lp.IsDefaultTemplate);
        }

        private void ThrowIfNotFeatureTemplate(ITemplate template)
        {
            if (template == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.FeatureExtension_NoFeatureProjectTemplate,
                    this.FeatureId));
            }
        }

        private void UnfoldDefaultTemplate(VsTemplateLaunchPoint launchPoint)
        {
            var templatePath = this.Solution.GetTemplatePath(launchPoint);
            var vsTemplate = VsTemplateFile.Read(templatePath);

            var template = this.Templates.Find(launchPoint);
            this.ThrowIfNotFeatureTemplate(template);

            var baseProjectName = vsTemplate.TemplateData.DefaultName;
            var currentSelection = this.Solution.GetSelection().FirstOrDefault();
            var solutionFolder = currentSelection as ISolutionFolder;
            if (solutionFolder != null)
            {
                solutionFolder.Add(GenerateProjectName(solutionFolder.PhysicalPath, baseProjectName), template);
            }
            else
            {
                this.Solution.Add(GenerateProjectName(this.Solution.PhysicalPath, baseProjectName), template);
            }

            if (currentSelection != null)
            {
                currentSelection.Select();
            }
            else
            {
                this.Solution.Select();
            }
        }
    }
}
