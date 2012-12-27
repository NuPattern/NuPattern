using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace NuPattern.Library
{
    /// <summary>
    /// Extension for helping with Drag and Drop automation.
    /// </summary>
    public static class DragDropHelpers
    {
        internal const char ExtensionDelimiter = ';';
        internal const string ProjectItemDataFormat = "CF_VSSTGPROJECTITEMS";
        internal const string ProjectDataFormat = "CF_VSREFPROJECTS";

        /// <summary>
        /// Returns the paths of all project items in the dragged data.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static IEnumerable<string> GetVSProjectItemsPaths(this DragEventArgs dragArgs)
        {
            Guard.NotNull(() => dragArgs, dragArgs);

            if (dragArgs.Data.GetDataPresent(DragDropHelpers.ProjectItemDataFormat))
            {
                using (var stream = dragArgs.Data.GetData(ProjectItemDataFormat) as MemoryStream)
                {
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd()
                            .Split('\0')
                            .Where(s => s.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                            .Select(p => GetProperCasedName(p.Split('|')[2]));
                    }
                }
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Returns the paths of all projects in the dragged data.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static IEnumerable<string> GetVSProjectsPaths(this DragEventArgs dragArgs)
        {
            Guard.NotNull(() => dragArgs, dragArgs);

            if (dragArgs.Data.GetDataPresent(DragDropHelpers.ProjectDataFormat))
            {
                using (var stream = dragArgs.Data.GetData(ProjectDataFormat) as MemoryStream)
                {
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd()
                            .Split('\0')
                            .Where(s => s.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                            .Select(p => GetProperCasedName(string.Concat(p.Split('|')[2], p.Split('|')[1])));
                    }
                }
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Gets the paths of all files in the dragged data.
        /// </summary>
        public static IEnumerable<string> GetWindowsFilePaths(this DragEventArgs dragArgs)
        {
            Guard.NotNull(() => dragArgs, dragArgs);

            if (dragArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return (dragArgs.Data.GetData(DataFormats.FileDrop) as string[])
                    .Select(f => GetProperCasedName(f));
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Filters the given file paths with only those that end with one or more of the given file extensions.
        /// </summary>
        /// <remarks>
        /// Allows extension in any of the following formats:
        ///		"cs", ".cs", "*.cs" or "cs;xml", ".cs;.xml", "*.cs;*.xml" or any combination of these.
        /// </remarks>
        /// <returns></returns>
        public static IEnumerable<string> GetPathsEndingWithExtensions(this IEnumerable<string> paths, string extensions)
        {
            var exts = GetSafeExtensions(extensions);

            return paths.Where(path => exts.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Gets a list of safe extensions from given extension.
        /// </summary>
        /// <remarks>
        /// Allows extension in any of the following formats:
        ///		"cs", ".cs", "*.cs" or "cs;xml", ".cs;.xml", "*.cs;*.xml" or any combination of these.
        /// </remarks>
        /// <returns>A list of extensions, each starting with a '.' character</returns>
        public static IEnumerable<string> GetSafeExtensions(string extensions)
        {
            Guard.NotNull(() => extensions, extensions);

            return extensions
                .Replace("*", string.Empty)
                .Replace(" ", string.Empty)
                .Split(new[] { ExtensionDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.StartsWith(".", StringComparison.OrdinalIgnoreCase) ? s : "." + s);
        }

        private static string GetProperCasedName(DirectoryInfo dirInfo)
        {
            DirectoryInfo parentDirInfo = dirInfo.Parent;
            if (null == parentDirInfo)
                return dirInfo.Name;
            return Path.Combine(GetProperCasedName(parentDirInfo),
                                parentDirInfo.GetDirectories(dirInfo.Name)[0].Name);
        }

        private static string GetProperCasedName(FileInfo fileInfo)
        {
            DirectoryInfo dirInfo = fileInfo.Directory;
            return Path.Combine(GetProperCasedName(dirInfo),
                                dirInfo.GetFiles(fileInfo.Name)[0].Name);
        }

        private static string GetProperCasedName(string path)
        {
            if (File.Exists(path))
            {
                return GetProperCasedName(new FileInfo(path));
            }
            else if (Directory.Exists(path))
            {
                return GetProperCasedName(new DirectoryInfo(path));
            }
            else
            {
                return path;
            }
        }
    }
}
