using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Modeling.Diagrams;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using NuPattern.Extensibility;
using NuPattern.Extensibility.References;
using NuPattern.Library.Commands;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Double-derived class to allow easier code customization.
    /// </summary>
    internal partial class PatternModelDocData
    {
        private bool modelCloned;

        [Import]
        private IPatternModelSchemaUpgradeManager PatternModelUpgradeManager { get; set; }

        /// <summary>
        /// Get the Partition where diagram data will be loaded into.
        /// Base implementation returns the default partition of the store.
        /// </summary>
        /// <returns>The diagram partition.</returns>
        protected internal override Partition GetDiagramPartition()
        {
            return this.Store.GetDefaultDiagramPartition();
        }

        protected override void OnDocumentLoaded(EventArgs e)
        {
            base.OnDocumentLoaded(e);

            if (this.modelCloned)
            {
                this.ValidationController.Validate(this.Store, ValidationCategories.Save | ValidationCategories.Menu);
            }

            InitializeLayout();
        }

        /// <summary>
        /// Loads the given file.
        /// </summary>
        /// <param name="fileName">The file to load.</param>
        /// <param name="reload">If it is reload or not.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Not Applicable")]
        protected override void Load(string fileName, bool reload)
        {
            Guard.NotNullOrEmpty(() => fileName, fileName);

            var serializationResult = new SerializationResult();
            PatternModelSchema modelRoot = null;
            var schemaResolver = new ModelingSchemaResolver(this.ServiceProvider);

            PatternModelDomainModel.EnableDiagramRules(this.Store);
            this.Store.RuleManager.DisableRule(typeof(FixUpDiagram));
            this.Store.RuleManager.EnableRule(typeof(FixUpMultipleDiagram));

            var diagramFileNames =
                Directory.GetFiles(
                    Path.GetDirectoryName(fileName), string.Concat("*", DesignerConstants.ModelExtension, DesignerConstants.DiagramFileExtension));

            // Run migration rules
            this.ExecuteUpgradeRules(fileName, diagramFileNames);

            // Load models
            modelRoot = PatternModelSerializationHelper.Instance.LoadModelAndDiagrams(
                    serializationResult,
                    this.GetModelPartition(),
                    fileName,
                    this.GetDiagramPartition(),
                    diagramFileNames,
                    schemaResolver,
                    null /* no load-time validation */,
                    this.SerializerLocator);

            this.SuspendErrorListRefresh();

            try
            {
                foreach (SerializationMessage serializationMessage in serializationResult)
                {
                    this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
                }
            }
            finally
            {
                this.ResumeErrorListRefresh();
            }

            if (serializationResult.Failed)
            {
                throw new InvalidOperationException(PatternModelDomainModel.SingletonResourceManager.GetString("CannotOpenDocument"));
            }
            else
            {
                this.SetRootElement(modelRoot);

                // Attempt to set the encoding
                if (serializationResult.Encoding != null)
                {
                    this.ModelingDocStore.SetEncoding(serializationResult.Encoding);
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.SetDocDataDirty(0)); // Setting the encoding will mark the document as dirty, so clear the dirty flag.
                }
            }

            this.RehydrateModel();
        }

        /// <summary>
        /// Save the given document that is subordinate to this document.
        /// </summary>
        /// <param name="subordinateDocument">The subordinate document.</param>
        /// <param name="fileName">The fileName to save.</param>
        protected override void SaveSubordinateFile(DocData subordinateDocument, string fileName)
        {
            Guard.NotNull(() => subordinateDocument, subordinateDocument);

            SerializationResult serializationResult = new SerializationResult();

            var diagrams = this.GetDiagrams(fileName);

            foreach (var diagram in diagrams)
            {
                try
                {
                    this.SuspendFileChangeNotification(diagram.Key);

                    PatternModelSerializationHelper.Instance.SaveDiagram(serializationResult, diagram.Value, diagram.Key, this.Encoding, false);
                }
                finally
                {
                    this.ResumeFileChangeNotification(fileName);
                }
            }

            this.SuspendErrorListRefresh();

            try
            {
                foreach (var serializationMessage in serializationResult)
                {
                    this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
                }
            }
            finally
            {
                this.ResumeErrorListRefresh();
            }

            if (!serializationResult.Failed)
            {
                this.NotifySubordinateDocumentSaved(subordinateDocument.FileName, fileName);
            }
            else
            {
                throw new InvalidOperationException(PatternModelDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
            }
        }

        /// <summary>
        /// Called on both document load and reload.
        /// </summary>
        protected override void OnDocumentLoaded()
        {
            base.OnDocumentLoaded();

            this.SubscribeToStoreEvents();
        }

        /// <summary>
        /// Saves the given file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        protected override void Save(string fileName)
        {
            Guard.NotNullOrEmpty(() => fileName, fileName);

            var serializationResult = new SerializationResult();
            var modelRoot = (PatternModelSchema)this.RootElement;
            bool saveAs = StringComparer.OrdinalIgnoreCase.Compare(fileName, this.FileName) != 0;

            var diagrams = this.GetDiagrams(fileName);

            if (diagrams.Count > 0 && !saveAs)
            {
                try
                {
                    foreach (var diagramFile in diagrams)
                    {
                        this.SuspendFileChangeNotification(diagramFile.Key);
                    }

                    PatternModelSerializationHelper.Instance.SaveModelAndDiagrams(serializationResult, modelRoot, fileName, diagrams, this.Encoding, false);

                    var solution = this.ServiceProvider.GetService<ISolution>();
                    if (solution == null)
                    {
                        throw new InvalidOperationException("Can not locate solution service.");
                    }

                    AddDependentFiles(this.ServiceProvider.GetService<ISolution>(), fileName, diagrams.Select(dg => dg.Key));
                }
                finally
                {
                    foreach (var diagramFile in diagrams)
                    {
                        this.ResumeFileChangeNotification(diagramFile.Key);
                    }
                }
            }
            else
            {
                PatternModelSerializationHelper.Instance.SaveModel(serializationResult, modelRoot, fileName, this.Encoding, false);
            }

            this.SuspendErrorListRefresh();

            try
            {
                foreach (SerializationMessage serializationMessage in serializationResult)
                {
                    this.AddErrorListItem(new SerializationErrorListItem(this.ServiceProvider, serializationMessage));
                }
            }
            finally
            {
                this.ResumeErrorListRefresh();
            }

            if (serializationResult.Failed)
            {
                throw new InvalidOperationException(PatternModelDomainModel.SingletonResourceManager.GetString("CannotSaveDocument"));
            }
        }

        private static void AddDependentFiles(ISolution solution, string modelFile, IEnumerable<string> diagramFiles)
        {
            var item = solution.Find<IItem>(it => it.PhysicalPath == modelFile).Single();

            foreach (var diagramFile in diagramFiles)
            {
                if (!item.Find<IItem>(it => it.PhysicalPath == diagramFile).Any())
                {
                    item.Add(diagramFile);
                }
            }
        }

        private static string ConstructDiagramFileName(string modelFile, ViewSchema view)
        {
            return String.Concat(
                modelFile.Replace(Path.GetFileNameWithoutExtension(modelFile), view.DiagramId),
                DesignerConstants.DiagramFileExtension);
        }

        private Dictionary<string, PatternModelSchemaDiagram> GetDiagrams(string fileName)
        {
            var diagrams = this.Store.ElementDirectory.FindElements<PatternModelSchemaDiagram>()
                .ToDictionary(diagram =>
                        Path.Combine(
                            Path.GetDirectoryName(fileName),
                            string.Concat(diagram.Id, DesignerConstants.ModelExtension, DesignerConstants.DiagramFileExtension)),
                        diagram => diagram);

            return diagrams;
        }

        private void RehydrateModel()
        {
            var patternModel = this.RootElement as PatternModelSchema;

            if (patternModel.IsInTailorMode() && patternModel.Pattern == null)
            {
                var componentModel = this.GetService<SComponentModel, IComponentModel>();
                var installedToolkits = componentModel.GetService<IEnumerable<IInstalledToolkitInfo>>();

                var toolkitInfo = installedToolkits
                    .SingleOrDefault(toolkit => toolkit.Id.Equals(patternModel.BaseId, StringComparison.OrdinalIgnoreCase));

                //// TODO: what should be the behavior if we cannot find the base toolkit?
                if (toolkitInfo != null)
                {
                    using (var store = new Store(this.ServiceProvider, typeof(CoreDesignSurfaceDomainModel)))
                    {
                        var baseProductModel = (PatternModelSchema)toolkitInfo.Schema;

                        patternModel.Tailor(baseProductModel, toolkitInfo.Version);

                        // Establish link between hydrdated product line and the runtime product model.
                        var patternManager = this.TryGetService<IPatternManager>();
                        var uriService = this.TryGetService<IFxrUriReferenceService>();
                        if (patternManager != null && uriService != null)
                        {
                            var fileName = Path.GetFileName(this.FileName);
                            var productModel = patternManager.Store.FindAll<IProductElement>().FirstOrDefault(element =>
                                // Grab the product element that has an artifact reference pointing 
                                // to this designer file.
                                SolutionArtifactLinkReference.GetReferences(element, reference =>
                                {
                                    var tag = ReferenceTag.TryDeserialize(reference.Tag);
                                    return tag != null && tag.TargetFileName == fileName;
                                }).Any());

                            // If we got a product model, create a uri pointing to the owning product, 
                            // and that becomes the product link.
                            if (productModel != null)
                            {
                                var link = uriService.CreateUri(productModel.Product);
                                patternModel.Pattern.PatternLink = link.ToString();
                            }
                        }

                        this.modelCloned = true;
                    }
                }
            }
        }

        private void SwitchView(ViewSchema view)
        {
            if (this.Store.GetCurrentDiagram() == this.Store.GetDiagramForView(view))
            {
                var docview = this.DocViews.First() as SingleDiagramDocView;
                docview.Diagram = view.Store.GetDiagramForDefaultView();
            }
        }

        private void DeleteDiagram(ViewSchema view)
        {
            this.Store.TransactionManager.DoWithinTransaction(() => this.Store.GetDiagramForView(view).Delete());

            var solution = this.ServiceProvider.GetService<ISolution>();
            var modelItem = solution.Find<IItem>(it => it.PhysicalPath == this.FileName).Single();
            var diagramFile = ConstructDiagramFileName(this.FileName, view);

            var diagramItem = modelItem.Find<IItem>(it => it.PhysicalPath == diagramFile).FirstOrDefault();

            if (diagramItem != null)
            {
                diagramItem.As<ProjectItem>().Delete();
            }
        }

        private void SubscribeToStoreEvents()
        {
            this.Store.EventManagerDirectory.ElementDeleted.Add(
                this.Store.DomainDataDirectory.FindDomainClass(typeof(ViewSchema)),
                new EventHandler<ElementDeletedEventArgs>((element, e) =>
                {
                    var view = (ViewSchema)e.ModelElement;

                    SwitchView(view);
                    DeleteDiagram(view);
                }));
        }

        private void InitializeLayout()
        {
            var currentView = this.Store.GetCurrentView();
            if (currentView != null)
            {
                var viewShape = currentView.GetShape<ViewShape>();
                if (viewShape != null)
                {
                    viewShape.PerformInitialLayout();
                }
            }
        }

        private void ExecuteUpgradeRules(string schemaFilePath, string[] diagramFilePaths)
        {
            if (this.PatternModelUpgradeManager != null)
            {
                // Create the context
                var context = new SchemaUpgradeContext(schemaFilePath, diagramFilePaths);
                var componentModel = this.ServiceProvider.GetService<SComponentModel, IComponentModel>();
                componentModel.DefaultCompositionService.SatisfyImportsOnce(context);

                // Execute the manager
                this.PatternModelUpgradeManager.Execute(context);
            }
        }
    }
}