using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime
{
	/// <summary>
	/// An extension repository that exposes the locally installed toolkit extensions.
	/// </summary>
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IToolkitRepository))]
	public class InstalledToolkitRepository : IToolkitRepository
	{
		private const string RepositoryName = "Installed";

		/// <summary>
		/// Initializes a new instance of the <see cref="InstalledToolkitRepository"/> class.
		/// </summary>
		[ImportingConstructor]
		public InstalledToolkitRepository(IEnumerable<IInstalledToolkitInfo> toolkits)
		{
			this.Toolkits = toolkits;
		}

		/// <summary>
		/// Gets the repository name.
		/// </summary>
		public string Name
		{
			get { return RepositoryName; }
		}

		/// <summary>
		/// Gets the list of toolkit extensions provided by the repository.
		/// </summary>
		public IEnumerable<IToolkitInfo> Toolkits { get; private set; }
	}
}