using System.Diagnostics;
using Microsoft.VisualStudio.Patterning.Runtime.Interfaces;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Represents the settings for a single trace source and 
	/// its logging level.
	/// </summary>
	public class TraceSourceSetting
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TraceSourceSetting"/> class.
		/// </summary>
		public TraceSourceSetting()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceSourceSetting"/> class.
		/// </summary>
		/// <param name="sourceName">Name of the source.</param>
		/// <param name="loggingLevel">The logging level.</param>
		public TraceSourceSetting(string sourceName, SourceLevels loggingLevel)
		{
			Guard.NotNullOrEmpty(() => sourceName, sourceName);

			this.SourceName = sourceName;
			this.LoggingLevel = loggingLevel;
		}

		/// <summary>
		/// Gets the name of the tracing source.
		/// </summary>
		public string SourceName { get; set; }

		/// <summary>
		/// Gets the logging level for this trace source.
		/// </summary>
		public SourceLevels LoggingLevel { get; set; }
	}
}