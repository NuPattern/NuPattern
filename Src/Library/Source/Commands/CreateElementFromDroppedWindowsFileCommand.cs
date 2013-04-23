using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Creates a new instance of a child element for each dropped explorer file.
    /// </summary>
    [DisplayNameResource(@"CreateElementFromDroppedWindowsFileCommand_DisplayName", typeof(Resources))]
    [DescriptionResource(@"CreateElementFromDroppedWindowsFileCommand_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_Automation", typeof(Resources))]
    [CLSCompliant(false)]
    public class CreateElementFromDroppedWindowsFileCommand : CreateElementFromDroppedFileCommand
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CreateElementFromDroppedWindowsFileCommand>();
        private IWindowsFileImporter importer;

        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"CreateElementFromDroppedWindowsFileCommand_TargetPath_DisplayName", typeof(Resources))]
        [DescriptionResource(@"CreateElementFromDroppedWindowsFileCommand_TargetPath_Description", typeof(Resources))]
        public virtual string TargetPath { get; set; }

        /// <summary>
        /// Executes the command
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CreateElementFromDroppedWindowsFileCommand_TraceInitial, this.CurrentElement.InstanceName, this.ChildElementName, this.Extension);

            try
            {
                this.FileImporter.Initialize();
                base.Execute();
            }
            finally
            {
                this.FileImporter.Cleanup();
            }
        }

        /// <summary>
        /// Creates and returns an instance of a file importer.
        /// </summary>
        [Required]
        [Browsable(false)]
        public virtual IWindowsFileImporter FileImporter
        {
            get
            {
                if (this.importer == null)
                {
                    this.importer = new WindowsFileImporter(this.Solution, this.UriService, this.CurrentElement, this.TargetPath);
                }

                return this.importer;
            }
            set
            {
                this.importer = value;
            }
        }

        /// <summary>
        /// Add each dragged file to the solution.
        /// </summary>
        /// <param name="filePath"></param>
        protected override bool AddFileToSolution(string filePath)
        {
            return this.FileImporter.ImportFileToSolution(filePath);
        }

        /// <summary>
        /// Returns an item in the solution for the given dropped file.
        /// </summary>
        protected override IItemContainer GetItemInSolution(string filePath)
        {
            return this.FileImporter.GetItemInSolution(filePath);
        }
    }
}
