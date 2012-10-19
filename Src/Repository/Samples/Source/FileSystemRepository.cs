using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.Patterning.Repository.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Repository
{
	/// <summary>
	/// Defines a VSIX repository for the FileSystem.
	/// </summary>
	public class FileSystemRepository : IFactoryRepository
	{
		private string rootDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileSystemRepository"/> class.
		/// </summary>
		/// <param name="repositoryName">Name of the repository.</param>
		/// <param name="rootDirectory">The root directory.</param>
		public FileSystemRepository(string repositoryName, string rootDirectory)
		{
			Guard.NotNullOrEmpty(() => repositoryName, repositoryName);
			Guard.NotNullOrEmpty(() => rootDirectory, rootDirectory);

			if (!Directory.Exists(rootDirectory))
			{
				throw new IOException(String.Format(
					CultureInfo.CurrentCulture,
					Resources.FileSystemRepository_DirectoryDoesNotExist,
					rootDirectory));
			}

			this.rootDirectory = rootDirectory;
			this.Name = repositoryName;
		}

		/// <summary>
		/// Gets the repository name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the list of extensions provided by the repository.
		/// </summary>
		//// TODO: we'll need to refactor to a search-oriented API here.
		//// network calls will kill us otherwise for other providers.
		public IEnumerable<IFactoryInfo> Factories
		{
			get { return this.BuildInfo(); }
		}

		private IEnumerable<IFactoryInfo> BuildInfo()
		{
			foreach (var vsixFile in Directory.EnumerateFiles(this.rootDirectory, "*.vsix"))
			{
				var vsixDir = Path.Combine(this.rootDirectory, Path.GetFileNameWithoutExtension(vsixFile));
				if (!Directory.Exists(vsixDir))
				{
					Vsix.Unzip(vsixFile, vsixDir);
				}

				if (!File.Exists(Path.Combine(vsixDir, "extension.vsixmanifest")))
				{
					continue;
				}

				var extension = Vsix.ReadManifest(Path.Combine(vsixDir, "extension.vsixmanifest"));
				//// We lazy-read here.
				yield return new FactoryInfo
				{
					Id = extension.Header.Identifier,
					Description = extension.Header.Description,
					Name = extension.Header.Name,
					DownloadUri = new Uri("file://" + vsixFile),
				};
			}
		}
	}
}