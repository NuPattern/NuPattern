
namespace NuPattern.Runtime
{
	/// <summary>
	/// Represents the settings for the runtime.
	/// </summary>
	public class RuntimeSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RuntimeSettings"/> class.
		/// </summary>
		public RuntimeSettings()
		{
			this.Tracing = new TracingSettings();
		}

		/// <summary>
		/// Gets the tracing settings.
		/// </summary>
		public TracingSettings Tracing { get; private set; }
	}
}
