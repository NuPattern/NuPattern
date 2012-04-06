using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.Patterning.Repository.Properties;
using Microsoft.VisualStudio.Patterning.Runtime;
using ClientOM = Microsoft.SharePoint.Client;

namespace Microsoft.VisualStudio.Patterning.Repository
{
	/// <summary>
	/// Defines a VSIX repository for SharePoint.
	/// </summary>
	[CLSCompliant(false)]
	public class Wss4Repository : IFactoryRepository
	{
		private const long BUFFERSIZE = 32768;
		private Uri siteUri;
		private string listTitle;
		private Guid listId;
		private ICredentials credentials;

		/// <summary>
		/// Initializes a new instance of the <see cref="Wss4Repository"/> class.
		/// </summary>
		/// <param name="repositoryName">Name of the repository.</param>
		/// <param name="siteUri">The site URI.</param>
		/// <param name="listId">The list id.</param>
		public Wss4Repository(string repositoryName, Uri siteUri, Guid listId)
			: this(repositoryName, siteUri, listId, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Wss4Repository"/> class.
		/// </summary>
		/// <param name="repositoryName">Name of the repository.</param>
		/// <param name="siteUri">The site URI.</param>
		/// <param name="listTitle">The list title.</param>
		public Wss4Repository(string repositoryName, Uri siteUri, string listTitle)
			: this(repositoryName, siteUri, listTitle, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Wss4Repository"/> class.
		/// </summary>
		/// <param name="repositoryName">Name of the repository.</param>
		/// <param name="siteUri">The site URI.</param>
		/// <param name="listId">The list id.</param>
		/// <param name="credentials">The credentials.</param>
		public Wss4Repository(string repositoryName, Uri siteUri, Guid listId, ICredentials credentials)
		{
			Guard.NotNullOrEmpty(() => repositoryName, repositoryName);
			Guard.NotNull(() => siteUri, siteUri);
			Guard.NotNullOrEmpty(() => siteUri.OriginalString, siteUri.OriginalString);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(siteUri))
			{
				ClientOM.List list = RetrieveWSSList(listId, credentials, ctx);
				if (null != list)
				{
					this.siteUri = siteUri;
					this.listId = listId;
					this.listTitle = list.Title;
				}
			}

			this.credentials = credentials;
			this.Name = repositoryName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Wss4Repository"/> class.
		/// </summary>
		/// <param name="repositoryName">Name of the repository.</param>
		/// <param name="siteUri">The site URI.</param>
		/// <param name="listTitle">The list title.</param>
		/// <param name="credentials">The credentials.</param>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Not Applicable")]
		public Wss4Repository(string repositoryName, Uri siteUri, string listTitle, ICredentials credentials)
		{
			Guard.NotNullOrEmpty(() => repositoryName, repositoryName);
			Guard.NotNullOrEmpty(() => "siteUri", siteUri.OriginalString);
			Guard.NotNullOrEmpty(() => listTitle, listTitle);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(siteUri))
			{
				ClientOM.List list = RetrieveWSSList(listTitle, credentials, ctx);
				if (null != list)
				{
					this.siteUri = siteUri;
					this.listTitle = listTitle;
					this.listId = list.Id;
				}
			}

			this.credentials = credentials;
			this.Name = repositoryName;
		}

		#region IFactoryRepository Members

		/// <summary>
		/// Gets the repository name.
		/// </summary>
		/// <value></value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the list of extensions provided by the repository.
		/// </summary>
		public IEnumerable<IFactoryInfo> Factories
		{
			get
			{
				using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(this.siteUri))
				{
					ClientOM.List list = RetrieveWSSList(this.listId, this.credentials, ctx);
					if (null != list)
					{
						ClientOM.CamlQuery camlQuery = new ClientOM.CamlQuery();
						camlQuery.ViewXml = @"<View><Query><Where><Eq><FieldRef Name='File_x0020_Type'/><Value Type='Text'>vsix</Value></Eq></Where></Query></View>";

						ClientOM.ListItemCollection listItems = list.GetItems(camlQuery);
						ctx.Load(listItems);
						ctx.ExecuteQuery();

						foreach (var listItem in listItems)
						{
							FactoryInfo factoryInfo = this.ExtractFactoryFromWSSListItem(ctx, listItem);
							yield return factoryInfo;
						}
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// Uploads the factory.
		/// </summary>
		/// <param name="vsixFile">The vsix file.</param>
		/// <param name="uploadFileName">Name of the upload file.</param>
		public void UploadFactory(FileStream vsixFile, string uploadFileName)
		{
			Guard.NotNull(() => vsixFile, vsixFile);
			Guard.NotNullOrEmpty(() => uploadFileName, uploadFileName);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(this.siteUri))
			{
				ClientOM.List list = RetrieveWSSList(this.listId, this.credentials, ctx);
				if (null != list)
				{
					string parentUrl = list.ParentWebUrl;
					string rootFolder = list.RootFolder.Name;
					string fileUrl = String.Format(CultureInfo.CurrentCulture, @"{0}/{1}/{2}", parentUrl, rootFolder, uploadFileName);
					ClientOM.File.SaveBinaryDirect(ctx, fileUrl, vsixFile, true);
				}
			}
		}

		/// <summary>
		/// Documents the exists.
		/// </summary>
		/// <param name="documentName">Name of the document.</param>
		/// <returns>A value indicating whether or not a document exists.</returns>
		public bool DocumentExists(string documentName)
		{
			Guard.NotNullOrEmpty(() => documentName, documentName);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(this.siteUri))
			{
				ClientOM.List list = RetrieveWSSList(this.listId, this.credentials, ctx);
				if (null != list)
				{
					ClientOM.CamlQuery camlQuery = new ClientOM.CamlQuery();
					camlQuery.ViewXml = String.Format(CultureInfo.CurrentCulture, @"<View><Query><Where><Eq> <FieldRef Name='FileLeafRef'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", documentName);

					ClientOM.ListItemCollection listItems = list.GetItems(camlQuery);
					ctx.Load(listItems);
					ctx.ExecuteQuery();
					return listItems.Count != 0;
				}

				return false;
			}
		}

		/// <summary>
		/// Gets the document.
		/// </summary>
		/// <param name="documentName">Name of the document.</param>
		/// <returns>The file information.</returns>
		public ClientOM.FileInformation GetDocument(string documentName)
		{
			Guard.NotNullOrEmpty(() => documentName, documentName);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(this.siteUri))
			{
				ClientOM.List list = RetrieveWSSList(this.listId, this.credentials, ctx);
				if (null != list)
				{
					ClientOM.CamlQuery camlQuery = new ClientOM.CamlQuery();
					camlQuery.ViewXml = String.Format(CultureInfo.CurrentCulture, @"<View><Query><Where><Eq> <FieldRef Name='FileLeafRef'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", documentName);

					ClientOM.ListItemCollection listItems = list.GetItems(camlQuery);
					ctx.Load(listItems);
					ctx.ExecuteQuery();
					if (listItems.Count == 1)
					{
						string fileName = (string)listItems[0]["FileRef"];
						ClientOM.FileInformation fileInformation = ClientOM.File.OpenBinaryDirect(ctx, fileName);
						return fileInformation;
					}
				}

				return null;
			}
		}

		/// <summary>
		/// Deletes the document.
		/// </summary>
		/// <param name="documentName">Name of the document.</param>
		/// <returns>This is <c>true</c> if the document exists; otherwise <c>false</c>.</returns>
		public bool DeleteDocument(string documentName)
		{
			Guard.NotNullOrEmpty(() => documentName, documentName);

			using (ClientOM.ClientContext ctx = new ClientOM.ClientContext(this.siteUri))
			{
				ClientOM.List list = RetrieveWSSList(this.listId, this.credentials, ctx);
				if (null != list)
				{
					ClientOM.CamlQuery camlQuery = new ClientOM.CamlQuery();
					camlQuery.ViewXml = String.Format(CultureInfo.CurrentCulture, @"<View><Query><Where><Eq> <FieldRef Name='FileLeafRef'/><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", documentName);

					ClientOM.ListItemCollection listItems = list.GetItems(camlQuery);
					ctx.Load(listItems);
					ctx.ExecuteQuery();
					if (listItems.Count == 1)
					{
						listItems[0].DeleteObject();
						ctx.ExecuteQuery();
						return true;
					}

					return false;
				}

				return false;
			}
		}

		private static ClientOM.Web GetSiteLists(ICredentials credentials, ClientOM.ClientContext ctx)
		{
			if (null != credentials)
			{
				ctx.Credentials = credentials;
			}

			ClientOM.Web site = ctx.Web;
			ctx.Load(site);
			ctx.Load(site.Lists);

			return site;
		}

		private static ClientOM.List RetrieveWSSList(Guid listId, ICredentials credentials, ClientOM.ClientContext ctx)
		{
			ClientOM.Web site = GetSiteLists(credentials, ctx);
			ClientOM.List list = site.Lists.GetById(listId);
			ctx.Load(list, l => l.Id, l => l.Title, l => l.RootFolder, l => l.ParentWebUrl);
			ctx.ExecuteQuery();

			if (null == list)
			{
				throw new ArgumentException(String.Format(
				CultureInfo.CurrentCulture,
				Resources.WSSRepository_ListIdDoesNotExist,
				listId));
			}
			else
			{
				return list;
			}
		}

		private static ClientOM.List RetrieveWSSList(string listTitle, ICredentials credentials, ClientOM.ClientContext ctx)
		{
			ClientOM.Web site = GetSiteLists(credentials, ctx);
			ClientOM.List list = site.Lists.GetByTitle(listTitle);
			ctx.Load(list, l => l.Id, l => l.Title, l => l.RootFolder, l => l.ParentWebUrl);
			ctx.ExecuteQuery();

			if (null == list)
			{
				throw new ArgumentException(String.Format(
				CultureInfo.CurrentCulture,
				Resources.WSSRepository_ListTitleDoesNotExist,
				listTitle));
			}
			else
			{
				return list;
			}
		}

		private static void CopyStream(System.IO.Stream inputStream, System.IO.Stream outputStream)
		{
			byte[] buffer = new byte[BUFFERSIZE];
			int bytesRead = 0;
			do
			{
				bytesRead = inputStream.Read(buffer, 0, buffer.Length);
				outputStream.Write(buffer, 0, bytesRead);
			}
			while (bytesRead != 0);
		}

		private FactoryInfo ExtractFactoryFromWSSListItem(ClientOM.ClientContext context, ClientOM.ListItem listItem)
		{
			Guard.NotNull(() => context, context);
			Guard.NotNull(() => listItem, listItem);

			string fileName = (string)listItem["FileRef"];
			string shortFfileName = (string)listItem["FileLeafRef"];
			using (ClientOM.FileInformation fileInformation = ClientOM.File.OpenBinaryDirect(context, fileName))
			{
				if (null != fileInformation)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						CopyStream(fileInformation.Stream, memoryStream);
						var extension = Vsix.ReadManifest(memoryStream);
						return new FactoryInfo
						{
							Id = extension.Header.Identifier,
							Description = extension.Header.Description,
							Name = extension.Header.Name,
							DownloadUri = new Uri(String.Format(CultureInfo.CurrentCulture, @"{0}/{1}/{2}", this.siteUri, this.listTitle, shortFfileName)),
						};
					}
				}
				else
				{
					throw new ArgumentException(String.Format(
					CultureInfo.CurrentCulture,
					Resources.WSSRepository_FileInformationUnavailable,
					listItem.DisplayName));
				}
			}
		}
	}
}