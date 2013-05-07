using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Diagnostics
{
    /// <summary>
    /// Implments the <see cref="ITraceOutputWindowManager"/> services.
    /// </summary>
    [Export(typeof(ITraceOutputWindowManager))]
    internal class TraceOutputWindowManager : ITraceOutputWindowManager, IDisposable
    {
        private ICollection<TraceOutputWindowPane> tracePanes;
        private IServiceProvider serviceProvider;
        private IShellEvents shellEvents;

        /// <summary>
        /// Creates a new instance of the <see cref="TraceOutputWindowManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public TraceOutputWindowManager(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            [Import(typeof(IShellEvents))] IShellEvents shellEvents)
        {
            this.tracePanes = new List<TraceOutputWindowPane>();
            this.serviceProvider = serviceProvider;
            this.shellEvents = shellEvents;
        }

        /// <summary>
        /// Creates a new trace pane.
        /// </summary>
        /// <param name="traceId">The identifier of the trace pane</param>
        /// <param name="title">The title to display for the  trace pane</param>
        /// <param name="traceSourceNames">The names of the sources to display in this trace window</param>
        public ITracePane CreateTracePane(Guid traceId, string title, IEnumerable<string> traceSourceNames)
        {
            Guard.NotNullOrEmpty(() => title, title);
            Guard.NotNull(() => traceSourceNames, traceSourceNames);

            if (this.tracePanes.Any(tp => tp.TracePaneId == traceId))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.TraceOutputWindowManager_ErrorDuplicateTraceId, traceId));
            }

            // Add tracer pane
            var tracePane = new TraceOutputWindowPane(this.serviceProvider, this.shellEvents, traceId, title,
                                                      traceSourceNames.ToArray());
            this.tracePanes.Add(tracePane);

            return tracePane;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.tracePanes != null)
            {
                this.tracePanes.ForEach(tp => tp.Dispose());
            }
        }

        /// <summary>
        /// Gets the trace panes.
        /// </summary>
        public IEnumerable<ITracePane> TracePanes
        {
            get { return this.tracePanes; }
        }
    }
}
