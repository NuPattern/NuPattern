using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	public class VsixSpec
	{
		private static readonly IAssertion Assert = new Assertion();

		[TestClass]
		[DeploymentItem("Authoring.UnitTests.Content", "Authoring.UnitTests.Content")]
		public class GivenAVsixFile
		{
			[TestMethod, TestCategory("Unit")]
			public void WhenVsixFileIsEmpty_ThenThrowsArgumentOutOfRangeException()
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => Vsix.Unzip(string.Empty, "Target"));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenVsixFileIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => Vsix.Unzip(null, "Target"));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenTargetDirIsEmpty_ThenArgumentOutOfRangeException()
			{
				Assert.Throws<ArgumentOutOfRangeException>(() => Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", string.Empty));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenTargetDirIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", null));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzipFromStream_ThenItIsCreated()
			{
				using (FileStream vsixFile = File.OpenRead("Authoring.UnitTests.Content\\Toolkit1.vsix"))
				{
					var vsixStreams = Vsix.Unzip(vsixFile);

					Assert.NotNull(vsixStreams);
					Assert.NotEqual(0, vsixStreams.Count);
					Assert.Contains<string>("/extension.vsixmanifest", vsixStreams.Keys);
				}
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzipToNonExistingFolder_ThenItIsCreated()
			{
				var targetDir = new DirectoryInfo("Target");
				if (targetDir.Exists)
				{
					targetDir.Delete(true);
				}

				Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", targetDir.FullName);

				Assert.True(Directory.Exists(targetDir.FullName));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzipToExistingFolder_ThenTargetFolderIsCleaned()
			{
				var targetDir = new DirectoryInfo("Target");
				var existingFile = Path.Combine(targetDir.FullName, "Foo.txt");

				if (targetDir.Exists)
				{
					targetDir.Delete(true);
				}

				targetDir.Create();
				File.WriteAllText(existingFile, "Foo");

				Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", targetDir.FullName);

				Assert.False(File.Exists(existingFile));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzip_ThenContainsTopLevelFiles()
			{
				var targetDir = new DirectoryInfo("Target").FullName;

				Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", targetDir);

				Assert.True(File.Exists(Path.Combine(targetDir, "extension.vsixmanifest")));
				Assert.True(File.Exists(Path.Combine(targetDir, "Toolkit1.dll")));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzip_ThenContainsNestedFolders()
			{
				var targetDir = new DirectoryInfo("Target").FullName;

				Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", targetDir);

				Assert.True(Directory.Exists(Path.Combine(targetDir, "Documentation")));
				Assert.True(Directory.Exists(Path.Combine(targetDir, "Automation")));
				Assert.True(Directory.Exists(Path.Combine(targetDir, "Automation\\Templates\\Projects")));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenUnzip_ThenContainsNestedFiles()
			{
				var targetDir = new DirectoryInfo("Target").FullName;

				Vsix.Unzip("Authoring.UnitTests.Content\\Toolkit1.vsix", targetDir);

				Assert.True(File.Exists(Path.Combine(targetDir, "Documentation\\ToolkitDocumentation.docx")));
				Assert.True(File.Exists(Path.Combine(targetDir, "Documentation\\PatternDocumentation.docx")));
				Assert.True(File.Exists(Path.Combine(targetDir, "Documentation\\PatternRequirements.xlsx")));
				Assert.True(File.Exists(Path.Combine(targetDir, "Automation\\Templates\\Projects\\ToolkitCustomization.zip")));
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenReadingManifest_ThenGetsFullInfo()
			{
				var extension = Vsix.ReadManifest("Authoring.UnitTests.Content\\Toolkit1.vsix");

				Assert.Equal("Toolkit1", extension.Header.Name);
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "MefComponent").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ProjectTemplate").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ToolkitDocumentationRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternDocumentationRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternRequirementsRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternModel").Count());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenReadingVsixFromStream_ThenGetsFullInfo()
			{
				using (FileStream vsixFile = File.OpenRead("Authoring.UnitTests.Content\\Toolkit1.vsix"))
				{
					var extension = Vsix.ReadManifest(vsixFile);

					Assert.Equal("Toolkit1", extension.Header.Name);
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "MefComponent").Count());
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ProjectTemplate").Count());
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ToolkitDocumentationRef").Count());
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternDocumentationRef").Count());
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternRequirementsRef").Count());
					Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternModel").Count());
				}
			}
		}

		[DeploymentItem("Authoring.UnitTests.Content\\extension.vsixmanifest", "Authoring.UnitTests.Content\\GivenAVsixManifestFile")]
		[TestClass]
		public class GivenAVsixManifestFile
		{
			[TestMethod, TestCategory("Unit")]
			public void WhenReadingManifest_ThenGetsFullInfo()
			{
				var extension = Vsix.ReadManifest("Authoring.UnitTests.Content\\GivenAVsixManifestFile\\extension.vsixmanifest");

				Assert.Equal("Toolkit1", extension.Header.Name);
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "MefComponent").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ProjectTemplate").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "ToolkitDocumentationRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternDocumentationRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternRequirementsRef").Count());
				Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "PatternModel").Count());
			}

			[TestMethod, TestCategory("Unit")]
			public void WhenReadingManifestId_ThenGetsIdentifier()
			{
				var extension = Vsix.ReadManifestIdentifier("Authoring.UnitTests.Content\\GivenAVsixManifestFile\\extension.vsixmanifest");

				Assert.Equal("ef4561f7-a3ea-4666-a080-bc2f195451e3", extension);
			}
		}
	}
}
