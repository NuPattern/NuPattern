using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
	/// <summary>
	/// Allows working with external APIs that create temporary 
	/// files and cleaning them up on Dispose.
	/// </summary>
	/// <remarks>
	/// Usage: 
	/// <code>
	/// using (new TempFileCleaner())
	/// {
	///   // do work that creates temp files
	/// }
	/// </code>
	/// </remarks>
	public class TempFileCleaner : IDisposable
	{
		private FileSystemWatcher watcher;
		private List<string> tempFiles = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TempFileCleaner"/> class and 
		/// starts watching for files matching *.tmp under the temp path.
		/// </summary>
		public TempFileCleaner()
		{
			this.watcher = new FileSystemWatcher(Path.GetTempPath(), "*.tmp");
			watcher.EnableRaisingEvents = true;
			watcher.Created += (sender, args) => this.tempFiles.Add(args.FullPath);
		}

		/// <summary>
		/// Gets the generated temp files since this class was constructed.
		/// </summary>
		public IEnumerable<string> TempFiles { get { return this.tempFiles.Distinct(); } }

		/// <summary>
		/// Cleans up all generated <see cref="TempFiles"/>.
		/// </summary>
		public void Dispose()
		{
			watcher.Dispose();
			foreach (var tempFile in this.TempFiles)
			{
				File.Delete(tempFile);
			}
		}
	}
}
