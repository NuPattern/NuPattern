using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Repository.Samples.IntegrationTests
{
	public class FileSystemRepositorySpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod]
			public void WhenNameIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new FileSystemRepository(null, "VsixRepository"));
			}

			[TestMethod]
            public void WhenNameIsEmpty_ThenThrowsArgumentOutOfRangeException()
			{
                Assert.Throws<ArgumentOutOfRangeException>(() => new FileSystemRepository(String.Empty, "VsixRepository"));
			}

			[TestMethod]
			public void WhenDirectoryIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new FileSystemRepository("Test", null));
			}

			[TestMethod]
			public void WhenDirectoryIsEmpty_ThenThrowsArgumentException()
			{
				Assert.Throws<ArgumentException>(() => new FileSystemRepository("Test", String.Empty));
			}

			[TestMethod]
			public void WhenDirectoryDoesNotExist_ThenThrowsIOException()
			{
				Assert.Throws<IOException>(() => new FileSystemRepository("Test", "Foo"));
			}
		}

		[DeploymentItem("VsixRepository\\Factory1.vsix", "VsixRepository")]
		[DeploymentItem("VsixRepository\\Factory2.vsix", "VsixRepository")]
		[DeploymentItem("Factory3.vsix")]
		[TestClass]
		public class GivenADirectoryVsixFiles
		{
			private IFactoryRepository repository;

			public GivenADirectoryVsixFiles()
			{
				if (File.Exists("VsixRepository\\Factory3.vsix"))
				{
					File.Delete("VsixRepository\\Factory3.vsix");
				}

				this.repository = new FileSystemRepository("Test", "VsixRepository");
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
				Assert.True(factories.Any(info => info.DownloadUri.OriginalString == "file://VsixRepository\\Factory1.vsix"));
				Assert.True(factories.Any(info => info.DownloadUri.OriginalString == "file://VsixRepository\\Factory2.vsix"));
			}

			[TestMethod]
			public void WhenNewVsixIsAddedToFolder_ThenNewInfoIsAvailableInmediately()
			{
				Assert.Equal(2, this.repository.Factories.Count());

				File.Copy("Factory3.vsix", "VsixRepository\\Factory3.vsix");

				Assert.Equal(3, this.repository.Factories.Count());
			}
		}
	}
}
