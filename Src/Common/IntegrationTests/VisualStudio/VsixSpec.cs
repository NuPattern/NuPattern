using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.IntegrationTests.VisualStudio
{
    public class VsixSpec
    {
        private static readonly IAssertion Assert = new Assertion();
#if VSVER10
        private const string VsixContentTypeMefComponent = "MefComponent";
        private const string VsixContentTypeProjectTemplate = "ProjectTemplate";
#endif
#if VSVER11 || VSVER12
        private const string VsixContentTypeMefComponent = "Microsoft.VisualStudio.MefComponent";
        private const string VsixContentTypeProjectTemplate = "Microsoft.VisualStudio.ProjectTemplate";
#endif

        [TestClass]
        [DeploymentItem("Extensibility.IntegrationTests.Content", "Extensibility.IntegrationTests.Content")]
        public class GivenAVsixFile
        {
            [TestMethod, TestCategory("Integration")]
            public void WhenVsixFileIsEmpty_ThenThrowsArgumentOutOfRangeException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Vsix.Unzip(string.Empty, "Target"));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenVsixFileIsNull_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => Vsix.Unzip(null, "Target"));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenTargetDirIsEmpty_ThenArgumentOutOfRangeException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", string.Empty));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenTargetDirIsNull_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", null));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUnzipFromStream_ThenItIsCreated()
            {
                using (FileStream vsixFile = File.OpenRead("Extensibility.IntegrationTests.Content\\Toolkit1.vsix"))
                {
                    var vsixStreams = Vsix.Unzip(vsixFile);

                    Assert.NotNull(vsixStreams);
                    Assert.NotEqual(0, vsixStreams.Count);
                    Assert.Contains<string>("/extension.vsixmanifest", vsixStreams.Keys);
                }
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUnzipToNonExistingFolder_ThenItIsCreated()
            {
                var targetDir = new DirectoryInfo("Target");
                if (targetDir.Exists)
                {
                    targetDir.Delete(true);
                }

                Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", targetDir.FullName);

                Assert.True(Directory.Exists(targetDir.FullName));
            }

            [TestMethod, TestCategory("Integration")]
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

                Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", targetDir.FullName);

                Assert.False(File.Exists(existingFile));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUnzip_ThenContainsTopLevelFiles()
            {
                var targetDir = new DirectoryInfo("Target").FullName;

                Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", targetDir);

                Assert.True(File.Exists(Path.Combine(targetDir, "extension.vsixmanifest")));
                Assert.True(File.Exists(Path.Combine(targetDir, "Toolkit1.dll")));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUnzip_ThenContainsNestedFolders()
            {
                var targetDir = new DirectoryInfo("Target").FullName;

                Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", targetDir);

                Assert.True(Directory.Exists(Path.Combine(targetDir, "Documentation")));
                Assert.True(Directory.Exists(Path.Combine(targetDir, "Automation")));
                Assert.True(Directory.Exists(Path.Combine(targetDir, "Automation\\Templates\\Projects")));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenUnzip_ThenContainsNestedFiles()
            {
                var targetDir = new DirectoryInfo("Target").FullName;

                Vsix.Unzip("Extensibility.IntegrationTests.Content\\Toolkit1.vsix", targetDir);

                Assert.True(File.Exists(Path.Combine(targetDir, "Automation\\Templates\\Projects\\ToolkitCustomization.zip")));
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenReadingManifest_ThenGetsFullInfo()
            {
                var extension = Vsix.ReadManifest("Extensibility.IntegrationTests.Content\\Toolkit1.vsix");

                Assert.Equal("Toolkit1", extension.Header.Name);
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeMefComponent).Count());
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeProjectTemplate).Count());
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "NuPattern.Toolkit.PatternModel").Count());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenReadingVsixFromStream_ThenGetsFullInfo()
            {
                using (FileStream vsixFile = File.OpenRead("Extensibility.IntegrationTests.Content\\Toolkit1.vsix"))
                {
                    var extension = Vsix.ReadManifest(vsixFile);

                    Assert.Equal("Toolkit1", extension.Header.Name);
                    Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeMefComponent).Count());
                    Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeProjectTemplate).Count());
                    Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "NuPattern.Toolkit.PatternModel").Count());
                }
            }
        }

        [DeploymentItem("Extensibility.IntegrationTests.Content\\extension.vsixmanifest", "Extensibility.IntegrationTests.Content\\GivenAVsixManifestFile")]
        [TestClass]
        public class GivenAVsixManifestFile
        {
            [TestMethod, TestCategory("Integration")]
            public void WhenReadingManifest_ThenGetsFullInfo()
            {
                var extension = Vsix.ReadManifest("Extensibility.IntegrationTests.Content\\GivenAVsixManifestFile\\extension.vsixmanifest");

                Assert.Equal("Toolkit1", extension.Header.Name);
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeMefComponent).Count());
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == VsixContentTypeProjectTemplate).Count());
                Assert.Equal(1, extension.Content.Where(c => c.ContentTypeName == "NuPattern.Toolkit.PatternModel").Count());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenReadingManifestId_ThenGetsIdentifier()
            {
                var extension = Vsix.ReadManifestIdentifier("Extensibility.IntegrationTests.Content\\GivenAVsixManifestFile\\extension.vsixmanifest");

                Assert.Equal("ef4561f7-a3ea-4666-a080-bc2f195451e3", extension);
            }
        }
    }
}

