using System;
using System.IO;
using System.Linq;

namespace NuPattern.IO
{
    /// <summary>
    /// Builds relative paths
    /// </summary>
    public class RelativePathBuilder
    {
        private FileInfo referencePath;
        private FileInfo toConvert;

        /// <summary>
        /// Creates a new instance of the <see cref="RelativePathBuilder"/> class.
        /// </summary>
        public RelativePathBuilder(string referencePath, string toConvert)
        {
            this.referencePath = new FileInfo(referencePath);
            this.toConvert = new FileInfo(toConvert);
        }

        /// <summary>
        /// Builds the path
        /// </summary>
        /// <returns></returns>
        public string Build()
        {
            if (referencePath.Directory.Root.Name != toConvert.Directory.Root.Name)
                return toConvert.FullName;

            var directory = referencePath.DirectoryName;
            if (toConvert.FullName.StartsWith(directory))
                return toConvert.FullName.Substring(directory.Length + 1);

            var rootPaths = directory.Split(Path.DirectorySeparatorChar);
            var convertPaths = toConvert.FullName.Split(Path.DirectorySeparatorChar);

            // Find the count of paths that match between the two paths
            var matching = Enumerable.Range(0, rootPaths.Length).TakeWhile(i => rootPaths[i] == convertPaths[i]).Count();
            // Build the up-navigtion from the reference.
            var result = String.Join(Path.DirectorySeparatorChar.ToString(), rootPaths.Skip(matching).Select(s => @".."));
            // Biuld the down-navigation to the target.
            result = String.Join(Path.DirectorySeparatorChar.ToString(), new[] { result }.Concat(convertPaths.Skip(matching)));

            return result;
        }
    }
}
