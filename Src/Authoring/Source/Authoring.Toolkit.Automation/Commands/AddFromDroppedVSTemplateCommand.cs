using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.Patterning.Authoring.Automation.Properties;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Library;
using Microsoft.VisualStudio.Patterning.Library.Commands;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;

namespace Microsoft.VisualStudio.Patterning.Authoring.Automation.Commands
{
    /// <summary>
    /// Creates a new template from a dropped *.zip template
    /// </summary>
    [DisplayNameResource("AddFromDroppedVSTemplateCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("AddFromDroppedVSTemplateCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class AddFromDroppedVSTemplateCommand : CreateElementFromDroppedWindowsFileCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<AddFromDroppedVSTemplateCommand>();
        private IWindowsFileImporter importer;

        /// <summary>
        /// Gets or sets the extension
        /// </summary>
        [Browsable(false)]
        public override string Extension
        {
            get
            {
                return VsTemplateFileImporter.TemplateFileExtension;
            }
            set
            {
                base.Extension = VsTemplateFileImporter.TemplateFileExtension;
            }
        }

        /// <summary>
        /// Gets the file importer.
        /// </summary>
        [Required]
        [Browsable(false)]
        public override IWindowsFileImporter FileImporter
        {
            get
            {
                if (this.importer == null)
                {
                    this.importer = new VsTemplateFileImporter(this.Solution, this.UriService, this.CurrentElement, this.TargetPath);
                }

                return this.importer;
            }
            set
            {
                this.importer = value;
            }
        }
    }
}
