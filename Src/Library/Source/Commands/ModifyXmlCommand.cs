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
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Solution;
using NuPattern.Xml;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Updates or deletes a value in the specified XML file.
    /// </summary>
    [DisplayNameResource(@"ModifyXmlCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ModifyXmlCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class ModifyXmlCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ModifyXmlCommand>();

        /// <summary>
        /// Creates a new instance of the <see cref="ModifyXmlCommand"/> class.
        /// </summary>
        public ModifyXmlCommand()
        {
            this.Namespaces = new Collection<XmlNamespace>();
        }

        /// <summary>
        /// Gets or sets the error list.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public IErrorList ErrorList { get; set; }

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
        [DisplayNameResource(@"ModifyXmlCommand_SourcePath_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ModifyXmlCommand_SourcePath_Description", typeof(Resources))]
        public virtual string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the path in the XML element/attribute to modify.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ModifyXmlCommand_XmlPath_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ModifyXmlCommand_XmlPath_Description", typeof(Resources))]
        public virtual string XmlPath { get; set; }

        /// <summary>
        /// Gets or sets the action to perform on the XML.
        /// </summary>
        [DefaultValue(ModifyAction.Update)]
        [DesignOnly(true)]
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ModifyXmlCommand_Action_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ModifyXmlCommand_Action_Description", typeof(Resources))]
        public virtual ModifyAction Action { get; set; }

        /// <summary>
        /// Gets or sets the new value to update with.
        /// </summary>
        [DisplayNameResource(@"ModifyXmlCommand_NewValue_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ModifyXmlCommand_NewValue_Description", typeof(Resources))]
        public virtual string NewValue { get; set; }

        /// <summary>
        /// Gets or sets the namespaces to use in the <see cref="XmlPath"/> query.
        /// </summary>
        [DisplayNameResource(@"ModifyXmlCommand_Namespaces_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ModifyXmlCommand_Namespaces_Description", typeof(Resources))]
        [TypeConverter(typeof(DesignCollectionConverter<XmlNamespace>))]
        public Collection<XmlNamespace> Namespaces { get; set; }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.ModifyXmlCommand_TraceInitial,
                    this.SourcePath, this.XmlPath, this.Action, this.NewValue);

            // Resolve SourcePath
            var sourceItem = PathResolver.ResolveToSolutionItem<IItem>(this.CurrentElement, this.Solution, this.UriReferenceService, this.SourcePath);
            if (sourceItem == null)
            {
                tracer.TraceError(
                    Resources.ModifyXmlCommand_ErrorNoSource, this.CurrentElement.InstanceName, this.SourcePath);
                return;
            }
            else
            {
                // Resolve XmlPath
                var xPath = ExpressionEvaluator.Evaluate(this.CurrentElement, this.XmlPath);
                tracer.TraceInformation(
                    Resources.ModifyXmlCommand_TraceXPath,
                        this.XmlPath, xPath);

                // Resolve NewValue
                var newValue = ExpressionEvaluator.Evaluate(this.CurrentElement, this.NewValue);
                tracer.TraceInformation(
                    Resources.ModifyXmlCommand_TraceNewValue,
                        this.NewValue, newValue);

                var sourceFilePath = sourceItem.PhysicalPath;
                var sourceLogicalPath = sourceItem.GetLogicalPath();

                this.ErrorList.Clear(sourceFilePath);

                // Load XML file
                try
                {
                    this.XmlProcessor.LoadDocument(sourceFilePath);
                }
                catch (Exception)
                {
                    tracer.TraceError(
                        Resources.ModifyXmlCommand_ErrorLoadXmlDoc, sourceFilePath);
                    throw;
                }

                // Modify XML
                try
                {

                    var updated = false;

                    // Locate the node in the XML
                    var node = this.XmlProcessor.FindFirst(xPath, this.Namespaces.ToDictionary());
                    if (node != null)
                    {
                        switch (this.Action)
                        {
                            case ModifyAction.Delete:
                                // Remove node
                                node.Remove();
                                updated = true;
                                break;

                            default:
                                // Update value
                                if (!string.IsNullOrEmpty(node.Value))
                                {
                                    if (!node.Value.Equals(newValue, StringComparison.Ordinal))
                                    {
                                        node.Value = newValue;
                                        updated = true;
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(newValue))
                                    {
                                        node.Value = newValue;
                                        updated = true;
                                    }
                                }
                                break;
                        }

                        // Save changes
                        if (updated)
                        {
                            try
                            {
                                this.XmlProcessor.SaveDocument(true);
                            }
                            catch (Exception)
                            {
                                tracer.TraceError(
                                    Resources.ModifyXmlCommand_ErrorSaveXmlDoc, sourceFilePath);
                                // Ignore exception on save, let the user save the changes in VS manually
                            }
                        }
                    }
                    else
                    {
                        tracer.TraceError(
                            Resources.ModifyXmlCommand_ErrorUpdateNodeNotFound, sourceLogicalPath, xPath);

                        // Report error to user
                        this.ErrorList.AddMessage(
                            String.Format(Resources.ModifyXmlCommand_ErrorUpdateNodeNotFound, sourceLogicalPath, xPath),
                            sourceFilePath, ErrorCategory.Error);
                    }
                }
                catch (XPathException)
                {
                    tracer.TraceError(
                        Resources.ModifyXmlCommand_ErrorXPath, sourceFilePath, xPath);
                    throw;
                }
            }
        }
    }
}
