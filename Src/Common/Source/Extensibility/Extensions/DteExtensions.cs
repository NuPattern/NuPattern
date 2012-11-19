using System;
using System.Globalization;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Extensions to <see cref="EnvDTE.DTE"/> class.
    /// </summary>
    public static class DteExtensions
    {
        private const string VsUserRegKeyFormat = @"Software\Microsoft\VisualStudio\{0}";

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
        [CLSCompliant(false)]
        public static string GetDefaultProjectSaveLocation(this EnvDTE.DTE dte)
        {
            Guard.NotNull(() => dte, dte);
            
            var currentVsRegKey = string.Format(CultureInfo.InvariantCulture, VsUserRegKeyFormat, dte.Version);

            using (var regKey = Registry.CurrentUser.OpenSubKey(currentVsRegKey))
            {
                if (regKey != null)
                {
                    var value = regKey.GetValue(@"DefaultNewProjectLocation");
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
            }

            return null;
        }
    }
}
