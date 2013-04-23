using System;
using System.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Abstraction of <see cref="TraceSource"/> used by
    /// components that want to statically cache their
    /// own instances of the trace source.
    /// </summary>
    /// <remarks>
    /// If you're using <see cref="Tracer"/> tracing methods
    /// directly, you don't need to use this interface.
    /// </remarks>
    /// <example>
    /// In this example, a component is caching statically
    /// an instance of an <see cref="ITraceSource"/> to
    /// reuse it whenever it needs to trace information:
    /// <code>
    /// public class MyComponent
    /// {
    ///             static readonly ITraceSource tracer = Tracer.GetSourceFor&lt;MyComponent&gt;();
    ///            
    ///             public MyComponent()
    ///             {
    ///                     tracer.TraceInformation("MyComponent constructed!");
    ///             }
    /// }
    /// </code>
    /// </example>
    /// <devdoc>
    /// This interface is basically extracted from <see cref="TraceSource"/>.
    /// </devdoc>
    public interface ITraceSource
    {
        /// <summary>
        /// See <see cref="TraceSource.Name"/>.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// See <see cref="TraceSource.Flush"/>.
        /// </summary>
        void Flush();
        /// <summary>
        /// See <see cref="TraceData(TraceEventType, int, object)"/>.
        /// </summary>
        void TraceData(TraceEventType eventType, int id, object data);
        /// <summary>
        /// See <see cref="TraceSource.TraceEvent(TraceEventType, int)"/>.
        /// </summary>
        void TraceEvent(TraceEventType eventType, int id);
        /// <summary>
        /// See <see cref="TraceSource.TraceEvent(TraceEventType, int, string, object[])"/>.
        /// </summary>
        void TraceEvent(TraceEventType eventType, int id, string format, params object[] args);
        /// <summary>
        /// See <see cref="TraceSource.TraceEvent(TraceEventType, int, string)"/>.
        /// </summary>
        void TraceEvent(TraceEventType eventType, int id, string message);
        /// <summary>
        /// See <see cref="TraceSource.TraceInformation(string)"/>.
        /// </summary>
        void TraceInformation(string message);
        /// <summary>
        /// See <see cref="TraceSource.TraceInformation(string, object[])"/>.
        /// </summary>
        void TraceInformation(string format, params object[] args);
        /// <summary>
        /// Issues a <see cref="TraceEvent(TraceEventType, Int32, String, Object[])"/> with a <see cref="TraceEventType.Verbose"/> type.
        /// </summary>
        void TraceVerbose(string message);
        /// <summary>
        /// Issues a <see cref="TraceEvent(TraceEventType, Int32, String, Object[])"/> with a <see cref="TraceEventType.Verbose"/> type.
        /// </summary>
        void TraceVerbose(string format, params object[] args);
        /// <summary>
        /// See <see cref="TraceSource.TraceTransfer(int, string, Guid)"/>.
        /// </summary>
        void TraceTransfer(int id, string message, Guid relatedActivityId);
        /// <summary>
        /// Traces the given error message, using the format and arguments.
        /// </summary>
        void TraceError(string format, params object[] args);
        /// <summary>
        /// Traces the given error message.
        /// </summary>
        void TraceError(string message);
        /// <summary>
        /// Traces the given exception, using the format and arguments.
        /// </summary>
        void TraceError(Exception exception, string format, params object[] args);
        /// <summary>
        /// Traces the given exception and its corresponding message.
        /// </summary>
        void TraceError(Exception exception, string message);
        /// <summary>
        /// Traces a warning, using the format and arguments to build the message.
        /// </summary>
        void TraceWarning(string format, params object[] args);
        /// <summary>
        /// Traces a warning with the given message.
        /// </summary>
        void TraceWarning(string message);
    }
}
