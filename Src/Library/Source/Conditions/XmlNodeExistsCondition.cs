using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.ComponentModel.Design;
using NuPattern.Extensibility;
using NuPattern.Extensibility.Bindings.Design;
using NuPattern.Library.Commands;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Xml;

namespace NuPattern.Library.Conditions
{
    /// <summary>
    /// Indicates that an element orattribute of an XML document exists.
    /// </summary>
    [DescriptionResource("XmlNodeExistsCondition_Description", typeof(Resources))]
    [DisplayNameResource("XmlNodeExistsCondition_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class XmlNodeExistsCondition : Condition
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ValidElementCondition>();

        /// <summary>
        /// Creates a new instance of the <see cref="XmlNodeExistsCondition"/> class.
        /// </summary>
        public XmlNodeExistsCondition()
        {
            this.Namespaces = new Collection<XmlNamespace>();
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution { get; set; }

        /// <summary>
        /// Gets or sets the current element.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IProductElement CurrentElement { get; set; }

        /// <summary>
        /// Gets or sets the URI reference service.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IFxrUriReferenceService UriReferenceService { get; set; }

        /// <summary>
        /// Gets or sets the XML Processor.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IXmlProcessor XmlProcessor { get; set; }

        /// <summary>
        /// Gets or sets the source path of the XML file.
        /// </summary>
        [DisplayNameResource("ModifyXmlCommand_SourcePath_DisplayName", typeof(Resources))]
        [DescriptionResource("ModifyXmlCommand_SourcePath_Description", typeof(Resources))]
        public virtual string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the path in the XML element/attribute to modify.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource("ModifyXmlCommand_XmlPath_DisplayName", typeof(Resources))]
        [DescriptionResource("ModifyXmlCommand_XmlPath_Description", typeof(Resources))]
        public virtual string XmlPath { get; set; }

        /// <summary>
        /// Gets or sets the namespaces to use in the <see cref="XmlPath"/> query.
        /// </summary>
        [DisplayNameResource("ModifyXmlCommand_Namespaces_DisplayName", typeof(Resources))]
        [DescriptionResource("ModifyXmlCommand_Namespaces_Description", typeof(Resources))]
        [TypeConverter(typeof(DesignCollectionConverter<XmlNamespace>))]
        public Collection<XmlNamespace> Namespaces { get; set; }

        /// <summary>
        /// Evaluates this instance.
        /// </summary>
        public override bool Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.XmlNodeExistsCondition_TraceInitial,
                    this.SourcePath, this.XmlPath);

            var result = false;

            // Resolve SourcePath
            var sourceItem = PathResolver.ResolveToSolutionItem<IItem>(this.CurrentElement, this.Solution, this.UriReferenceService, this.SourcePath);
            if (sourceItem == null)
            {
                tracer.TraceError(
                    Resources.XmlNodeExistsCondition_ErrorNoSource, this.CurrentElement.InstanceName, this.SourcePath);
            }
            else
            {
                // Resolve XmlPath
                var xPath = ExpressionEvaluator.Evaluate(this.CurrentElement, this.XmlPath);
                tracer.TraceInformation(
                    Resources.XmlNodeExistsCondition_TraceXPath,
                        this.XmlPath, xPath);

                var sourceFilePath = sourceItem.PhysicalPath;

                // Load XML file
                try
                {
                    this.XmlProcessor.LoadDocument(sourceFilePath);
                }
                catch (Exception)
                {
                    tracer.TraceError(
                        Resources.XmlNodeExistsCondition_ErrorLoadXmlDoc, sourceFilePath);
                    throw;
                }

                // Search XML
                try
                {
                    // Locate the node in the XML
                    var node = this.XmlProcessor.FindFirst(xPath, this.Namespaces.ToDictionary());
                    result = (node != null);
                }
                catch (XPathException)
                {
                    tracer.TraceError(
                        Resources.XmlNodeExistsCondition_ErrorXPath, sourceFilePath, xPath);
                    throw;
                }
                finally
                {
                    try
                    {
                        this.XmlProcessor.CloseDocument();
                    }
                    catch (Exception)
                    {
                        tracer.TraceError(
                            Resources.XmlNodeExistsCondition_ErrorCloseXmlDoc, sourceFilePath);
                        // Ignore exception on close
                    }
                }
            }

            tracer.TraceInformation(
                Resources.XmlNodeExistsCondition_TraceEvaluation, this.SourcePath, this.XmlPath, result);

            return result;
        }
    }
}
