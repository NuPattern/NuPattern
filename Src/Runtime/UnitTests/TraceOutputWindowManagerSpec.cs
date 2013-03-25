using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime.UnitTests
{
	[TestClass]
	public class TraceOutputWindowManagerSpec
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test")]
		[TestClass]
		public class GivenAManagerAndOutputWindow
		{
			private const string PaneTitle = "Foo";
			private static readonly string SourceName = Tracer.GetSourceNameFor<TraceOutputWindowManagerSpec>();
			private static readonly ITraceSource TraceSource = Tracer.GetSourceFor<TraceOutputWindowManagerSpec>();

			private Mock<IServiceProvider> serviceProvider;
			private Mock<IVsOutputWindowPane> outputPane;
			private Mock<IShellEvents> shellEvents;
			private Mock<IVsOutputWindow> outputWindow;

			private TraceOutputWindowManager traceManager;
			private Guid paneId = Guid.NewGuid();

			[TestInitialize]
			public virtual void Initialize()
			{
				this.serviceProvider = new Mock<IServiceProvider>();
				this.outputPane = new Mock<IVsOutputWindowPane>();
				this.outputWindow = new Mock<IVsOutputWindow>();
				this.shellEvents = new Mock<IShellEvents>();

				var pane = this.outputPane.Object;

				this.serviceProvider.Setup(x => x.GetService(typeof(SVsOutputWindow))).Returns(this.outputWindow.Object);
				this.outputWindow.Setup(x => x.GetPane(ref this.paneId, out pane)).Returns(0);

				this.traceManager = new TraceOutputWindowManager(this.serviceProvider.Object, this.shellEvents.Object, this.paneId, PaneTitle, SourceName);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenCreated_ThenDoesNotCreateOutputWindow()
			{
				var pane = this.outputPane.Object;

				this.outputWindow.Verify(x => x.GetPane(ref this.paneId, out pane), Times.Never());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenShellInitialized_ThenOutputWindowIsInitialized()
			{
				var pane = this.outputPane.Object;

				this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);

				this.outputWindow.Verify(x => x.GetPane(ref this.paneId, out pane));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenShellInitializedAndExistingTraces_ThenWritesToOutputPane()
			{
				var trace = Tracer.GetOrCreateUnderlyingSource(SourceName);
				trace.Switch.Level = System.Diagnostics.SourceLevels.All;
				TraceSource.TraceInformation("Hello");

				this.outputPane.Verify(x => x.OutputStringThreadSafe(It.IsAny<string>()), Times.Never());

				this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);

				this.outputPane.Verify(x => x.OutputStringThreadSafe(It.Is<string>(s => s.Contains("Hello"))));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposed_ThenNoMoreTracesAreWritten()
			{
				this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);

				this.traceManager.Dispose();

				var trace = Tracer.GetOrCreateUnderlyingSource(SourceName);
				trace.Switch.Level = System.Diagnostics.SourceLevels.All;
				TraceSource.TraceInformation("Hello");

				this.outputPane.Verify(x => x.OutputStringThreadSafe(It.IsAny<string>()), Times.Never());
			}

			[Ignore]
			[TestMethod, TestCategory("Unit")]
			public void WhenTraceSourcesSet_ThenAddsNewTracerToOutput()
			{
				// TODO: continue to investigate what's wrong with this!!!
				// Seems something weird in the underlying Tracer functionality :(
				var name = Tracer.GetSourceNameFor<TraceTestClass>();
				var trace = Tracer.GetOrCreateUnderlyingSource(name);
				trace.Switch.Level = System.Diagnostics.SourceLevels.All;

				this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);
				this.traceManager.SetTraceSourceNames(new[] { name });

				var source = Tracer.GetSourceFor<TraceTestClass>();

				source.TraceInformation("Hello");

				this.outputPane.Verify(x => x.OutputStringThreadSafe(It.Is<string>(s => s.Contains("Hello"))));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenTraceSourcesSet_ThenNoLongerTracesInitialSource()
			{
				var name = Tracer.GetSourceNameFor<TraceTestClass>();

				this.shellEvents.Raise(x => x.ShellInitialized += null, EventArgs.Empty);
				this.traceManager.SetTraceSourceNames(new[] { name });

				var trace = Tracer.GetOrCreateUnderlyingSource(SourceName);
				trace.Switch.Level = System.Diagnostics.SourceLevels.All;
				TraceSource.TraceInformation("Hello");

				this.outputPane.Verify(x => x.OutputStringThreadSafe(It.IsAny<string>()), Times.Never());
			}
		}
	}

	public class TraceTestClass
	{
	}
}
