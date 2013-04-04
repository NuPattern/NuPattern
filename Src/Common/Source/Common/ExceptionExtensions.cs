using System;
using System.Reflection;

namespace NuPattern
{
    /// <summary>
    /// Utility methods for exceptions.
    /// </summary>
    internal static class ExceptionExtensions
    {
        private static readonly FieldInfo remoteStackTraceString =
            typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic) ??
            typeof(Exception).GetField("remote_stack_trace", BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Rethrows an exception object without losing the existing stack trace information.
        /// </summary>
        /// <param name="exception">The exception to re-throw.</param>
        /// <remarks>
        /// For more information on this technique, see
        /// http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx
        /// </remarks>
        public static void RethrowWithNoStackTraceLoss(this Exception exception)
        {
            Guard.NotNull(() => exception, exception);

            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);

            throw exception;
        }
    }
}
