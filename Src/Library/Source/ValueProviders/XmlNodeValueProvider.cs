using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Xml.XPath;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.Runtime.Bindings.Design;
using NuPattern.VisualStudio.Solution;
using NuPattern.Xml;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref=" ValueProvider"/> that provides the value of an XML element/attribute from an XML configuration file.
    /// </summary>
    [DisplayNameResource("XmlNodeValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource("XmlNodeValueProvider_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class XmlNodeValueProvider : ValueProvider
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<SolutionNameValueProvider>();

        /// <summary>
        /// Creates a new instance of the <see cref="XmlNodeValueProvider"/> class.
        /// </summary>
        public XmlNodeValueProvider()
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
        public IUriReferenceService UriReferenceService { get; set; }

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
        /// Evaluates this provider.
        /// </summary>
        public override object Evaluate()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.XmlNodeValueProvider_TraceInitial,
                    this.SourcePath, this.XmlPath);

            var result = (string)null;

            // Resolve SourcePath
            var sourceItem = PathResolver.ResolveToSolutionItem<IItem>(this.CurrentElement, this.Solution, this.UriReferenceService, this.SourcePath);
            if (sourceItem == null)
            {
                tracer.TraceError(
                    Resources.XmlNodeValueProvider_ErrorNoSource, this.CurrentElement.InstanceName, this.SourcePath);
            }
            else
            {
                // Resolve XmlPath
                var xPath = ExpressionEvaluator.Evaluate(this.CurrentElement, this.XmlPath);
                tracer.TraceInformation(
                    Resources.XmlNodeValueProvider_TraceXPath,
                        this.XmlPath, xPath);

                var sourceFilePath = sourceItem.PhysicalPath;
                var sourceLogicalPath = sourceItem.GetLogicalPath();

                // Load XML file
                try
                {
                    this.XmlProcessor.LoadDocument(sourceFilePath);
                }
                catch (Exception)
                {
                    tracer.TraceError(
                        Resources.XmlNodeValueProvider_ErrorLoadXmlDoc, sourceFilePath);
                    throw;
                }

                // Search XML
                try
                {
                    // Locate the node in the XML
                    var node = this.XmlProcessor.FindFirst(xPath, this.Namespaces.ToDictionary());
                    if (node != null)
                    {
                        result = node.Value;
                    }
                    else
                    {
                        tracer.TraceError(
                            Resources.XmlNodeValueProvider_ErrorSearchNodeNotFound, sourceLogicalPath, xPath);
                    }
                }
                catch (XPathException)
                {
                    tracer.TraceError(
                        Resources.XmlNodeValueProvider_ErrorXPath, sourceFilePath, xPath);
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
                            Resources.XmlNodeValueProvider_ErrorCloseXmlDoc, sourceFilePath);
                        // Ignore exception on close
                    }
                }
            }

            tracer.TraceInformation(
                Resources.XmlNodeValueProvider_TraceEvaluation, this.CurrentElement.InstanceName, this.SourcePath, this.XmlPath, result);

            return result;
        }
    }
}
