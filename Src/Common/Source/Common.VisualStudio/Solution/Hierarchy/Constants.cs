using System;

namespace NuPattern.VisualStudio.Solution.Hierarchy
{
    internal static class Constants
    {
        public const string SolutionFolderType = "2150E333-8FDC-42a3-9474-1A3956D46DE8";
        public static readonly Guid SolutionFolderGuid = new Guid(EnvDTE.Constants.vsProjectItemKindVirtualFolder);
    }
}