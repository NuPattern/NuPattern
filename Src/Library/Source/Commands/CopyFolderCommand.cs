using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms.Design;
using Microsoft.VisualBasic.FileIO;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.Commands
{
    /// <summary>
    /// Activates (opens or selects) the linked artifacts associated to current element.
    /// </summary>
    [DisplayNameResource("CopyFolderCommand_DisplayName", typeof(Resources))]
    [DescriptionResource("CopyFolderCommand_Description", typeof(Resources))]
    [CategoryResource("AutomationCategory_General", typeof(Resources))]
    [CLSCompliant(false)]
    public class CopyFolderCommand : Command
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<CopyFolderCommand>();
        private const bool DefaultOverwriteIfExists = true;

        /// <summary>
        /// Create a new instance of the <see cref="CopyFolderCommand"/> class.
        /// </summary>
        public CopyFolderCommand()
        {
            this.OverwriteIfExists = DefaultOverwriteIfExists;
        }

        /// <summary>
        /// Gets or sets the solution.
        /// </summary>
        [Required]
        [Import(AllowDefault = true)]
        public ISolution Solution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to overwrite the destination folder if it exists.
        /// </summary>
        [DisplayNameResource("CopyFolderCommand_OverwriteIfExists_DisplayName", typeof(Resources))]
        [DescriptionResource("CopyFolderCommand_OverwriteIfExists_Description", typeof(Resources))]
        [DefaultValue(DefaultOverwriteIfExists)]
        [Required]
        public bool OverwriteIfExists { get; set; }

        /// <summary>
        /// Gets or sets the source folder to copy from.
        /// </summary>
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [DisplayNameResource("CopyFolderCommand_SourcePath_DisplayName", typeof(Resources))]
        [DescriptionResource("CopyFolderCommand_SourcePath_Description", typeof(Resources))]
        [Required]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the destination folder to copy to.
        /// </summary>
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        [DisplayNameResource("CopyFolderCommand_DestinationPath_DisplayName", typeof(Resources))]
        [DescriptionResource("CopyFolderCommand_DestinationPath_Description", typeof(Resources))]
        [Required]
        public string DestinationPath { get; set; }

        /// <summary>
        /// Executes the activation behavior.
        /// </summary>
        public override void Execute()
        {
            this.ValidateObject();

            tracer.TraceInformation(
                Resources.CopyFolderCommand_TraceInitial, this.SourcePath, this.DestinationPath, this.OverwriteIfExists);

            var sourcePath = Environment.ExpandEnvironmentVariables(this.SourcePath);
            var destinationPath = Environment.ExpandEnvironmentVariables(this.DestinationPath);

            if (!Directory.Exists(sourcePath))
            {
                tracer.TraceError(
                    Resources.CopyFolderCommand_TraceSourceNotFound, this.SourcePath);
                return;
            }

            if (!Directory.Exists(destinationPath))
            {
                tracer.TraceInformation(
                    Resources.CopyFolderCommand_TraceDestinationNotFound, this.DestinationPath);

                FileSystem.CreateDirectory(destinationPath);

                tracer.TraceInformation(
                    Resources.CopyFolderCommand_TraceCopying, this.SourcePath, this.DestinationPath);

                FileSystem.CopyDirectory(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
            }
            else
            {
                if (this.OverwriteIfExists)
                {
                    tracer.TraceInformation(
                        Resources.CopyFolderCommand_TraceCopying, this.SourcePath, this.DestinationPath);

                    FileSystem.CopyDirectory(sourcePath, destinationPath, UIOption.AllDialogs, UICancelOption.ThrowException);
                }
                else
                {
                    tracer.TraceInformation(
                        Resources.CopyFolderCommand_TraceDestinationExistsNoOverwrite, this.DestinationPath);
                }
            }
        }
    }
}