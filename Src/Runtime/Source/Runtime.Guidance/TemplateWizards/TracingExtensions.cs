using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern;
using NuPattern.Diagnostics;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    internal static class TracingExtensions
    {
        /// <summary>
        /// Executes the given <paramref name="action"/> shielding any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="format"/> message.
        /// </summary>
        [DebuggerStepThrough]
        public static Exception Shield(this ITraceSource traceSource, Action action, string format, params string[] args)
        {
            Guard.NotNull(() => traceSource, traceSource);
            Guard.NotNull(() => action, action);
            Guard.NotNullOrEmpty(() => format, format);
            Guard.NotNull(() => args, args);

            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                {
                    throw;
                }
                else
                {
                    // Guard against mismatched formats and args
                    string message = string.Empty;
                    try
                    {
                        message = string.Format(CultureInfo.CurrentCulture, format, args);
                    }
                    catch (FormatException)
                    {
                        message = format;
                    }

                    traceSource.TraceError(ex, message);

                    var components = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                    if (components != null)
                    {
                        var services = (IServiceProvider)components.GetService<SVsServiceProvider>();
                        if (services != null)
                        {
                            Microsoft.VisualStudio.Modeling.Shell.PackageUtility.ShowError(
                                services,
                                message);
                        }
                    }

                    return ex;
                }
            }

            return null;
        }

        /// <summary>
        /// Executes the given <paramref name="action"/> shielding any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="errorMessage"/> message.
        /// </summary>
        public static Exception Shield(this ITraceSource traceSource, Action action, string errorMessage)
        {
            Guard.NotNullOrEmpty(() => errorMessage, errorMessage);

            return Shield(traceSource, action, errorMessage, new string[0]);
        }
    }
}