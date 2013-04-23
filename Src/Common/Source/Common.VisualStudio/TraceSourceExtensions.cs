using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Modeling.Shell;
using Microsoft.VisualStudio.Shell;
using NuPattern.Diagnostics;
using NuPattern.VisualStudio.Properties;

namespace NuPattern.VisualStudio
{
    /// <summary>
    /// Provides tracing extensions on top of <see cref="ITraceSource"/>.
    /// </summary>
    public static class TraceSourceExtensions
    {
        /// <summary>
        /// Executes the given <paramref name="action"/> shielding to the UI any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="errorMessage"/> message.
        /// </summary>
        [CLSCompliant(false)]
        public static Exception ShieldUI(this ITraceSource traceSource, Action action, string errorMessage)
        {
            Guard.NotNullOrEmpty(() => errorMessage, errorMessage);

            return ShieldUI(traceSource, action, errorMessage, new string[0]);
        }

        /// <summary>
        /// Executes the given <paramref name="action"/> shielding to the UI any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="format"/> message.
        /// </summary>
        [DebuggerStepThrough]
        [CLSCompliant(false)]
        public static Exception ShieldUI(this ITraceSource traceSource, Action action, string format, params string[] args)
        {
            Guard.NotNull(() => traceSource, traceSource);
            Guard.NotNull(() => action, action);
            Guard.NotNullOrEmpty(() => format, format);
            Guard.NotNull(() => args, args);

            return DoShield(traceSource, action, format, true, args);
        }

        /// <summary>
        /// Executes the given <paramref name="action"/> shielding any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="errorMessage"/> message.
        /// </summary>
        [CLSCompliant(false)]
        public static Exception Shield(this ITraceSource traceSource, Action action, string errorMessage)
        {
            Guard.NotNullOrEmpty(() => errorMessage, errorMessage);

            return Shield(traceSource, action, errorMessage, new string[0]);
        }

        /// <summary>
        /// Executes the given <paramref name="action"/> shielding to any non-critical exceptions 
        /// and logging them to the <paramref name="traceSource"/> with the given <paramref name="format"/> message.
        /// </summary>
        [DebuggerStepThrough]
        [CLSCompliant(false)]
        public static Exception Shield(this ITraceSource traceSource, Action action, string format, params string[] args)
        {
            Guard.NotNull(() => traceSource, traceSource);
            Guard.NotNull(() => action, action);
            Guard.NotNullOrEmpty(() => format, format);
            Guard.NotNull(() => args, args);

            return DoShield(traceSource, action, format, false, args);
        }

        [DebuggerStepThrough]
        private static Exception DoShield(this ITraceSource traceSource, Action action, string format, bool showUI, params string[] args)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (Microsoft.VisualStudio.ErrorHandler.IsCriticalException(ex))
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

                    if (showUI)
                    {
                        ShowExceptionAction(message, ex);
                    }

                    return ex;
                }
            }

            return null;
        }

        internal static Action<string, Exception> ShowExceptionAction = (message, ex) =>
        {
            var components = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            if (components != null)
            {
                var services = (IServiceProvider)components.GetService<SVsServiceProvider>();
                if (services != null)
                {
                    PackageUtility.ShowError(
                        services,
                        message + Environment.NewLine + Environment.NewLine + Resources.TraceSourceExtensions_SeeDiagnosticsWindow);
                }
            }
        };
    }
}