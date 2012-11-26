using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Patterning.Extensibility.Properties;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Extensions to <see cref="EnvDTE.DTE"/> class.
    /// </summary>
    public static class DteExtensions
    {
        private const string VsUserRegKeyFormat = @"Software\Microsoft\VisualStudio\{0}";
        private const string VsUserRegValue = @"DefaultNewProjectLocation";
        private const string NewSolutionNamePrefix = "Solution";

        /// <summary>
        /// Gets the hierarchy.
        /// </summary>
        [CLSCompliant(false)]
        public static EnvDTE.UIHierarchy GetHierarchy(this EnvDTE.DTE dte)
        {
            Guard.NotNull(() => dte, dte);

            var solutionExplorer = dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer);
            if (solutionExplorer != null)
            {
                return solutionExplorer.Object as EnvDTE.UIHierarchy;
            }

            return null;
        }

        /// <summary>
        /// Gets the default save location of projects in the current Visual Studio environment.
        /// </summary>
        private static string GetDefaultProjectSaveLocation(IRegistryReader reader)
        {
            Guard.NotNull(()=> reader, reader);

            var value = reader.ReadValue();
            if (value != null)
            {
                return value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Gets the default save location of projects in the current Visual Studio environment.
        /// </summary>
        [CLSCompliant(false)]
        public static string GetDefaultProjectSaveLocation(this EnvDTE.DTE dte)
        {
            return GetDefaultProjectSaveLocation(new RegistryReader(Registry.CurrentUser, dte.GetCurrentVsHive(), VsUserRegValue));
        }

        private static string GetCurrentVsHive(this EnvDTE.DTE dte)
        {
            Guard.NotNull(() => dte, dte);

            return string.Format(CultureInfo.InvariantCulture, VsUserRegKeyFormat, dte.Version);
        }

        /// <summary>
        /// Creates and opens a new blank solution
        /// </summary>
        [CLSCompliant(false)]
        public static void CreateBlankSolution(this EnvDTE.DTE dte)
        {
            dte.CreateBlankSolution(new RegistryReader(Registry.CurrentUser, dte.GetCurrentVsHive(), VsUserRegValue));
        }

        /// <summary>
        /// Creates and opens a new blank solution
        /// </summary>
        internal static void CreateBlankSolution(this EnvDTE.DTE dte, IRegistryReader reader)
        {
            Guard.NotNull(()=> dte, dte);

            //Close existing solution
            if (dte.Solution.IsOpen)
            {
                dte.Solution.Close(true);
            }

            // Determine next available solution directory
            var defaultSaveLocation = GetDefaultProjectSaveLocation(reader);
            if (string.IsNullOrEmpty(defaultSaveLocation))
            {
                throw new InvalidOperationException(Resources.DteExtensions_CreateNewSolution_FailedDirSearch);
            }

            var existingSolutionFolders = Directory.GetDirectories(defaultSaveLocation).Select(dir => new DirectoryInfo(dir).Name);
            var nextSolutionDir = UniqueNameGenerator.EnsureUnique(NewSolutionNamePrefix, existingSolutionFolders, true);

            // Create solution directory
            var solutionDir = Path.Combine(defaultSaveLocation, nextSolutionDir);
            if (!Directory.Exists(solutionDir))
            {
                Directory.CreateDirectory(solutionDir);
            }

            // Save and Open new solution
            var solutionFullPath = Path.Combine(solutionDir, nextSolutionDir);
            try
            {
                dte.Solution.Create(solutionDir, nextSolutionDir);
                dte.Solution.SaveAs(solutionFullPath);
            }
            catch (COMException)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, 
                    Resources.DteExtensions_CreateNewSolution_FailedCreate, solutionDir));
            }
        }
    }
}
