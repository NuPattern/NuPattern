using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualStudio.Zip;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio.Solution.Templates
{
    /// <summary>
    /// Provides static helper methods to manage template files 
    /// within a feature.
    /// </summary>
    [CLSCompliant(false)]
    public static class VsTemplateFile
    {
        private static readonly XmlQualifiedName TemplateDefaultNamespace = new XmlQualifiedName("", "http://schemas.microsoft.com/developer/vstemplate/2005");
        private const string TemplateFileExtension = ".vstemplate";
        private const string TemplateArchiveFileExtension = ".zip";
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(VsTemplate));

        /// <summary>
        /// Reads the .vstemplate file
        /// </summary>
        public static IVsTemplate Read(string templateFilename)
        {
            if (templateFilename.EndsWith(TemplateArchiveFileExtension, StringComparison.InvariantCultureIgnoreCase))
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
                            entry.FileName.EndsWith(TemplateFileExtension, StringComparison.InvariantCultureIgnoreCase) &&
                            !entry.FileName.Contains("/")
                            );

                    if (templateZipEntry == null)
                    {
                        throw new InvalidOperationException(Resources.VsTemplateFile_ErrorNoTemplateInArchive);
                    }

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
            else if (templateFilename.EndsWith(TemplateFileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                return ReadVsTemplate(templateFilename);
            }

            throw new InvalidOperationException(Resources.VsTemplateFile_ErrorUnsupportedVsTemplateExtension);
        }

        private static VsTemplate ReadVsTemplate(string templateFilename)
        {
            using (var reader = XmlReader.Create(templateFilename))
            {
                var template = (VsTemplate)Serializer.Deserialize(reader);
                template.PhysicalPath = new FileInfo(templateFilename).FullName;
                template.TemplateFileName = Path.GetFileName(template.PhysicalPath);
                return template;
            }
        }

        /// <summary>
        /// Writes a .vstemplate file from the given instance.
        /// </summary>
        public static void Write(IVsTemplate templateInstance, string templateFilename)
        {
            var hasFragment = templateFilename.Contains('?');
            if (hasFragment || templateFilename.EndsWith(TemplateArchiveFileExtension, StringComparison.InvariantCultureIgnoreCase))
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
            else if (templateFilename.EndsWith(TemplateFileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                VsHelper.CheckOut(templateFilename);

                using (var file = new StreamWriter(templateFilename, false))
                using (var writer = XmlWriter.Create(file, new XmlWriterSettings { Indent = true }))
                {
                    var namespaces = new XmlSerializerNamespaces(new[] { TemplateDefaultNamespace });
                    Serializer.Serialize(writer, templateInstance, namespaces);
                }
            }
            else
            {
                throw new InvalidOperationException(Resources.VsTemplateFile_ErrorUnsupportedVsTemplateExtension);
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
        /// Writes a .vstemplate file back to its <see cref="IVsTemplate.PhysicalPath"/> location.
        /// </summary>
        public static void Write(IVsTemplate templateInstance)
        {
            Write(templateInstance, templateInstance.PhysicalPath);
        }
    }
}