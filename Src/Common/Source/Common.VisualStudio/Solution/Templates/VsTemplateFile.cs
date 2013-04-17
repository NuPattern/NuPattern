using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.Zip;

namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// Provides static helper methods to manage template files 
    /// within a feature.
    /// </summary>
    [CLSCompliant(false)]
    public static class VsTemplateFile
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(NuPattern.VisualStudio.Solution.Templates.VsTemplate));

        /// <summary>
        /// The .vstemplate ProjectSubType template data that signals a template 
        /// as the one that represents a Banana (modeling) state and is used 
        /// for its instantiation, and equals the value "BananaExtension".
        /// </summary>
        public const string BananaExtensionProjectSubType = "BananaExtension";

        /// <summary>
        /// Determines whether the given template is the feature (modeling) state by 
        /// checking for its project subtype for the <see cref="BananaExtensionProjectSubType"/> value.
        /// </summary>
        public static bool IsBananaExtensionProject(NuPattern.VisualStudio.Solution.Templates.IVsTemplate template, IBananaRegistration registration)
        {
            return template.TemplateData.ProjectSubType == BananaExtensionProjectSubType || IsProjectFlavorTemplate(template);
        }

        private static bool IsProjectFlavorTemplate(NuPattern.VisualStudio.Solution.Templates.IVsTemplate template)
        {
            return false;

            //var projectContent = template.TemplateContent.Items.OfType<VSTemplateTemplateContentProject>().FirstOrDefault();
            //return template.Type == VsTemplateType.Project &&
            //    template.TemplateData.ProjectType == ModelingFeatureExtension.FeatureProjectCategory &&
            //    projectContent != null &&
            //    ReadProjectTypeGuids(Path.Combine(Path.GetDirectoryName(template.PhysicalPath), projectContent.File))
            //        .Contains(ModelingFeatureExtension.FeatureProjectFlavorGuid);
        }

        private static string ReadProjectTypeGuids(string modelingProjectFile)
        {
            try
            {
                using (var reader = XmlReader.Create(modelingProjectFile))
                {
                    reader.MoveToContent();
                    if (reader.ReadToDescendant("ProjectTypeGuids", "http://schemas.microsoft.com/developer/msbuild/2003"))
                        return reader.ReadElementContentAsString();
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads a .vstemplate file
        /// </summary>
        public static NuPattern.VisualStudio.Solution.Templates.IVsTemplate Read(string templateFilename)
        {
            if (templateFilename.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                var decompressor = new ZipFileDecompressor(templateFilename);
                try
                {
                    //
                    // See if we can find the topmost .vstemplate file.
                    // ones in sub-directories do not need to have the WizardExtension added to them
                    //
                    var templateZipEntry = decompressor.ZipFileEntries
                        .OfType<ZipEntry>()
                        .FirstOrDefault(entry =>
                            entry.FileName.EndsWith(".vstemplate", StringComparison.InvariantCultureIgnoreCase) &&
                            !entry.FileName.Contains("/")
                            );

                    if (templateZipEntry == null)
                        throw new ArgumentException("The .zip file does not contain any .vstemplate file");

                    var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    Directory.CreateDirectory(tempPath);
                    decompressor.UncompressZipEntry(templateZipEntry, tempPath, true);
                    var unzippedFile = Path.Combine(tempPath, templateZipEntry.FileName);

                    var template = ReadVsTemplate(unzippedFile);
                    template.PhysicalPath = new FileInfo(templateFilename).FullName;
                    template.TemplateFileName = Path.GetFileName(templateZipEntry.FileName);

                    return template;
                }
                finally
                {
                    decompressor.Close();
                }
            }
            else if (templateFilename.EndsWith(".vstemplate", StringComparison.InvariantCultureIgnoreCase))
            {
                return ReadVsTemplate(templateFilename);
            }

            throw new ArgumentException("Template must have a .zip or .vstemplate extension", "templateFilename");
        }

        private static NuPattern.VisualStudio.Solution.Templates.VsTemplate ReadVsTemplate(string templateFilename)
        {
            using (var reader = XmlReader.Create(templateFilename))
            {
                var template = (NuPattern.VisualStudio.Solution.Templates.VsTemplate)Serializer.Deserialize(reader);
                template.PhysicalPath = new FileInfo(templateFilename).FullName;
                template.TemplateFileName = Path.GetFileName(template.PhysicalPath);
                return template;
            }
        }

        /// <summary>
        /// Writes a .vstemplate file from the given instance.
        /// </summary>
        public static void Write(NuPattern.VisualStudio.Solution.Templates.IVsTemplate templateInstance, string templateFilename)
        {
            var hasFragment = templateFilename.Contains('?');
            if (hasFragment || templateFilename.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                var vsTemplateFileName = templateInstance.TemplateFileName;

                var tempDir = UncompressToTempDir(templateFilename);

                var tempFile = Path.Combine(tempDir, vsTemplateFileName);
                File.SetAttributes(tempFile, FileAttributes.Normal);
                // call recursivly for the .vstemplate file in the temp directory)
                Write(templateInstance, tempFile);

                VsHelper.CheckOut(templateFilename);

                new ZipFileCompressor(
                    templateFilename,
                    tempDir,
                    Directory
                        .GetFiles(tempDir, "*.*", SearchOption.AllDirectories)
                        .Select(x => x.Replace(tempDir + Path.DirectorySeparatorChar, ""))
                        .ToArray(),
                    true,
                    true);
            }
            else if (templateFilename.EndsWith(".vstemplate", StringComparison.InvariantCultureIgnoreCase))
            {
                VsHelper.CheckOut(templateFilename);

                using (var file = new StreamWriter(templateFilename, false))
                using (var writer = XmlWriter.Create(file, new XmlWriterSettings { Indent = true }))
                {
                    var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "http://schemas.microsoft.com/developer/vstemplate/2005") });
                    Serializer.Serialize(writer, templateInstance, namespaces);
                }
            }
            else
            {
                throw new ArgumentException("Template must have a .zip or .vstemplate extension", "templateFilename");
            }
        }

        private static string UncompressToTempDir(string templateFilename)
        {
            // Unzip temporarily to overwrite the .vstemplate later.
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
            if (File.Exists(tempDir)) File.Delete(tempDir);
            Directory.CreateDirectory(tempDir);

            var decompressor = new ZipFileDecompressor(templateFilename);
            try
            {
                decompressor.UncompressToFolder(tempDir);
            }
            finally
            {
                decompressor.Close();
                decompressor = null;
            }
            return tempDir;
        }

        /// <summary>
        /// Writes a .vstemplate file back to its <see cref="NuPattern.VisualStudio.Solution.Templates.IVsTemplate.PhysicalPath"/> location.
        /// </summary>
        public static void Write(NuPattern.VisualStudio.Solution.Templates.IVsTemplate templateInstance)
        {
            Write(templateInstance, templateInstance.PhysicalPath);
        }
    }
}