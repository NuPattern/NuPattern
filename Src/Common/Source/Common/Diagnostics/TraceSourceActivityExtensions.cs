using System;
using System.Globalization;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Extensions to <see cref="ITraceSource"/> for activity tracing.
    /// </summary>
    public static class TraceSourceActivityExtensions
    {
        /// <summary>
        /// Starts a new trace activity, which is automatically finished when the returned 
        /// object is disposed (enclose with a <c>using</c> statement typically).
        /// </summary>
        public static IDisposable StartActivity(this ITraceSource source, string activityName)
        {
            return new TraceActivity(activityName, source);
        }

        /// <summary>
        /// Starts a new trace activity, which is automatically finished when the returned 
        /// object is disposed (enclose with a <c>using</c> statement typically).
        /// </summary>
        public static IDisposable StartActivity(this ITraceSource source, string activityNameFormat, params object[] args)
        {
            return new TraceActivity(String.Format(CultureInfo.CurrentCulture, activityNameFormat, args), source);
        }
    }
}
