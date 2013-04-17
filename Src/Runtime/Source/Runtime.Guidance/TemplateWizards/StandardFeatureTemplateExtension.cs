using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern;
using NuPattern.ComponentModel.Composition;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Guidance;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution.Templates;
using Construction = Microsoft.Build.Construction;
using Evaluation = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Template extension that automatically instantiates the associated 
    /// feature if it's not already instantiated in the current solution 
    /// (singleton behavior).
    /// </summary>
    [DisplayName("Standard Feature Template Extension")]
    [Description("The wizard template extension used as defaut to unfold a template.")]
    internal class StandardFeatureTemplateExtension : VsBananaTemplateExtension
    {
        private Stack<IFeatureExtension> instantiatingFeatures;
        private ITraceSource tracer;
        private TemplateUnfoldScope unfoldScope;
        private VsTemplateLaunchPoint launchPoint;
        private DefaultTemplateInstantiationScope defaultTemplateScope;
        private IFeatureExtension newFeatureExtension = null;
        private string newFeatureExtensionInstanceName = null;

        protected IVsTemplate VsTemplate { get; private set; }


        public StandardFeatureTemplateExtension()
        {
            instantiatingFeatures = new Stack<IFeatureExtension>();
        }

        protected new IDictionary<string, string> ReplacementsDictionary
        {
            get { return FeatureCallContext.Current.TemplateReplacementsDictionary; }
        }

        [ImportMany]
        private IEnumerable<Lazy<ILaunchPoint, IDictionary<string, object>>> LaunchPoints { get; set; }

        public override void RunFinished()
        {
            //
            // Let's do the magic check to see if the VSSDK is installed when user is trying to instantiate a Feature Builder solution
            //
            if (this.FeatureRegistration.ExtensionManifest.Header.Identifier == "FeatureBuilder")
            {
                string VSSDK100Install = System.Environment.GetEnvironmentVariable("VSSDK100Install");

                if (VSSDK100Install == null || VSSDK100Install == string.Empty)
                {
                    throw new Exception("The Visual Studio SDK must be installed to use the Feature Builder");
                }
                FixupLinks(_dte);
            }

            //
            // If there is a FeatureInstantiationScope active then
            // we got into this template wizard because of an API call
            FeatureManager fm = this.FeatureManager as FeatureManager;
            if (!FeatureInstantiationScope.IsActive)
            {
                try
                {
                    //
                    // If we created a Feature Extension (which we should have) in RunStarted
                    // Here, we initialize it
                    //

                    if (newFeatureExtension != null)
                    {
                        newFeatureExtension.PostInitialize();
                        fm.CompleteInitializationOfUnfoldedFeature(this.FeatureRegistration.FeatureId, newFeatureExtension);
                        fm.ActiveFeature = newFeatureExtension;
                    }

                    base.RunFinished();
                    EnvDTE.Solution dteSolution = Solution.As<EnvDTE.Solution>();
                    dteSolution.SaveAs(Solution.PhysicalPath);

                    if (this.unfoldScope != null)
                        this.unfoldScope.Dispose();

                }
                finally
                {
                    this.ReleaseDefaultTemplateScope();
                }
            }

            fm.InTemplateWizard = fm.InTemplateWizard - 1;
            try
            {
                this.AfterUnfold();
            }
            catch (Exception e)
            {
                throw new TemplateCommandFailedException(e);
            }
        }

        public override void RunStarted(
            object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind,
            object[] customParams)
        {
            this.tracer = Tracer.GetSourceFor<StandardFeatureTemplateExtension>();
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);
            FeatureManager fm = this.FeatureManager as FeatureManager;
            try
            {
                this._dte = (EnvDTE.DTE)automationObject;

                var serviceProvider = new ServiceProvider((OLE.Interop.IServiceProvider)this._dte);
                var uiShell = serviceProvider.GetService<IVsUIShell>();

                //
                // Check to see if the VSSDK is installed when user is trying to instantiate a Feature Builder solution
                //
                if (this.FeatureRegistration.ExtensionManifest.Header.Identifier == "FeatureBuilder")
                {
                    string VSSDK100Install = System.Environment.GetEnvironmentVariable("VSSDK100Install");

                    if (VSSDK100Install == null || VSSDK100Install == string.Empty)
                    {
                        uiShell.ShowError(null, Resources.StandardFeatureTemplateExtension_VSSDKNotInstalled);
                        throw new Exception(Resources.StandardFeatureTemplateExtension_ErrorVSSDKNotInstalled);
                    }

                }

                this.tracer = FeatureTracer.GetSourceFor<StandardFeatureTemplateExtension>(this.FeatureRegistration.FeatureId);

                var previousUnfoldScopeActive = TemplateUnfoldScope.IsActive;
                this.unfoldScope = new TemplateUnfoldScope(replacementsDictionary);
                this.tracer.TraceVerbose("Reading vstemplate file");
                this.VsTemplate = VsTemplateFile.Read(this.TemplateFile);
                this.launchPoint = this.LaunchPoints.FromFeaturesCatalog().Find(this.VsTemplate.TemplateData);

                this.SetDefaultTemplateScopeIfNeeded();

                if (!FeatureInstantiationScope.IsActive &&
                    (!this.FeatureManager.IsInstantiated(this.FeatureRegistration) ||
                    this.IsMultipleInstance(previousUnfoldScopeActive)))
                {
                    this.tracer.TraceVerbose("Existing feature of the same type was not found in the current solution. Instantiating a new one.");
                    //
                    // Note:  This will Instantiate but NOT Initialize this feature extension.
                    // That must be done in RunFinished
                    //


                    //
                    // First, create the instance name
                    //
                    newFeatureExtensionInstanceName = this.GenerateFeatureInstanceName();

                    //
                    // Then create the feature instance, saving the reference
                    // for use in RunFinished
                    //
                    newFeatureExtension = fm.InstantiateButNotInitialize(this.FeatureRegistration.FeatureId, newFeatureExtensionInstanceName);

                    if (replacementsDictionary.ContainsKey("$featurename$"))
                        replacementsDictionary.Remove("$featurename$");

                    replacementsDictionary.Add("$featurename$", newFeatureExtensionInstanceName);
                }
                else
                {
                    tracer.TraceVerbose("Found existing feature of the same type in the current solution. Skipping feature instantiation.");
                }

                if (launchPoint != null)
                {
                    this.tracer.TraceVerbose(
                        "Associated template launch point found for template '{0}'. Its Before and After logic will be executed.",
                        Path.GetFileName(this.TemplateFile));
                }
                else
                {
                    this.tracer.TraceVerbose(
                        "No associated template launch point found for template '{0}'.",
                        Path.GetFileName(this.TemplateFile));
                }

                try
                {
                    this.BeforeUnfold();
                }
                catch (Exception e)
                {
                    fm.InTemplateWizard = 0;
                    this.tracer.TraceError(e, "Before Unfold threw error");
                    this.ReleaseDefaultTemplateScope();
                    throw new TemplateCommandFailedException(e);
                }

                //
                // Tell the Feature Manager we're expanding a template so that it doesn't
                // respond to VS events that say the solution is "closed" which seem to come
                // after we've really started to create the new solution
                //
                fm.InTemplateWizard = fm.InTemplateWizard + 1;
            }
            catch (Exception e)
            {
                this.tracer.TraceError(e, "Template could not be unfolded");
                this.ReleaseDefaultTemplateScope();
                fm.InTemplateWizard = 0;
            }
        }

        private void SetDefaultTemplateScopeIfNeeded()
        {
            if (this.launchPoint != null && this.launchPoint.IsDefaultTemplate)
            {
                this.defaultTemplateScope = new DefaultTemplateInstantiationScope();
            }
        }

        /// <summary>
        /// Derived templates can provide additional behavior before unfolding a 
        /// template in this method, to affect the token replacements dictionary passed to 
        /// the template unfolding process.
        /// </summary>
        protected virtual void BeforeUnfold()
        {
            if (this.launchPoint != null)
            {
                this.tracer.TraceVerbose("Executing BeforeUnfold template action.");
                this.launchPoint.OnBeforeUnfoldTemplate();
            }
        }

        /// <summary>
        /// Derived templates can provide additional behavior after unfolding a 
        /// template in this method.
        /// </summary>
        protected virtual void AfterUnfold()
        {
            if (this.launchPoint != null)
            {
                this.tracer.TraceVerbose("Executing AfterUnfold template action.");
                this.launchPoint.OnAfterUnfoldTemplate();
            }
        }

        protected virtual string GenerateFeatureInstanceName()
        {
            int index = 1;
            var baseProjectName = string.Format(
                CultureInfo.InvariantCulture,
                this.FeatureRegistration.DefaultName,
                this.TemplateParameters.SafeProjectName);
            var instanceName = baseProjectName;

            while (this.FeatureManager.InstantiatedFeatures.Any(
                f => f.InstanceName.Equals(instanceName, StringComparison.OrdinalIgnoreCase)))
            {
                instanceName = baseProjectName + index++;
            }

            return instanceName;
        }

        private bool IsMultipleInstance(bool previousUnfoldScopeActive)
        {
            return this.launchPoint != null &&
                this.launchPoint.FeatureInstantiation == FeatureInstantiation.MultipleInstance &&
                !previousUnfoldScopeActive;
        }

        private void ReleaseDefaultTemplateScope()
        {
            if (this.launchPoint != null && this.launchPoint.IsDefaultTemplate && this.defaultTemplateScope != null)
            {
                this.defaultTemplateScope.Dispose();
            }
        }

        private EnvDTE.DTE _dte;

        private const string Link = "Link";
        private const string FixedLink = "FeatureBuilderFixupLink";


        private void FixupLinks(EnvDTE.DTE dte)
        {
            using (var serviceProvider = new ServiceProvider((OLE.Interop.IServiceProvider)dte))
            {
                tracer.Shield(() =>
                {
                    var projects = dte.Solution.Projects
                        .OfType<Project>()
                        .Where(p => !string.IsNullOrEmpty(p.FullName));

                    foreach (var project in projects)
                    {
                        var fileInfo = new FileInfo(project.FullName);
                        var projectBuilder = Construction.ProjectRootElement.Open(fileInfo.FullName);
                        var projectEvaluated = Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects
                            .FirstOrDefault<Evaluation.Project>(
                                p => p.FullPath.Equals(fileInfo.FullName, StringComparison.InvariantCultureIgnoreCase));

                        if (projectBuilder != null && projectEvaluated != null)
                        {
                            var items = projectBuilder.Items.Where(i =>
                                                                   i.Metadata.Any(m => m.Name == Link) &&
                                                                   i.Metadata.Any(m => m.Name == FixedLink));

                            if (items.Any())
                            {
                                // Suspend project file change notifications
                                // var fileChange = serviceProvider.GetService<SVsFileChangeEx, IVsFileChangeEx>();
                                var fileChange = (IVsFileChangeEx)serviceProvider.GetService(typeof(SVsFileChangeEx));
                                uint cookie = 0;

                                if (fileChange != null)
                                {
                                    ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(cookie, fileInfo.FullName, 1));
                                }

                                foreach (var item in items)
                                {
                                    // Ensure we have a fixed link property
                                    var fixedLink = item.Metadata.First(m => m.Name == FixedLink);
                                    if (fixedLink != null
                                        && !String.IsNullOrEmpty(fixedLink.Value))
                                    {
                                        // Ensure we have an Include attribute
                                        if (!string.IsNullOrEmpty(item.Include))
                                        {
                                            item.Include = fixedLink.Value;
                                            fixedLink.Value = string.Empty;
                                        }
                                    }
                                }

                                projectBuilder.Save();

                                // Resume project file change notifications
                                if (fileChange != null)
                                {
                                    ErrorHandler.ThrowOnFailure(fileChange.SyncFile(fileInfo.FullName));
                                    ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(cookie, fileInfo.FullName, 0));

                                    project.ReloadProject();
                                }
                            }
                        }
                    }
                }, "Fixup Link TemplateWizard: Failed To Fix Link");
            }
        }
    }
}