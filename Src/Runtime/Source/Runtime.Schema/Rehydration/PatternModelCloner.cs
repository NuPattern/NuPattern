using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Clones a pattern schema model from a source to a target.
    /// </summary>
    public class PatternModelCloner
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<PatternModelCloner>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternModelCloner"/> class.
        /// </summary>
        public PatternModelCloner()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternModelCloner"/> class.
        /// </summary>
        /// <param name="sourcePatternModel">The source pattern line.</param>
        /// <param name="sourceVersion">The source version.</param>
        /// <param name="targetPatternModel">The target pattern line.</param>
        public PatternModelCloner(PatternModelSchema sourcePatternModel, Version sourceVersion, PatternModelSchema targetPatternModel)
        {
            Guard.NotNull(() => sourcePatternModel, sourcePatternModel);
            Guard.NotNull(() => sourceVersion, sourceVersion);
            Guard.NotNull(() => targetPatternModel, targetPatternModel);

            this.SourcePatternModel = sourcePatternModel;
            this.SourceVersion = sourceVersion;
            this.TargetPatternModel = targetPatternModel;
        }

        /// <summary>
        /// Gets the source pattern model.
        /// </summary>
        /// <value>The source pattern model.</value>
        public PatternModelSchema SourcePatternModel { get; set; }

        /// <summary>
        /// Gets or sets the source version.
        /// </summary>
        /// <value>The source version.</value>
        public Version SourceVersion { get; set; }

        /// <summary>
        /// Gets the target pattern model.
        /// </summary>
        /// <value>The target pattern model.</value>
        public PatternModelSchema TargetPatternModel { get; set; }

        /// <summary>
        /// Clones the definition of the model <c>PatternModelCloner</c>.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Not Applicable")]
        public void Clone()
        {
            tracer.TraceInformation(string.Format(CultureInfo.InvariantCulture, Properties.Resources.PatternModelCloner_PatternModelCloning, this.SourcePatternModel.Id));

            tracer.TraceInformation(Properties.Resources.PatternModelCloner_EstablishingPatternModelInheritance);

            this.TargetPatternModel.Store.TransactionManager.DoWithinTransaction(
                () => this.TargetPatternModel.BaseVersion = this.SourceVersion.ToString());

            if (this.SourcePatternModel.Pattern != null)
            {
                var serializationResult = new SerializationResult();

                this.TargetPatternModel.Store.TransactionManager.DoWithinTransaction(() =>
                {
                    tracer.TraceInformation(Properties.Resources.PatternModelCloner_CloningViews);

                    foreach (var view in this.SourcePatternModel.Pattern.Views)
                    {
                        tracer.TraceInformation(string.Format(CultureInfo.InvariantCulture, Properties.Resources.PatternModelCloner_CloningView, view.Name));

                        PatternModelSerializationHelper.CreatePatternModelSchemaDiagram(
                            serializationResult,
                            this.TargetPatternModel.Store.GetDefaultDiagramPartition(),
                            this.TargetPatternModel,
                            view.DiagramId);
                    }
                });

                if (this.SourcePatternModel.Pattern != null)
                {
                    tracer.TraceInformation(Properties.Resources.PatternModelCloner_CloningProduct);

                    CopyElementGraph(this.TargetPatternModel.Store, this.TargetPatternModel, this.SourcePatternModel.Pattern);
                }

                tracer.TraceInformation(Properties.Resources.PatternModelCloner_EstablishingElementInheritance);

                this.SetNamedElementBaseIdentifiers();
            }
            else
            {
                tracer.TraceWarning(Properties.Resources.PatternModelCloner_PatternSchemaNotFound);
            }
        }

        private static void CopyElementGraph(Store targetStore, ModelElement targetElement, ModelElement element)
        {
            var dataObject = new DataObject();

            var elementOperations = new ElementOperations(targetStore, targetStore.DefaultPartition);

            tracer.TraceVerbose(string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.PatternModelCloner_CloningElement, element.Id));
            elementOperations.Copy(dataObject, new List<ModelElement> { element });

            targetStore.TransactionManager.DoWithinTransaction(() =>
                elementOperations.Merge(targetElement, dataObject));
        }

        private static string ConstructHierarchyName(NamedElementSchema element)
        {
            StringBuilder nameBuilder = new StringBuilder();

            nameBuilder.Insert(0, element.Name);
            nameBuilder.Insert(0, ":");
            nameBuilder.Insert(0, element.GetType().Name);
            var parent = DomainClassInfo.FindEmbeddingElement(element);

            while (parent != null && parent is NamedElementSchema)
            {
                var namedParent = (NamedElementSchema)parent;
                nameBuilder.Insert(0, ".");
                nameBuilder.Insert(0, namedParent.Name);
                nameBuilder.Insert(0, ":");
                nameBuilder.Insert(0, namedParent.GetType().Name);

                parent = DomainClassInfo.FindEmbeddingElement(parent);
            }

            return nameBuilder.ToString();
        }

        /// <summary>
        /// Assigns the NamedElementSchema.BaseId of the targetPatternModel from the ModelElement.Id of the sourcePatternModel.
        /// </summary>
        private void SetNamedElementBaseIdentifiers()
        {
            var sourceHierarchyNames =
                this.SourcePatternModel.Store.ElementDirectory.AllElements.OfType<NamedElementSchema>()
                    .ToDictionary(nm => ConstructHierarchyName(nm), nm => nm.Id.ToString());

            this.TargetPatternModel.Store.TransactionManager.DoWithinTransaction(() =>
            {
                foreach (var sourceHierarchyName in sourceHierarchyNames)
                {
                    var namedElement = this.TargetPatternModel.Store.ElementDirectory.AllElements
                        .OfType<NamedElementSchema>()
                        .FirstOrDefault(nm => ConstructHierarchyName(nm).Equals(sourceHierarchyName.Key));

                    if (namedElement != null && string.IsNullOrEmpty(namedElement.BaseId))
                    {
                        tracer.TraceVerbose(string.Format(CultureInfo.InvariantCulture,
                            Properties.Resources.PatternModelCloner_EstablishElementInheritance, ConstructHierarchyName(namedElement)));

                        namedElement.BaseId = sourceHierarchyName.Value;
                    }
                }
            });
        }
    }
}