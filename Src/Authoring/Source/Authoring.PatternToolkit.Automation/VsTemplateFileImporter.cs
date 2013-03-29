using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.Zip;
using Microsoft.Win32;
using NuPattern.Authoring.PatternToolkit.Automation.Properties;
using NuPattern.Library;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Authoring.PatternToolkit.Automation
{
    /// <summary>
    /// Performs the importing of the given files.
    /// </summary>
    internal class VsTemplateFileImporter : WindowsFileImporter
    {
        private static readonly ITraceSource tracer = Tracer.GetSourceFor<VsTemplateFileImporter>();
        internal static string TemplateFileExtension = ".zip";
        private static string VsTemplateExtension = ".vstemplate";
        private static string UserExportedTemplatesPath = "My Exported Templates";
#if VSVER10
        private static string VsSettingsRegistryKey = @"Software\Microsoft\VisualStudio\10.0";
#endif
#if VSVER11
        private static string VsSettingsRegistryKey = @"Software\Microsoft\VisualStudio\11.0";
#endif
        private static string VsSettingsRegistryValue = @"VisualStudioLocation";
        private static string exportPath;
        private string tempFolderPath;
        private IProductElement currentElement;

        /// <summary>
        /// Creates a new instance of the <see cref="VsTemplateFileImporter"/> class.
        /// </summary>
        public VsTemplateFileImporter(ISolution solution, IFxrUriReferenceService uriService, IProductElement currentElement, string targetPath)
            : base(solution, uriService, currentElement, targetPath)
        {
            this.currentElement = currentElement;
        }

        /// <summary>
        /// Starts the import.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.tempFolderPath = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
        }

        /// <summary>
        /// Cleansup the import
        /// </summary>
        public override void Cleanup()
        {
            base.Cleanup();

            // Clean up temp folder
            if (Directory.Exists(tempFolderPath))
            {
                tracer.TraceInformation(
                    Resources.VsTemplateFileImporter_TraceDeleteTempFolder, tempFolderPath);

                try
                {
                    Directory.Delete(tempFolderPath, true);
                }
                catch (UnauthorizedAccessException)
                {
                    // Ignore and continue
                }
                catch (IOException)
                {
                    // Ignore and continue
                }
            }
        }

        /// <summary>
        /// Process each dragged file.
        /// </summary>
        /// <param name="filePath"></param>
        public override bool ImportFileToSolution(string filePath)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            var unzipFolderPath = Path.Combine(this.tempFolderPath, filename);

            // Cache added file
            this.AddedItems.Add(filename, filename);

            // Unzip it
            if (UnzipTemplate(filePath, unzipFolderPath))
            {
                //Ensure filename is unique name in the container folder.
                var uniqueName = base.EnsureItemNameUniqueInTargetContainer(filename);
                if (!uniqueName.Equals(filename, StringComparison.OrdinalIgnoreCase))
                {
                    tracer.TraceInformation(
                        Resources.VsTemplateFileImporter_TraceRenamingTempUnzipFolder, filename, uniqueName);

                    // Move things around
                    this.AddedItems[filename] = uniqueName;
                    var newUnzipFolderPath = Path.Combine(this.tempFolderPath, uniqueName);
                    Directory.Move(unzipFolderPath, newUnzipFolderPath);
                    unzipFolderPath = newUnzipFolderPath;
                    filename = uniqueName;
                }

                // Rename the added (*.vstemplate) file to keep in sync with folder name
                var templateFile = new DirectoryInfo(unzipFolderPath).GetFiles()
                    .Where(f => f.Extension.Equals(VsTemplateExtension)).FirstOrDefault();
                if (templateFile != null)
                {
                    var oldFilename = Path.GetFileNameWithoutExtension(templateFile.Name);
                    if (!oldFilename.Equals(filename, StringComparison.OrdinalIgnoreCase))
                    {
                        tracer.TraceInformation(
                            Resources.VsTemplateFileImporter_TraceRenamingTempVsTemplate, TemplateFileExtension, oldFilename, filename);

                        templateFile.MoveTo(Path.Combine(unzipFolderPath, Path.ChangeExtension(filename, VsTemplateExtension)));
                    }
                }

                // Copy each unzipped folder to container folder
                var folder = this.TargetContainer.AddDirectory(unzipFolderPath);
                if (folder != null)
                {
                    // Set Build action to all items to 'None'
                    foreach (var item in folder.Traverse().OfType<IItem>())
                    {
                        tracer.TraceInformation(
                            Resources.VsTemplateFileImporter_TraceResetItemProps, item.Name);

                        item.Data.CopyToOutputDirectory = (int)CopyToOutput.DoNotCopy;
                        item.Data.ItemType = BuildAction.None.ToString();
                        item.Data.CustomTool = string.Empty;
                        item.Data.IncludeInVSIX = Boolean.FalseString.ToLower(CultureInfo.CurrentCulture);
                    }
                }

                tracer.TraceInformation(
                    Resources.VsTemplateFileImporter_TraceAddFilesComplete, filename, this.currentElement.InstanceName, folder.GetLogicalPath());

                return true;
            }

            this.AddedItems[filename] = null;
            return false;
        }

        /// <summary>
        /// Returns an item in the solution for the given dropped file.
        /// </summary>
        public override IItemContainer GetItemInSolution(string filePath)
        {
            var filename = Path.GetFileNameWithoutExtension(filePath);
            var folderName = this.AddedItems[filename];
            if (!string.IsNullOrEmpty(folderName))
            {
                // Get the newly created folder in the solution
                var templateItemFolder = this.TargetContainer.Find<IFolder>(folderName).FirstOrDefault();
                if (templateItemFolder == null)
                {
                    tracer.TraceError(
                        Resources.VsTemplateFileImporter_TraceProjectFolderNotFound, folderName, TargetContainer.GetLogicalPath());
                    return null;
                }
                else
                {
                    // Find first *.vstemplate in the folder
                    var templateFile = templateItemFolder.Find<IItem>("*" + VsTemplateExtension).FirstOrDefault();
                    if (templateFile == null)
                    {
                        tracer.TraceError(
                            Resources.VsTemplateFileImporter_TraceVsTemplateFileNotFound, VsTemplateExtension, templateItemFolder.GetLogicalPath());
                        return null;
                    }
                    else
                    {
                        tracer.TraceInformation(
                            Resources.VsTemplateFileImporter_TraceVsTemplateFound, templateFile.GetLogicalPath());

                        return templateFile as IItemContainer;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the exported template directory for VS.
        /// </summary>
        public static string ExportedTemplatesDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(exportPath))
                {
                    using (var key = Registry.CurrentUser.OpenSubKey(VsSettingsRegistryKey, false))
                    {
                        if (key != null)
                        {
                            var value = key.GetValue(VsSettingsRegistryValue);
                            if (value != null)
                            {
                                exportPath = Path.Combine(value.ToString(), UserExportedTemplatesPath);
                            }
                        }
                    }
                }

                return exportPath == null ? string.Empty : exportPath;
            }
        }

        private bool UnzipTemplate(string sourceZipFile, string unzipFolderPath)
        {
            // Try to unzip file to temp folder
            var compressor = new ZipFileDecompressor(sourceZipFile);
            try
            {
                tracer.TraceInformation(
                    Resources.VsTemplateFileImporter_TraceUnzipTemplateFile, sourceZipFile, unzipFolderPath);

                compressor.UncompressToFolder(unzipFolderPath, true);
                return true;
            }
            catch (ZipException ex)
            {
                tracer.TraceError(
                    Resources.VsTemplateFileImporter_TraceFailedToUnzipToLocation,
                    sourceZipFile, this.currentElement.InstanceName, unzipFolderPath, ex.Message);
                return false;
            }
            finally
            {
                if (compressor != null)
                {
                    compressor.Close();
                }
            }
        }
    }
}
