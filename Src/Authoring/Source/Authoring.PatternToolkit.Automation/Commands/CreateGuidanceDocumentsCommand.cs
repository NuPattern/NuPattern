using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using NuPattern.Authoring.Guidance;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Runtime.References;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.PatternToolkit.Automation.Commands
{
    /// <summary>
    /// Shreds an associated toolkit guidance document and adds documents to current toolkit project.
    /// </summary>
    [DisplayNameResource("CreateGuidanceDocumentsCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("CreateGuidanceDocumentsCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class CreateGuidanceDocumentsCommand : NuPattern.Runtime.Command
    {
        private const string ProjectExtension = ".csproj";
        private const string GeneratedFileExtensionFilter = "*" + TocGuidanceProcessor.ContentFileExtension;
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateGuidanceDocumentsCommand>();
        private IGuidanceProcessor processor;

        /// <summary>
        /// Creates a new instance of the <see cref="CreateGuidanceDocumentsCommand"/> class.
        /// </summary>
        public CreateGuidanceDocumentsCommand()
            : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="CreateGuidanceDocumentsCommand"/> class.
        /// </summary>
        /// <remarks>This is for testing only.</remarks>
        internal CreateGuidanceDocumentsCommand(IGuidanceProcessor processor)
            : this()
        {
            this.processor = processor;
        }

        /// <summary>
        /// Gets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IUriReferenceService UriReferenceService
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current solution
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the error list
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IErrorList ErrorList
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IGuidance CurrentElement
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this commmand.
        /// </summary>
        /// <remarks></remarks>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CreateGuidanceDocumentsCommand_TraceInitial, this.CurrentElement.InstanceName);

            // Get guidance document path
            var documentFilePath = GuidanceDocumentHelper.GetDocumentPath(tracer,
                this.CurrentElement.AsElement(), this.UriReferenceService);

            if (this.processor == null)
            {
                this.processor = new TocGuidanceProcessor(documentFilePath,
                    this.CurrentElement.Parent.Parent.PatternToolkitInfo.Identifier, this.CurrentElement.ProjectContentPath);
            }

            // Validate document first
            tracer.TraceInformation(
                Resources.CreateGuidanceDocumentsCommand_TraceValidatingDocument, documentFilePath);

            this.ErrorList.Clear(documentFilePath);
            var errors = this.processor.ValidateDocument();
            if (errors.Any())
            {
                tracer.TraceInformation(
                    Resources.CreateGuidanceDocumentsCommand_TraceDocumentValidationFailed, documentFilePath, errors.Count());

                this.ErrorList.AddRange(documentFilePath, errors);
                return;
            }

            // Ensure content container exists
            var container = EnsureContentContainer();
            if (container == null)
            {
                return;
            }

            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                // Generate documents to temporary location
                tracer.TraceInformation(
                    Resources.CreateGuidanceDocumentsCommand_TraceTempGeneration, tempPath);

                var generatedFiles = this.processor.GenerateWorkflowDocuments(tempPath);

                tracer.TraceInformation(
                    Resources.CreateGuidanceDocumentsCommand_TraceSyncingDocuments, tempPath, container.GetLogicalPath());

                // Rationalize generated documents with project container
                SyncGeneratedProjectFiles(generatedFiles.ToDictionary<string, string>(gf => gf), tempPath, container);
            }
            finally
            {
                CleanupGeneratedDocuments(tempPath);
            }
        }

        private void SyncGeneratedProjectFiles(IDictionary<string, string> generatedFiles, string sourceFolderPath, IItemContainer targetContainer)
        {
            var targetContainerPath = targetContainer.GetLogicalPath();

            tracer.TraceInformation(
                Resources.CreateGuidanceDocumentsCommand_TraceAddingExcludedFiles, targetContainer.GetLogicalPath());

            // Ensure that all (*.MHT) files on the disk are included the targetContainer
            var targetContainerFiles = Directory.GetFiles(targetContainer.PhysicalPath, GeneratedFileExtensionFilter);
            foreach (var targetContainerFile in targetContainerFiles)
            {
                var projectItem = targetContainer.Find<IItem>(targetContainerFile).FirstOrDefault();
                if (projectItem == null)
                {
                    // Add existing file to project (SCC handles the 'Add' automatically)
                    tracer.TraceInformation(
                        Resources.CreateGuidanceDocumentsCommand_TraceAddingExcludedFile, targetContainerFile, targetContainerPath);

                    targetContainer.Add(targetContainerFile);
                }
            }

            tracer.TraceInformation(
                Resources.CreateGuidanceDocumentsCommand_TraceAddingNewFiles, targetContainer.GetLogicalPath());

            // Copy all generated files to project (overwrite existing)
            foreach (var generatedFile in generatedFiles)
            {
                var generatedFileName = generatedFile.Key;
                var targetContainerItem = targetContainer.Find<IItem>(generatedFileName).FirstOrDefault();
                if (targetContainerItem == null)
                {
                    // Add new file to project (SCC Handles the 'Add' automatically)
                    tracer.TraceInformation(
                        Resources.CreateGuidanceDocumentsCommand_TraceAddingNewFile, generatedFileName, targetContainerPath);

                    targetContainer.Add(Path.Combine(sourceFolderPath, generatedFileName));
                }
                else
                {
                    // SCC 'Checkout' existing file (or make writable), and copy file to disk location
                    tracer.TraceInformation(
                        Resources.CreateGuidanceDocumentsCommand_TraceOverwriteExistingFile, generatedFileName, targetContainerPath);

                    targetContainerItem.Checkout();
                    File.Copy(Path.Combine(sourceFolderPath, generatedFileName),
                        Path.Combine(targetContainer.PhysicalPath, generatedFileName), true);
                }
            }

            // Remove any non-current items
            var itemsToDelete = new List<string>();
            var targetContainerItems = targetContainer.Items.OfType<IItem>();
            foreach (var targetContainerItem in targetContainerItems)
            {
                if (!generatedFiles.ContainsKey(targetContainerItem.Name))
                {
                    itemsToDelete.Add(targetContainerItem.Name);
                }
                else
                {
                    // Ensure build properties
                    tracer.TraceInformation(
                        Resources.CreateGuidanceDocumentsCommand_TraceSettingBuildAction, targetContainerItem.Name, targetContainerPath);

                    targetContainerItem.Data.ItemType = BuildAction.Content.ToString();
                    targetContainerItem.Data.IncludeInVSIX = Boolean.TrueString.ToLowerInvariant();
                }
            }

            tracer.TraceInformation(
            Resources.CreateGuidanceDocumentsCommand_TraceDeleteSuperfluousFiles, targetContainer.GetLogicalPath());

            if (itemsToDelete.Any())
            {
                foreach (var item in itemsToDelete)
                {
                    tracer.TraceInformation(
                        Resources.CreateGuidanceDocumentsCommand_TraceDeleteSuperfluousFile, item, targetContainerPath);

                    var itemToDelete = targetContainer.Find<IItem>(item).FirstOrDefault();
                    if (itemToDelete != null)
                    {
                        itemToDelete.Delete();
                    }
                }
            }
        }

        private IProject GetProjectReference()
        {
            var references = SolutionArtifactLinkReference.GetResolvedReferences(this.CurrentElement.Parent.Parent.Parent.AsProduct(),
                this.UriReferenceService);
            if (!references.Any())
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    Resources.CreateGuidanceDocumentsCommand_ErrorNoReferencesFound, this.CurrentElement.InstanceName));
            }
            else
            {
                var reference = references.FirstOrDefault(r => r.PhysicalPath.EndsWith(ProjectExtension));
                if (reference == null)
                {
                    throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                        Resources.CreateGuidanceDocumentsCommand_ErrorNoProjectReferenceFound, this.CurrentElement.InstanceName));
                }

                return reference as IProject;
            }
        }

        private IItemContainer EnsureContentContainer()
        {
            var project = GetProjectReference();
            var projectContainer = project.Find(this.CurrentElement.ProjectContentPath).FirstOrDefault();
            if (projectContainer == null)
            {
                tracer.TraceInformation(
                    Resources.CreateGuidanceDocumentsCommand_TraceContentFolderCreating, project.Name, this.CurrentElement.ProjectContentPath);

                projectContainer = project.CreateFolderPath(this.CurrentElement.ProjectContentPath) as IItemContainer;
                if (projectContainer == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                        Resources.CreateGuidanceDocumentsCommand_ErrorNoContentFolderFound, this.CurrentElement.ProjectContentPath));
                }
            }

            return projectContainer;
        }

        private void CleanupGeneratedDocuments(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
            catch (IOException)
            {
                // Try to wait for any process that maybe locking the generated documents to end.
                for (var count = 0; count < 10; count++)
                {
                    tracer.TraceWarning(
                        Resources.CreateGuidanceDocumentsCommand_TraceTryDeleteGeneratedDocuments, path);

                    System.Threading.Thread.Sleep(1000);

                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }

                        return;
                    }
                    catch (IOException)
                    {
                        // Ignore and try again
                    }
                }

                tracer.TraceError(
                    Resources.CreateGuidanceDocumentsCommand_TraceDeleteGeneratedDocumentsFailed, path);
            }
        }
    }
}
