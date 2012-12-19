using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.UriProviders
{
	/// <summary>
	/// Provides support for uris that reference elements in a solution.
	/// </summary>
	[PartCreationPolicy(CreationPolicy.Shared)]
	[Export(typeof(IFxrUriReferenceProvider))]
	[CLSCompliant(false)]
	public class SolutionUriProvider : IFxrUriReferenceProvider<IItemContainer>
	{
		/// <summary>
		/// The default scheme for this provider.
		/// </summary>
		public const string UriSchemeName = "solution";

		internal const char UriPathDelimiter = '/';
		internal const char SolutionPathDelimiter = '\\';

		/// <summary>
		/// Gets the URI scheme for this provider.
		/// </summary>
		/// <value>The URI scheme.</value>
		public string UriScheme
		{
			get
			{
				return UriSchemeName;
			}
		}

		[Import]
		internal ISolution Solution { get; set; }

		/// <summary>
		/// Resolves the given URI.
		/// </summary>
		public IItemContainer ResolveUri(Uri uri)
		{
			Guard.NotNull(() => uri, uri);

			if (!uri.Scheme.Equals(UriSchemeName))
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidSolutionUriProviderUri, uri.Scheme));
			}

			var solutionPath = GetSolutionPathFromUri(uri);
			if (!IsProjectHost(uri))
			{
				// Solution or solutionfolder or solutionitem (by full solution path)
				if (!String.IsNullOrEmpty(solutionPath))
				{
					// Solutionfolder or solutionitem (by full solution path)
					return ResolveSolutionRelativeUri(uri);
				}
				else
				{
					// Solution
					return this.Solution;
				}
			}
			else
			{
				var projectPath = GetProjectPathFromUri(uri);
				if (!String.IsNullOrEmpty(projectPath))
				{
					// Projectfolder or projectitem
					return ResolveProjectItemUri(uri);
				}
				else
				{
					// Project
					return ResolveProjectUri(uri);
				}
			}
		}

		/// <summary>
		/// Returns the solution path to the solution item from the given Uri
		/// </summary>
		/// <remarks>
		/// This method returns the path in case of original URI, in order to preserve the case of the orginal URI.
		/// </remarks>
		private static string GetSolutionPathFromUri(Uri uri)
		{
			// We include the authority in this case, which is the hardcoded "root" word, so that uris always work
			// regardless of whitespaces and potential invalid characters as part of the "domain".
			var hostPath = uri.GetLeftPart(UriPartial.Authority);
			var originalPath = uri.OriginalString.Substring(hostPath.Length);
			var solutionPath = originalPath.Trim(UriPathDelimiter);

			return solutionPath.Replace(UriPathDelimiter, SolutionPathDelimiter);
		}

		/// <summary>
		/// Returns the project path to the project item from the given Uri
		/// </summary>
		/// <remarks>
		/// This method returns the path in case of original URI, in order to preserve the case of the orginal URI.
		/// </remarks>
		private static string GetProjectPathFromUri(Uri uri)
		{
			var projectId = GetProjectIdFromUri(uri);
			if (string.IsNullOrEmpty(projectId))
			{
				return null;
			}

			var hostPath = (UriSchemeName + Uri.SchemeDelimiter + projectId);
			var originalPath = uri.OriginalString.Substring(hostPath.Length);
			var projectPath = originalPath.Trim(UriPathDelimiter);

			return projectPath.Replace(UriPathDelimiter, SolutionPathDelimiter);
		}

		/// <summary>
		/// Creates a URI for the given instance.
		/// </summary>
		public Uri CreateUri(IItemContainer instance)
		{
			Guard.NotNull(() => instance, instance);

			var solution = instance as ISolution;
			if (solution == null)
			{
				var solutionFolder = instance as ISolutionFolder;
				if (solutionFolder == null)
				{
					var project = instance as IProject;
					if (project == null)
					{
						var projectFolder = instance as IFolder;
						if (projectFolder == null)
						{
							var item = instance as IItem;
							if (item == null)
							{
								throw new NotImplementedException();
							}
							else
							{
								return CreateSolutionOrProjectItemUri(item);
							}
						}
						else
						{
							return CreateProjectFolderUri(projectFolder);
						}
					}
					else
					{
						return CreateProjectUri(project);
					}
				}
				else
				{
					return CreateSolutionFolderUri(solutionFolder);
				}
			}
			else
			{
				return CreateSolutionUri(solution);
			}
		}

		/// <summary>
		/// Opens the given instance.
		/// </summary>
		public void Open(IItemContainer instance)
		{
			Guard.NotNull(() => instance, instance);

			// Open: ProjectItems
			var item = instance as IItem;
			if (item != null)
			{
				item.Open();
			}
			else
			{
				// Select: Solutions, SolutionFolders, Projects, ProjectFolders
				instance.Select();
			}
		}

		/// <summary>
		/// Resolves the given URI to any solution item.
		/// </summary>
		/// <remarks>This method locates the solution item using a case-sensitive search from the solution.</remarks>
		private IItemContainer ResolveSolutionRelativeUri(Uri uri)
		{
			Guard.NotNull(() => uri, uri);

			var solutionPath = GetSolutionPathFromUri(uri);
			if (String.IsNullOrEmpty(solutionPath))
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidSolutionItemUri, uri));
			}

			var solutionItem = this.Solution.Find(solutionPath).FirstOrDefault();
			if (solutionItem == null)
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidSolutionItemUri, uri));
			}

			return solutionItem;
		}

		/// <summary>
		/// Resolves the given project URI.
		/// </summary>
		private IProject ResolveProjectUri(Uri uri)
		{
			Guard.NotNull(() => uri, uri);

			var projectId = GetProjectIdFromUri(uri);
			if (string.IsNullOrEmpty(projectId))
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectUri, uri));
			}

			// Find project in solution by GUID (case-insensitive)
			var project = GetProjectFromUri(uri);
			if (project == null)
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectUri, uri));
			}

			return project;
		}

		/// <summary>
		/// Resolves the given project item URI.
		/// </summary>
		private IItemContainer ResolveProjectItemUri(Uri uri)
		{
			Guard.NotNull(() => uri, uri);

			var projectPath = GetProjectPathFromUri(uri);
			if (String.IsNullOrEmpty(projectPath))
			{
				throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
			}

			var project = GetProjectFromUri(uri);
			if (project != null)
			{
				// Check if an folder/item id or folder/item path
				var itemId = GetProjectItemIdFromUri(uri);
				if (!String.IsNullOrEmpty(itemId))
				{
					// Find item in *project* by GUID (case-insensitive)
					var item = project.Traverse().OfType<IItem>()
						.Where(i => i.As<ProjectItem>().Properties != null)
						.FirstOrDefault(i => i.Data.ItemGuid != null && i.Data.ItemGuid.Equals(itemId, StringComparison.OrdinalIgnoreCase));
					if (item == null)
					{
						throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
					}

					return item;
				}
				else
				{
					// Find projectfolder or projectitem in project by name (case-sensitive)
					var folderOrItem = project.Find(projectPath).FirstOrDefault();
					if (folderOrItem == null)
					{
						throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
					}

					return folderOrItem;
				}
			}

			throw new UriFormatException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.SolutionUriProvider_InvalidProjectItemUri, uri));
		}

		/// <summary>
		/// Creates a URI for the given solution.
		/// </summary>
		/// <remarks>solution://root/</remarks>
		private Uri CreateSolutionUri(ISolution solution)
		{
			Guard.NotNull(() => solution, solution);

			var builder = new StringBuilder()
				.Append(this.UriScheme)
				.Append(Uri.SchemeDelimiter)
				.Append("root/");

			return new Uri(builder.ToString());
		}

		/// <summary>
		/// Creates a URI for the given solution folder.
		/// </summary>
		/// <remarks>solution://root/SolutionFolderPath</remarks>
		private Uri CreateSolutionFolderUri(ISolutionFolder solutionFolder)
		{
			Guard.NotNull(() => solutionFolder, solutionFolder);

			// Use full solution path format
			var builder = new StringBuilder()
				.Append(this.UriScheme)
				.Append(Uri.SchemeDelimiter)
				.Append("root/")
				.Append(MakeUriPath(solutionFolder.GetLogicalPath()));

			return new Uri(builder.ToString());
		}

		/// <summary>
		/// Creates a URI for the given project folder.
		/// </summary>
		/// <remarks>solution://PROJECTIDGUID/ProjectFolderPath</remarks>
		private Uri CreateProjectFolderUri(IFolder projectFolder)
		{
			Guard.NotNull(() => projectFolder, projectFolder);

			//Check if projectitem or solutionitem
			var project = projectFolder.GetContainingProject();
			if (project != null)
			{
				// Use full project path Format
				UriBuilder builder = new UriBuilder();
				builder.Scheme = this.UriScheme;
				builder.Host = project.Id;
				builder.Path = MakeUriPath(projectFolder.GetLogicalPath(project));
				return builder.Uri;
			}

			throw new InvalidOperationException();
		}

		/// <summary>
		/// Creates a URI for the given project.
		/// </summary>
		/// <remarks>solution://PROJECTGUID</remarks>
		private Uri CreateProjectUri(IProject project)
		{
			Guard.NotNull(() => project, project);

			UriBuilder builder = new UriBuilder();
			builder.Scheme = this.UriScheme;
			builder.Host = project.Id;
			return builder.Uri;
		}

		/// <summary>
		/// Creates a URI for the given project/solution item.
		/// </summary>
		/// <remarks>
		/// For Project Items: solution://PROJECTGUID/ITEMGUID or solution://PROJECTIDGUID/ProjectItemPath
		/// For Solution Items: solution://root/SolutionItemPath
		/// </remarks>
		private Uri CreateSolutionOrProjectItemUri(IItem solutionOrProjectItem)
		{
			Guard.NotNull(() => solutionOrProjectItem, solutionOrProjectItem);

			//Check if projectitem or solutionitem
			var project = solutionOrProjectItem.GetContainingProject();
			if (project != null)
			{
				// Check if projectitem has <ItemGuid>.
				var itemGuid = (string)solutionOrProjectItem.Data.ItemGuid;
				if (string.IsNullOrEmpty(itemGuid))
				{
					// Try creating unique id
					solutionOrProjectItem.AddItemIdentifier();
				}

				// Check if projectitem has <ItemGuid>.
				itemGuid = (string)solutionOrProjectItem.Data.ItemGuid;
				if (!string.IsNullOrEmpty(itemGuid))
				{
					// Use GUID Format
					UriBuilder builder = new UriBuilder();
					builder.Scheme = this.UriScheme;
					builder.Host = project.Id;
					builder.Path = TrimUriPath(itemGuid);
					return builder.Uri;
				}
				else
				{
					// Use full project path Format
					UriBuilder builder = new UriBuilder();
					builder.Scheme = this.UriScheme;
					builder.Host = project.Id;
					builder.Path = MakeUriPath(solutionOrProjectItem.GetLogicalPath(project));
					return builder.Uri;
				}
			}
			else
			{
				// Use full solution path format
				var builder = new StringBuilder()
					.Append(this.UriScheme)
					.Append(Uri.SchemeDelimiter)
					.Append("root/")
					.Append(MakeUriPath(solutionOrProjectItem.GetLogicalPath()));

				return new Uri(builder.ToString());
			}
		}

		/// <summary>
		/// Determines of the host of the Uri is a project or not
		/// </summary>
		private static bool IsProjectHost(Uri uri)
		{
			return GetProjectIdFromUri(uri) != null;
		}

		/// <summary>
		/// Returns the project identifier from the URI.
		/// </summary>
		private static string GetProjectIdFromUri(Uri uri)
		{
			Guid projectId = Guid.Empty;
			var result = Guid.TryParseExact(uri.Host, "D", out projectId);

			return result ? projectId.ToString("D") : null;
		}

		/// <summary>
		/// Returns the project from the solution.
		/// </summary>
		private IProject GetProjectFromUri(Uri uri)
		{
			var projectId = GetProjectIdFromUri(uri);
			if (string.IsNullOrEmpty(projectId))
			{
				return null;
			}

			// Find project in solution by GUID (case-insensitive)
			return this.Solution.Traverse().OfType<IProject>().FirstOrDefault(
				p => p.Id.Equals(projectId, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Determines if the Uri has a project item ID
		/// </summary>
		/// <returns></returns>
		private static bool IsProjectItemPath(Uri uri)
		{
			return GetProjectItemIdFromUri(uri) != null;
		}

		/// <summary>
		/// Returns the item identifier from the URI.
		/// </summary>
		private static string GetProjectItemIdFromUri(Uri uri)
		{
			var projectPath = GetProjectPathFromUri(uri);

			Guid itemId = Guid.Empty;
			var result = Guid.TryParseExact(projectPath, "D", out itemId);

			return result ? itemId.ToString("D") : null;
		}

		/// <summary>
		/// Converts a physical path to a uri path.
		/// </summary>
		private static string MakeUriPath(string pyhsicalPath)
		{
			return pyhsicalPath.Replace(SolutionPathDelimiter, UriPathDelimiter);
		}

		/// <summary>
		/// Returns a sanitized Uri Path.
		/// </summary>
		private static string TrimUriPath(string value)
		{
			// Remove GUID delimiters (if exist)
			//TODO: Remove other illegal chars
			return value.Trim(new char[] { '{', '}', '[', ']' });
		}
	}
}