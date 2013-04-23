using System;

namespace NuPattern.VisualStudio.Solution
{
    /// <summary>
    /// The kind of item.
    /// </summary>
    [Flags]
    public enum ItemKind
    {
        /// <summary>
        /// The item is a <see cref="ISolution"/>.
        /// </summary>
        Solution = 1,
        /// <summary>
        /// The item is a <see cref="ISolutionFolder"/>.
        /// </summary>
        SolutionFolder = 2,
        /// <summary>
        /// The item is a <see cref="IProject"/>.
        /// </summary>
        Project = 4,
        /// <summary>
        /// The item is a <see cref="IFolder"/>.
        /// </summary>
        Folder = 8,
        /// <summary>
        /// The item is a <see cref="IItem"/>.
        /// </summary>
        Item = 16,
        /// <summary>
        /// The item is a reference within the references folder.
        /// </summary>
        Reference = 32,
        /// <summary>
        /// The item is a the references folder within a project.
        /// </summary>
        ReferencesFolder = 64,
        /// <summary>
        /// The item anything within a Solution including the Solution itself.
        /// </summary>
        Any = Solution | SolutionFolder | Project | Folder | Item | Reference | ReferencesFolder,
        /// <summary>
        /// The item is of an unknown type.
        /// </summary>
        Unknown = 128
    }
}
