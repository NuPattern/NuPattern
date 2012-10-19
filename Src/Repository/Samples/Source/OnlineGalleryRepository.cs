using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.VisualStudio.Patterning.Repository.OnlineGallery;
using Microsoft.VisualStudio.Patterning.Runtime;

namespace Microsoft.VisualStudio.Patterning.Repository
{
	/// <summary>
	/// Defines a VSIX repository for the on-line gallery.
	/// </summary>
	public class OnlineGalleryRepository : IFactoryRepository
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="OnlineGalleryRepository"/> class.
		/// </summary>
		public OnlineGalleryRepository()
		{
			this.Name = "Online Gallery Factories";
		}

		/// <summary>
		/// Gets the repository name.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the list of extensions provided by the repository.
		/// </summary>
		public IEnumerable<IFactoryInfo> Factories
		{
			get { return BuildInfo(); }
		}

		private static IEnumerable<IFactoryInfo> BuildInfo()
		{
			var gallery = new VsGalleryRepository(
				() => new ServiceClient<ICategoryService>(
					new WSHttpBinding(SecurityMode.None),
					new EndpointAddress("http://visualstudiogallery.msdn.microsoft.com/services/Category2.svc")),
				() => new ServiceClient<IReleaseService>(
					new WSHttpBinding(SecurityMode.None),
					new EndpointAddress("http://visualstudiogallery.msdn.microsoft.com/services/Release3.svc")));

			var result = gallery.Search(string.Empty, "Project.Metadata['ContentTypes'] LIKE '%ComponentModel%'", "Project.Name", null, null);
			return result.Releases.Select(release => new FactoryInfo
			{
				Id = release.Project.Metadata["VsixId"],
				Description = release.Project.Description,
				Name = release.Project.Title,
				DownloadUri = new Uri(release.Project.Metadata["DownloadUrl"]),
			});
		}
	}
}