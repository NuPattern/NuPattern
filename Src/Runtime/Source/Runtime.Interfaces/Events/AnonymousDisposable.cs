using System;

namespace Microsoft.VisualStudio.Patterning.Runtime
{
	/// <summary>
	/// Provides an implementation of <see cref="IDisposable"/> that 
	/// invokes the action received in the constructor upon disposal.
	/// </summary>
	/// <devdoc>Taken from Rx extensions.</devdoc>
	public sealed class AnonymousDisposable : IDisposable
	{
		private readonly Action dispose;
		private bool isDisposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnonymousDisposable"/> class.
		/// </summary>
		public AnonymousDisposable(Action dispose)
		{
			Guard.NotNull(() => dispose, dispose);

			this.dispose = dispose;
		}

		/// <summary>
		/// Supports the Rx infrastructure.
		/// </summary>
		public void Dispose()
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.dispose();
			}
		}
	}
}
