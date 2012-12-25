using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Exposes Visual Studio shell events.
	/// </summary>
	public interface IShellEvents : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether the shell has been initialized.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Occurs when the shell has finished initializing.
		/// </summary>
		event EventHandler ShellInitialized;
	}
}
