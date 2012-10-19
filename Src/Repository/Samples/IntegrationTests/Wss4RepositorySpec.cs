using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Repository.Samples.IntegrationTests
{
	public sealed class Wss4RepositorySpec
	{
		private const string FactoryRespositoryUrl = @"http://lfenster-t61p/testsite";                       // private static string factoryRespositoryUrl = @"https://mscs-tfs.tier3network.com/factoryrepository";
		private const string FactoryAssetListTitle = @"test document library";                               // @"factory documents";
		private static readonly IAssertion Assert = new Assertion();
		private static readonly Guid factoryAssetListGuid = Guid.Parse(@"D1C473B2-F0BB-4ACE-B6D8-0726C00886AD");       // "365FE155-076F-4C48-9635-049176DCA5A6"
		private static ICredentials credentials = new NetworkCredential("testuser1", "Password123", "mcs");

		private Wss4RepositorySpec()
		{
		}

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod]
			public void WhenNameIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new Wss4Repository(null, new Uri(FactoryRespositoryUrl), Guid.NewGuid()));
			}

			[TestMethod]
            public void WhenNameIsEmpty_ThenThrowsArgumentOutOfRangeException()
			{
                Assert.Throws<ArgumentOutOfRangeException>(() => new Wss4Repository(String.Empty, new Uri(FactoryRespositoryUrl), Guid.NewGuid()));
			}

			[TestMethod]
			public void WhenSiteUrlIsInvalid_ThenThrowsArgumentNullException()
			{
				Assert.Throws<UriFormatException>(() => new Wss4Repository("Test", new Uri("invalidsite"), Guid.NewGuid(), credentials));
			}

			[TestMethod]
			public void WhenListTitleisNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new Wss4Repository("Test", new Uri(FactoryRespositoryUrl), null));
			}

			[TestMethod]
			public void WhenListIDisInvalid_ThenThrowsSharePointServerException()
			{
				Guid listId = Guid.NewGuid();
				Assert.Throws<Microsoft.SharePoint.Client.ServerException>(() => new Wss4Repository("Test", new Uri(FactoryRespositoryUrl), listId));
			}

			[TestMethod]
			public void WhenListTitleisInvalid_ThenThrowsSharePointServerException()
			{
				Assert.Throws<Microsoft.SharePoint.Client.ServerException>(() => new Wss4Repository("Test", new Uri(FactoryRespositoryUrl), "invalidList"));
			}

			[TestMethod]
			public void WhenListIdIsValid_ThenNoExceptionThrown()
			{
				Wss4Repository repository = new Wss4Repository("Test", new Uri(FactoryRespositoryUrl), factoryAssetListGuid);
				Assert.Equal("Test", repository.Name);
			}
		}

		[DeploymentItem("VsixRepository\\Factory1.vsix", "VsixRepository")]
		[DeploymentItem("VsixRepository\\Factory2.vsix", "VsixRepository")]
		[DeploymentItem("Factory3.vsix")]
		[TestClass]
		public class GivenAWssListVsixFiles
		{
			private IFactoryRepository repository;

			public GivenAWssListVsixFiles()
			{
				this.repository = new Wss4Repository("Test", new Uri(FactoryRespositoryUrl), FactoryAssetListTitle);
				Wss4Repository thisRepository = this.repository as Wss4Repository;
				if (thisRepository.DocumentExists("Factory3.vsix"))
				{
					thisRepository.DeleteDocument("Factory3.vsix");
				}
			}

			[TestMethod]
			public void WhenRepositoryIsConstructed_ThenItsNameMatchesProvidedName()
			{
				Assert.Equal("Test", this.repository.Name);
			}

			[TestMethod]
			public void WhenFactoriesRetrieved_ThenContainsFactoryInfo()
			{
				var factories = this.repository.Factories;

				Assert.Equal(2, factories.Count());
				Assert.True(factories.Any(info => info.Name == "Factory1"));
				Assert.True(factories.Any(info => info.Name == "Factory2"));
				Assert.True(factories.Any(info => info.DownloadUri.OriginalString == String.Format(CultureInfo.CurrentCulture, @"{0}/{1}/{2}", FactoryRespositoryUrl, FactoryAssetListTitle, "Factory1.vsix")));
				Assert.True(factories.Any(info => info.DownloadUri.OriginalString == String.Format(CultureInfo.CurrentCulture, @"{0}/{1}/{2}", FactoryRespositoryUrl, FactoryAssetListTitle, "Factory2.vsix")));
			}

			[TestMethod]
			public void WhenNewVsixIsAddedToFolder_ThenNewInfoIsAvailableInmediately()
			{
				Assert.Equal(2, this.repository.Factories.Count());
				using (var vsixFile = File.Open("Factory3.vsix", FileMode.Open))
				{
					((Wss4Repository)this.repository).UploadFactory(vsixFile, "Factory3.vsix");
				}

				Assert.Equal(3, this.repository.Factories.Count());
			}
		}
	}
}
