using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace NuPattern.Diagnostics
{
    /// <summary>
    /// Provides uniformity for tracing, by providing a consistent way of
    /// logging and leveraging System.Diagnostics support. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides <see cref="ITraceSource"/> retrieval for 
    /// components that wish to log (<see cref="GetSourceFor{T}"/>) 
    /// as well as APIs for managing the underlying <see cref="TraceSource"/> 
    /// and their <see cref="TraceListener"/>s.
    /// </para>
    /// <para>
    /// When retrieving an <see cref="ITraceSource"/> via <see cref="GetSourceFor{T}"/>, 
    /// the type parameter is used to provide logging that can be customized at each namespace 
    /// "level" (i.e. if the source type full name is <c>Company.Product.MyComponent</c>, then 
    /// logging can be turned on for the entire <c>Company</c> namespace, <c>Company.Product</c> or 
    /// only specifically for <c>Company.Product.MyComponent</c>. There is also a default 
    /// trace source named '<c>*</c>' that is added automatically and allows application-wide 
    /// tracing configuration.
    /// </para>
    /// <para>
    /// The <see cref="AddListener"/> and <see cref="RemoveListener(string, string)"/> methods provide
    /// dynamic real-time updates to the trace sources, unlike built-in .NET which does not allow this. 
    /// Calls to both methods will cause updates on trace sources already created (in real time) and new 
    /// ones created from that point on.
    /// </para>
    /// </remarks>
    /// <devdoc>Provides a static facade over the <see cref="DiagnosticsTracer"/>.</devdoc>
    [DebuggerStepThrough]
    public static class Tracer
    {
        static Tracer()
        {
            Instance = new DiagnosticsTracer();
        }

        /// <summary>
        /// Makes the tracer testable by allowing the singleton to be set from tests.
        /// </summary>
        internal static DiagnosticsTracer Instance { get; set; }

        /// <summary>
        /// Enumerates the <see cref="TraceSource"/> components that 
        /// have been created so far by the tracer.
        /// </summary>
        public static IEnumerable<TraceSource> UnderlyingSources
        {
            get { return Instance.UnderlyingSources; }
        }

        /// <summary>
        /// Gets the default <see cref="TraceSource"/> that is automatically 
        /// added for all components.
        /// </summary>
        public static TraceSource DefaultUnderlyingSource
        {
            get { return Instance.DefaultUnderlyingSource; }
        }

        /// <summary>
        /// Gets an existing <see cref="TraceSource"/> with the given name, 
        /// or creates a new one if it was not created yet.
        /// </summary>
        public static TraceSource GetOrCreateUnderlyingSource(string sourceName)
        {
            return Instance.GetOrCreateUnderlyingSource(sourceName);
        }

        /// <summary>
        /// Gets the specific trace source name that will be 
        /// used for the given type.
        /// </summary>
        public static string GetSourceNameFor<T>()
        {
            return GetSourceNameFor(typeof(T));
        }

        /// <summary>
        /// Gets the specific trace source name for the given type.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static string GetSourceNameFor(Type type)
        {
            return Instance.GetSourceNameFor(type);
        }

        /// <summary>
        /// Retrieves a <see cref="ITraceSource"/> that can be
        /// used by components of type <typeparamref name="T"/> to issue
        /// trace statements.
        /// </summary>
        /// <typeparam name="T">Type of the component that will perform the logging.</typeparam>
        public static ITraceSource GetSourceFor<T>()
        {
            return GetSourceFor(typeof(T));
        }

        /// <summary>
        /// Retrieves a <see cref="ITraceSource"/> that can be
        /// used by component of type <paramref name="type"/> to issue
        /// trace statements.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static ITraceSource GetSourceFor(Type type)
        {
            return Instance.GetSourceFor(type);
        }

        /// <summary>
        /// Adds a trace listener to the underlying <see cref="TraceSource"/> with 
        /// the given <paramref name="sourceName"/>.
        /// </summary>
        public static void AddListener(string sourceName, TraceListener listener)
        {
            Instance.AddListener(sourceName, listener);
        }

        /// <summary>
        /// Removes the trace listener from the underlying <see cref="TraceSource"/> with 
        /// the given <paramref name="sourceName"/>.
        /// </summary>
        public static void RemoveListener(string sourceName, TraceListener listener)
        {
            Instance.RemoveListener(sourceName, listener);
        }

        /// <summary>
        /// Removes the trace listener from the underlying <see cref="TraceSource"/> with 
        /// the given <paramref name="sourceName"/>.
        /// </summary>
        public static void RemoveListener(string sourceName, string listenerName)
        {
            Instance.RemoveListener(sourceName, listenerName);
        }
    }
}
