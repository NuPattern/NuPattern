using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Shell.Interop;
using NuPattern.Diagnostics;
using NuPattern.Runtime.Properties;
using NuPattern.VisualStudio;
using NuPattern.VisualStudio.Diagnostics;

namespace NuPattern.Runtime.Diagnostics
{
    /// <summary>
    ///  Manages the output of trace messages to an output window pane.
    /// </summary>
    internal sealed class TraceOutputWindowPane : ITracePane, IDisposable
    {
        private static readonly ITracer tracer = Tracer.Get<TraceOutputWindowPane>();
        private static readonly TraceFilter defaultFilter = new DelegateTraceFilter((cache, source, eventType, id) => true);

        private IServiceProvider serviceProvider;
        private Guid outputPaneGuid;
        private IVsOutputWindowPane outputWindowPane;
        private TraceListener listener;
        private StringWriter temporaryWriter;
        private IShellEvents shellEvents;
        private string[] traceSourceNames;
        private string outputPaneTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputWindowPane"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="shellEvents">The shell events.</param>
        /// <param name="outputPaneId">The output pane GUID, which must be unique and remain constant for a given pane.</param>
        /// <param name="outputPaneTitle">The output pane title.</param>
        /// <param name="traceSourceName">The name of the trace source to write to the output window pane.</param>
        public TraceOutputWindowPane(IServiceProvider serviceProvider, IShellEvents shellEvents, Guid outputPaneId, string outputPaneTitle, string traceSourceName)
            : this(serviceProvider, shellEvents, outputPaneId, outputPaneTitle, new[] { traceSourceName })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceOutputWindowPane"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="shellEvents">The shell events.</param>
        /// <param name="outputPaneId">The output pane GUID, which must be unique and remain constant for a given pane.</param>
        /// <param name="outputPaneTitle">The output pane title.</param>
        /// <param name="traceSourceNames">The names of the trace sources to write to the output window pane.</param>
        public TraceOutputWindowPane(IServiceProvider serviceProvider, IShellEvents shellEvents, Guid outputPaneId, string outputPaneTitle, params string[] traceSourceNames)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => shellEvents, shellEvents);
            Guard.NotNullOrEmpty(() => outputPaneTitle, outputPaneTitle);
            Guard.NotNull(() => traceSourceNames, traceSourceNames);

            this.serviceProvider = serviceProvider;
            this.outputPaneGuid = outputPaneId;
            this.outputPaneTitle = outputPaneTitle;
            this.traceSourceNames = traceSourceNames;
            this.shellEvents = shellEvents;

            this.shellEvents.ShellInitialized += this.OnShellInitialized;

            // Create a temporary writer that buffers events that happen 
            // before shell initialization is completed, so that we don't 
            // miss anything.
            this.temporaryWriter = new StringWriter(CultureInfo.CurrentCulture);
            this.listener = new TextWriterTraceListener(this.temporaryWriter, this.outputPaneTitle);
            this.listener.IndentLevel = 4;
            this.listener.Filter = defaultFilter;

            this.AddListenerToSources();
        }

        /// <summary>
        /// Gets the identifier of the trace pane.
        /// </summary>
        public Guid TracePaneId
        {
            get { return this.outputPaneGuid; }
        }

        /// <summary>
        /// Sets the names of the trace sources.
        /// </summary>
        /// <param name="sourceNames">The names of the trace sources.</param>
        public void SetTraceSourceNames(IEnumerable<string> sourceNames)
        {
            Guard.NotNull(() => sourceNames, sourceNames);

            this.RemoveListenerFromSources();
            this.traceSourceNames = sourceNames.ToArray();
            this.AddListenerToSources();
        }

        /// <summary>
        /// Cleans resources used by the manager.
        /// </summary>
        public void Dispose()
        {
            this.shellEvents.Dispose();
            if (this.listener != null)
            {
                this.RemoveListenerFromSources();
                this.listener.Dispose();
                this.listener = null;
            }

            if (this.temporaryWriter != null)
            {
                this.temporaryWriter.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "We dispose the listener, which disposes the internal writer.")]
        private void OnShellInitialized(object sender, EventArgs args)
        {
            this.EnsureOutputWindow();

            // Replace temporary listener with the proper one, populating the 
            // output window from the temporary buffer.
            var tempLog = this.temporaryWriter.ToString();

            if (!string.IsNullOrEmpty(tempLog))
            {
                Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(this.outputWindowPane.OutputStringThreadSafe(this.temporaryWriter.ToString()));
            }

            this.temporaryWriter = null;

            this.RemoveListenerFromSources();

            this.listener = new ActivityTextListener(new OutputWindowTextWriter(this.outputWindowPane), this.outputPaneTitle);
            this.listener.IndentLevel = 4;
            this.listener.Filter = defaultFilter;

            this.AddListenerToSources();
        }

        private void AddListenerToSources()
        {
            foreach (var sourceName in this.traceSourceNames)
            {
                Tracer.Manager.AddListener(sourceName, this.listener);
            }
        }

        private void RemoveListenerFromSources()
        {
            foreach (var sourceName in this.traceSourceNames)
            {
                Tracer.Manager.RemoveListener(sourceName, this.listener);
            }
        }

        private void EnsureOutputWindow()
        {
            if (this.outputWindowPane == null)
            {
                var outputWindow = (IVsOutputWindow)this.serviceProvider.GetService(typeof(SVsOutputWindow));
                tracer.ShieldUI(() =>
                {
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(outputWindow.CreatePane(ref this.outputPaneGuid, this.outputPaneTitle, 1, 1));
                    Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(outputWindow.GetPane(ref this.outputPaneGuid, out this.outputWindowPane));
                },
                Resources.TraceOutput_FailedToCreateOutputWindow);
            }
        }
    }
}
