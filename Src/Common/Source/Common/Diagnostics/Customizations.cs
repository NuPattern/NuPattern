namespace NuPattern.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    static partial class Tracer
    {
        /// <summary>
        /// Retrieves the configured <see cref="ITracerManager"/>.
        /// </summary>
        public static ITracerManager Manager 
        { 
            get { return manager; }
            set { manager = value ?? new DefaultManager(); }
        }

        partial class DefaultManager
        {
            public TraceSource GetSource(string name)
            {
                throw new NotImplementedException();
            }

            public void AddListener(string sourceName, TraceListener listener)
            {
                throw new NotImplementedException();
            }

            public void RemoveListener(string sourceName, TraceListener listener)
            {
                throw new NotImplementedException();
            }
        }
    }

    partial interface ITracerManager
    {
        /// <summary>
        /// Gets the underlying <see cref="TraceSource"/> for the given name.
        /// </summary>
        TraceSource GetSource(string name);

        /// <summary>
        /// Adds a listener to the source with the given <paramref name="sourceName"/>.
        /// </summary>
        void AddListener(string sourceName, TraceListener listener);

        /// <summary>
        /// Removes a listener from the source with the given <paramref name="sourceName"/>.
        /// </summary>
        void RemoveListener(string sourceName, TraceListener listener);
    }
}