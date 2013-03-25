using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TemplateWizard;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.References;
using NuPattern.Runtime.TemplateWizards;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Wizard extension that creates a pattern instance automatically when 
    /// the template is unfolded.
    /// </summary>
    [CLSCompliant(false)]
    public sealed class InstantiationTemplateWizard : TemplateWizard
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<InstantiationTemplateWizard>();

        private const string ProjectNameKey = "$projectname$";
        private const string ItemNameKey = "$itemname$";

        private UnfoldScope templateScope;
        private string templateFile;

        private bool wizardCancelled = false;

        private string projectCreated;
        private string projectItemCreated;

        private string tempStoreFile;
        private Uri templateUri;
        private IProduct element;
        private StoreEventBufferingScope eventBufferingScope = null;

        /// <summary>
        /// Gets the solution.
        /// </summary>
        [Import]
        public ISolution Solution { get; internal set; }

        /// <summary>
        /// Gets the pattern manager.
        /// </summary>
        [Import]
        public IPatternManager PatternManager { get; internal set; }

        /// <summary>
        /// Gets the URI service.
        /// </summary>
        [Import]
        public IFxrUriReferenceService UriService { get; internal set; }

        /// <summary>
        /// Gets or sets the binding factory.
        /// </summary>
        [Import]
        public IBindingFactory BindingFactory { get; internal set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "eventBufferingScope")]
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.templateScope != null)
            {
                this.templateScope.Dispose();
                this.templateScope = null;
            }
            if (this.eventBufferingScope != null)
            {
                this.eventBufferingScope.Dispose();
                this.eventBufferingScope = null;
            }
        }

        /// <summary>
        /// Indicates whether the specified project item should be added to the project.
        /// </summary>
        /// <param name="filePath">The path to the project item.</param>
        /// <returns>
        /// True if the project item should be added to the project; otherwise, false.
        /// </returns>
        public override bool ShouldAddProjectItem(string filePath)
        {
            return !wizardCancelled;
        }

        /// <summary>
        /// Runs custom wizard logic when a project has finished generating.
        /// </summary>
        /// <param name="project">The project that finished generating.</param>
        public override void ProjectFinishedGenerating(Project project)
        {
            base.ProjectFinishedGenerating(project);

            if (!wizardCancelled)
            {
                if (this.projectCreated == null
                    && project != null)
                {
                    this.projectCreated = project.FullName;
                }
            }
            else
            {
                project.DTE.Solution.Remove(project);
            }
        }

        /// <summary>
        /// Runs custom wizard logic when a project item has finished generating.
        /// </summary>
        /// <param name="projectItem">The project item that finished generating.</param>
        public override void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            base.ProjectItemFinishedGenerating(projectItem);

            if (this.projectItemCreated == null)
            {
                this.projectItemCreated = projectItem.get_FileNames(1);
            }
        }

        /// <summary>
        /// Runs custom wizard logic at the beginning of a template wizard run.
        /// </summary>
        /// <param name="automationObject">The automation object being used by the template wizard.</param>
        /// <param name="replacementsDictionary">The list of standard parameters to be replaced.</param>
        /// <param name="runKind">A <see cref="T:Microsoft.VisualStudio.TemplateWizard.WizardRunKind"/> indicating the type of wizard run.</param>
        /// <param name="customParams">The custom parameters with which to perform parameter replacement in the project.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public override void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            base.RunStarted(automationObject, replacementsDictionary, runKind, customParams);

            if (this.PatternManager == null)
            {
                FeatureCompositionService.Instance.SatisfyImportsOnce(this);
            }

            Guard.NotNull(() => this.PatternManager, this.PatternManager);
            Guard.NotNull(() => this.UriService, this.UriService);

            var safeProductName = GetSafeTemplateName(replacementsDictionary);
            this.templateFile = (string)customParams[0];
            this.templateUri = this.UriService.CreateUri(this.TemplateSchema);

            if (!UnfoldScope.IsActive ||
                !UnfoldScope.Current.HasUnfolded(templateUri.AbsoluteUri))
            {
                var toolkit = FindToolkitOrThrow(this.PatternManager, templateFile);
                var settings = FindTemplateSettingsOrThrow(toolkit, templateUri);

                using (tracer.StartActivity(Resources.InstantiationTemplateWizard_StartingTemplateOriginated, this.TemplateSchema.PhysicalPath))
                {
                    if (settings.CreateElementOnUnfold)
                    {
                        if (!this.Solution.IsOpen && !this.PatternManager.IsOpen)
                        {
                            this.tempStoreFile = Path.GetTempFileName();
                            this.PatternManager.Open(this.tempStoreFile, true);
                        }

                        // We always create the new scope. If there's a wrapping EventBufferingTemplateWizard, we'll just 
                        // hook on top of that. Events that we buffer will not be raised until the outer buffering completes.
                        eventBufferingScope = new StoreEventBufferingScope();

                        this.element = this.PatternManager.CreateProduct(toolkit, safeProductName);
                        var templateAutomation = FindTemplateAutomationOrThrow(this.element, this.templateUri.AbsoluteUri);

                        if (settings.SyncName && settings.TargetFileName.ValueProvider != null)
                            throw new NotSupportedException(Resources.TemplateAutomation_ValueProviderUnsupportedForSyncNames);

                        this.templateScope = new UnfoldScope(
                            templateAutomation,
                            new ReferenceTag
                            {
                                SyncNames = settings.SyncName,
                                TargetFileName = settings.TargetFileName.Value
                            },
                            this.templateUri.AbsoluteUri);

                        if (!templateAutomation.ExecuteWizard())
                        {
                            if (this.templateScope != null)
                            {
                                this.PatternManager.Close();
                                File.Delete(this.tempStoreFile);
                                throw new WizardCancelledException();
                            }
                            else
                            {
                                wizardCancelled = true;
                            }
                        }
                    }
                }
            }
        }

        private string GetSafeTemplateName(Dictionary<string, string> replacementsDictionary)
        {
            if (replacementsDictionary.ContainsKey("$safeprojectname$"))
            {
                return replacementsDictionary["$safeprojectname$"];
            }
            else
            {
                if (replacementsDictionary.ContainsKey("$rootname$"))
                {
                    return Path.GetFileNameWithoutExtension(replacementsDictionary["$rootname$"]);
                }
                else
                {
                    return Guid.NewGuid().ToString("N");
                }
            }
        }

        /// <summary>
        /// Runs custom wizard logic when the wizard has completed all tasks.
        /// </summary>
        public override void RunFinished()
        {
            // wizardCancelled will be false too if the template wizard originated from a command automation.
            if (!wizardCancelled)
            {
                // Set the artifact link. We use the static UnfoldScope.Current as we may have originated 
                // from a command automation.
                this.SetArtifactLink(this.templateScope ?? UnfoldScope.Current);

                // Move the temporary state file, if any, to the current solution.
                if (!string.IsNullOrEmpty(this.tempStoreFile))
                {
                    var fileName = Path.ChangeExtension(Solution.PhysicalPath, Runtime.StoreConstants.RuntimeStoreExtension);
                    this.PatternManager.SaveAs(fileName);
                    this.RemoveTempFile(tempStoreFile);
                }

                // Run the command if originated in a template automation.
                if (this.templateScope != null)
                {
                    // We know if we have a local template scope, it was created by us and with a template automation.
                    // And if we have a template automation, it's because its CreateElementOnUnfold settings was true.
                    ((TemplateAutomation)this.templateScope.Automation).ExecuteCommand();
                }

                if (this.eventBufferingScope != null)
                {
                    // Complete within a top-level transaction 
                    // to minimize side-effects form potentially 
                    // lots of small commits.
                    using (var tx = this.PatternManager.Store.BeginTransaction())
                    {
                        this.eventBufferingScope.Complete();
                        this.eventBufferingScope.Dispose();

                        tx.Commit();
                    }
                }
            }
            else
            {
                if (this.eventBufferingScope != null)
                {
                    this.eventBufferingScope.Cancel();
                    this.eventBufferingScope.Dispose();
                }
            }

            if (this.templateScope != null)
                this.templateScope.Dispose();

            base.RunFinished();
        }

        private void RemoveTempFile(string storeFile)
        {
            var storeItem = (from item in this.Solution.Traverse().OfType<IItem>()
                             where item.Name == Path.GetFileName(storeFile)
                             select item)
                                  .FirstOrDefault();

            if (storeItem != null)
            {
                File.Delete(storeItem.PhysicalPath);
            }
        }

        private void SetArtifactLink(UnfoldScope scope)
        {
            var container = scope.Automation.Owner;

            // Determine created item
            IItemContainer createdItem = null;
            createdItem = this.Solution.Find<IProject>(p => p.PhysicalPath == this.projectCreated).SingleOrDefault();
            if (createdItem == null)
            {
                createdItem = this.Solution.Find<IItem>(pi => pi.PhysicalPath == this.projectItemCreated).SingleOrDefault();
            }

            if (createdItem != null)
            {
                if (this.UriService.CanCreateUri<IItemContainer>(createdItem))
                {
                    // Set the artifact link
                    SolutionArtifactLinkReference
                        .SetReference(container, this.UriService.CreateUri<IItemContainer>(createdItem))
                        .Tag = BindingSerializer.Serialize(scope.ReferenceTag);
                }
                else
                {
                    tracer.TraceWarning(Properties.Resources.InstantiationTemplateWizard_CantCreateUri, createdItem.GetLogicalPath());
                }
            }

        }

        private static string GetProductNameOrThrow(Dictionary<string, string> replacementsDictionary, WizardRunKind runKind)
        {
            if (runKind == WizardRunKind.AsNewProject)
            {
                return replacementsDictionary[ProjectNameKey];
            }

            if (runKind == WizardRunKind.AsNewItem)
            {
                return replacementsDictionary[ItemNameKey];
            }

            throw new NotSupportedException(string.Format(
                CultureInfo.CurrentCulture,
                Resources.InstantiationTemplateWizard_InvalidWizardType,
                runKind));
        }

        private static TemplateAutomation FindTemplateAutomationOrThrow(IProduct element, string templateUri)
        {
            return element.AutomationExtensions.OfType<TemplateAutomation>().FirstOrDefault(t => t.Settings.TemplateUri == templateUri);
        }

        private static ITemplateSettings FindTemplateSettingsOrThrow(IInstalledToolkitInfo toolkit, Uri uri)
        {
            var uriString = uri.ToString();

            var settings = toolkit.Schema.Pattern.AutomationSettings
                .Where(info => info.As<ITemplateSettings>() != null &&
                    info.As<ITemplateSettings>().TemplateUri == uriString)
                .Select(info => info.As<ITemplateSettings>())
                .FirstOrDefault();

            if (settings != null)
            {
                return settings;
            }

            // Traverse the rest in a generic way and find the first match;
            settings = toolkit.Schema.Pattern
                .Views
                .SelectMany(view => view.Elements)
                .Traverse(element => element.Elements)
                .SelectMany(element => element.AutomationSettings)
                .Where(setting => setting.As<ITemplateSettings>() != null &&
                    setting.As<ITemplateSettings>().TemplateUri == uriString)
                .Select(info => info.As<ITemplateSettings>())
                .FirstOrDefault();

            if (settings == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.InstantiationTemplateWizard_SettingsUnavailable,
                    toolkit.Name,
                    uriString));
            }

            return settings;
        }

        /// <summary>
        /// Returns the installed toolkit that contains the given template.
        /// </summary>
        /// <param name="patternManager">The PatternManager service</param>
        /// <param name="templateFile">The full path to the vstemplate file. 
        /// i.e. (VS2010) %localappdata%\Microsoft\VisualStudio\10.0\Extensions\[author][\[extensionname]\[version]\Assets\Templates\Projects\~PC\Template.zip\Template.vstempate
        /// i.e. (VS2012) %localappdata%\Microsoft\VisualStudio\11.0\VTC\[guid]\~PC\Projects\Template.zip\Template.vstemplate
        /// </param>
        /// <returns></returns>
        internal static IInstalledToolkitInfo FindToolkitOrThrow(IPatternManager patternManager, string templateFile)
        {
            // Find the toolkit that shares the same path as this vstemplate (VS2010 only)
            var toolkitInfo = patternManager.InstalledToolkits
                .FirstOrDefault(f => templateFile.StartsWith(f.Extension.InstallPath, StringComparison.OrdinalIgnoreCase));

#if VSVER11
            // In VS2012, vstemplates are loaded from cache not from toolkit installation path.
            if (toolkitInfo == null)
            {
                // Find the toolkit that contains the vstemplate, by TemplateID or Name
                if (File.Exists(templateFile))
                {
                    var template = VsTemplateFile.Read(templateFile);
                    if (template != null)
                    {
                        if (!String.IsNullOrEmpty(template.TemplateData.TemplateID))
                        {
                            toolkitInfo = patternManager.InstalledToolkits
                                .FirstOrDefault(it => it.Templates.Any(t => ((t.TemplateData.TemplateID != null) &&
                                (t.TemplateData.TemplateID.Equals(template.TemplateData.TemplateID, StringComparison.OrdinalIgnoreCase)))));
                        }
                        else
                        {
                            toolkitInfo = patternManager.InstalledToolkits
                                .FirstOrDefault(it => it.Templates.Any(t => ((t.TemplateData.Name != null) &&
                                (t.TemplateData.Name.Value.Equals(template.TemplateData.Name.Value, StringComparison.OrdinalIgnoreCase)))));
                        }
                    }
                }
            }
#endif

            if (toolkitInfo == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.InstantiationTemplateWizard_ToolkitInfoNotFound,
                    templateFile));
            }

            return toolkitInfo;
        }
    }
}
