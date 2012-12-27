using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NuPattern.Runtime.UnitTests
{
	[TestClass]
	public class TracingSettingsMonitorSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Tests")]
		[TestClass]
		public class GivenAMonitorAndASettingsManager
		{
			private Mock<ISettingsManager> manager;
			private TracingSettingsMonitor monitor;
			private TraceSource source;

			[TestInitialize]
			public void Initialize()
			{
				this.manager = new Mock<ISettingsManager>();
				this.monitor = new TracingSettingsMonitor(this.manager.Object);

				this.source = Tracer.GetOrCreateUnderlyingSource("Foo");
				this.source.Switch.Level = SourceLevels.Off;
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNewSettingsApplied_ThenTraceSourceLevelIsSet()
			{
				var setting = new RuntimeSettings();
				setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Warning));

				this.manager.Raise(x => x.SettingsChanged += null, new ChangedEventArgs<RuntimeSettings>(new RuntimeSettings(), setting));

				Assert.Equal(SourceLevels.Warning, this.source.Switch.Level);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenDisposingMonitor_ThenStopsTrackingChanges()
			{
				var setting = new RuntimeSettings();
				setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Information));

				this.monitor.Dispose();

				this.manager.Raise(x => x.SettingsChanged += null, new ChangedEventArgs<RuntimeSettings>(new RuntimeSettings(), setting));

				Assert.Equal(SourceLevels.Off, this.source.Switch.Level);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Tests")]
		[TestClass]
		public class GivenAMonitorAndASettingsManagerWithInitialSettings
		{
			private Mock<ISettingsManager> manager;
			private TracingSettingsMonitor monitor;
			private TraceSource source;

			[TestInitialize]
			public void Initialize()
			{
				this.source = Tracer.GetOrCreateUnderlyingSource("Foo");
				this.source.Switch.Level = SourceLevels.Off;

				this.manager = new Mock<ISettingsManager>();

				var setting = new RuntimeSettings();
				setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Warning));

				this.manager.Setup(x => x.Read()).Returns(setting);

				this.monitor = new TracingSettingsMonitor(this.manager.Object);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenConstructed_ThenInitialTraceSourceLevelIsApplied()
			{
				Assert.Equal(SourceLevels.Warning, this.source.Switch.Level);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenConstructed_ThenDefaultSourceLevelIsApplied()
			{
				var defaultSource = Tracer.GetOrCreateUnderlyingSource(TracingSettings.DefaultRootSourceName);

				Assert.Equal(TracingSettings.DefaultRootSourceLevel, defaultSource.Switch.Level);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNewSettingsApplied_ThenTraceSourceLevelIsSet()
			{
				var setting = new RuntimeSettings();
				setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Information));

				this.manager.Raise(x => x.SettingsChanged += null, new ChangedEventArgs<RuntimeSettings>(new RuntimeSettings(), setting));

				Assert.Equal(SourceLevels.Information, this.source.Switch.Level);
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNewSettingsApplied_ThenDefaultTraceListenerIsRestored()
			{
				var setting = new RuntimeSettings();
				setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Information));

				this.manager.Raise(x => x.SettingsChanged += null, new ChangedEventArgs<RuntimeSettings>(new RuntimeSettings(), setting));

				var source = Tracer.GetOrCreateUnderlyingSource("Foo");

				Assert.True(source.Listeners.Cast<TraceListener>().OfType<DefaultTraceListener>().Any());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenNewRootDefaultLevelIsApplied_ThenDefaultSourceLevelIsApplied()
			{
				var setting = new RuntimeSettings();
				setting.Tracing.RootSourceLevel = SourceLevels.ActivityTracing;

				this.manager.Raise(x => x.SettingsChanged += null, new ChangedEventArgs<RuntimeSettings>(new RuntimeSettings(), setting));

				var defaultSource = Tracer.GetOrCreateUnderlyingSource(TracingSettings.DefaultRootSourceName);

				Assert.Equal(SourceLevels.ActivityTracing, defaultSource.Switch.Level);
			}
		}
	}
}
