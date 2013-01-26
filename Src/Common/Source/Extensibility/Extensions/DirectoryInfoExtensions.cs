using System;
using System.Collections.Generic;
using System.IO;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Extensions to the <see cref="DirectoryInfo"/> class.
    /// </summary>
    internal static class DirectoryInfoExtensions
    {
    }

    /// <summary>
    /// Compare two <see cref="DirectoryInfo"/> instances to see of they are the same directory.
    /// </summary>
    internal class DirectoryInfoComparer : IEqualityComparer<DirectoryInfo>
    {
        /// <summary>
        /// Determines whether the given directories are the same.
        /// </summary>
        public bool Equals(DirectoryInfo dir1, DirectoryInfo dir2)
        {
            if (dir1 == null || dir2 == null)
            {
                return false;
            }

            var path1 = Path.Combine(dir1.Parent.FullName, dir1.Name);
            var path2 = Path.Combine(dir2.Parent.FullName, dir2.Name);

            return (path1.Equals(path2, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Creates a unique has for the directory.
        /// </summary>
        public int GetHashCode(DirectoryInfo dir)
        {
            string s = Path.Combine(dir.Parent.FullName, dir.Name);
            return s.GetHashCode();
        }
    }
}
