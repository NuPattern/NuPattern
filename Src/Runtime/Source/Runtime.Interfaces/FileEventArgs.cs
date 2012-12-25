using System;

namespace NuPattern.Runtime
{
	/// <summary>
	/// Argument for events that expose a file name.
	/// </summary>
	public class FileEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileEventArgs"/> class.
		/// </summary>
		public FileEventArgs(string fileName)
		{
			this.FileName = fileName;
		}

		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public string FileName { get; private set; }
	}
}
