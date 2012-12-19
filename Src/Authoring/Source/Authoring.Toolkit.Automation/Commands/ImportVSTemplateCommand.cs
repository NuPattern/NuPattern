using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using NuPattern.Authoring.Automation.Properties;
using NuPattern.Extensibility;
using NuPattern.Library;
using NuPattern.Library.Commands;

namespace NuPattern.Authoring.Automation.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each imported VS template file.
    /// </summary>
    [DisplayNameResource("ImportVSTemplateCommand_DisplayName", typeof(Resources))]
    [CategoryResource("AutomationCategory_PatternToolkitAuthoring", typeof(Resources))]
    [DescriptionResource("ImportVSTemplateCommand_Description", typeof(Resources))]
    [CLSCompliant(false)]
    public class ImportVSTemplateCommand : CreateElementFromPickedWindowsFileCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<ImportVSTemplateCommand>();
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
        /// Gets or sets the initial directory
        /// </summary>
        [Browsable(false)]
        public override string InitialDirectory
        {
            get
            {
                return VsTemplateFileImporter.ExportedTemplatesDirectory;
            }
            set
            {
                base.InitialDirectory = VsTemplateFileImporter.ExportedTemplatesDirectory;
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
