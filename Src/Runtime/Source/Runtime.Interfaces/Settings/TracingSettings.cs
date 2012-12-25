using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents tracing-related settings.
	/// </summary>
	public class TracingSettings
	{
		/// <summary>
		/// Default <see cref="SourceLevels"/> logging level for the catch-all 
		/// root trace source ("NuPattern"), which equals <see cref="SourceLevels.Warning"/>.
		/// </summary>
		public const SourceLevels DefaultRootSourceLevel = SourceLevels.Warning;

		/// <summary>
		/// Default root source name for the runtime and all above layers, which equals "NuPattern".
		/// </summary>
		public static readonly string DefaultRootSourceName = typeof(Guard).Namespace;

		/// <summary>
		/// Initializes a new instance of the <see cref="TracingSettings"/> class.
		/// </summary>
		public TracingSettings()
		{
			this.TraceSources = new BindingList<TraceSourceSetting>();
			this.RootSourceLevel = DefaultRootSourceLevel;
		}

		/// <summary>
		/// Gets or sets the logging level for the catch-all root trace source ("NuPattern").
		/// </summary>
		public SourceLevels RootSourceLevel { get; set; }

		/// <summary>
		/// Gets the trace sources configurations.
		/// </summary>
		public IList<TraceSourceSetting> TraceSources { get; private set; }
	}
}
