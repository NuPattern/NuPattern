using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.ExtensionManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.Runtime.UnitTests
{
    public class InstalledToolkitInfoSpec
    {
        [TestClass]
        public class GivenNoContext
        {
            internal static readonly IAssertion Assert = new Assertion();

            private IInstalledExtension extension;
            private ISchemaReader reader;
            private ISchemaResource resource;

            [TestInitialize]
            public void InitializeContext()
            {
                this.extension = null;
                this.reader = new Mock<ISchemaReader>().Object;
                this.resource = new Mock<ISchemaResource>().Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializingWithANullExtension_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new InstalledToolkitInfo(this.extension, this.reader, this.resource));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenExtensionIsSet()
            {
                var extension = new Mock<IInstalledExtension>().Object;

                var target = new InstalledToolkitInfo(extension, this.reader, this.resource);

                Assert.Same(extension, target.Extension);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenIdIsExposed()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.Header.Identifier).Returns("Foo");

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Equal("Foo", target.Id);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenNameIsExposed()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.Header.Name).Returns("Foo");

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Equal("Foo", target.Name);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenAuthorIsExposed()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.Header.Author).Returns("Foo");

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Equal("Foo", target.Author);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenVersionIsExposed()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.Header.Version).Returns(new Version(0, 0, 0, 1));

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Equal(new Version(0, 0, 0, 1), target.Version);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenIconPathIsExposed()
            {
                var installPath = Path.GetTempPath();
                var icon = Path.GetRandomFileName();
                var iconPath = Path.Combine(installPath, icon);

                using (File.CreateText(iconPath))
                {
                }

                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.InstallPath).Returns(installPath);
                extension.Setup(ext => ext.Header.Icon).Returns(icon);

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                File.Delete(iconPath);

                Assert.Equal(iconPath, target.ToolkitIconPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializingAndIconIsNull_ThenIconPathIsNull()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.InstallPath).Returns(@"Z:\InstallDir");
                extension.Setup(ext => ext.Header.Icon).Returns((string)null);

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Null(target.PatternIconPath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_TheTemplatesEmpty()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.InstallPath).Returns(@"Z:\InstallDir");

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.NotNull(target.Templates);
                Assert.Equal(0, target.Templates.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenDefaultClassification()
            {
                var extension = new Mock<IInstalledExtension>();
                extension.Setup(ext => ext.Content).Returns(Enumerable.Empty<IExtensionContent>());

                var target = new InstalledToolkitInfo(extension.Object, this.reader, this.resource);

                Assert.Equal(string.Empty, target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }
        }

        [TestClass]
        public class GivenAnInstalledToolkitWithContents
        {
            internal static readonly IAssertion Assert = new Assertion();

            private IInstalledExtension extension;
            private InstalledToolkitInfo target;
            private ISchemaReader reader;
            private ISchemaResource resource;

            [TestInitialize]
            public void Initialize()
            {
                var mock = new Mock<IInstalledExtension>();
                mock.Setup(ext => ext.InstallPath).Returns(@"X:\");
                mock.Setup(ext => ext.Content).Returns(
                    new[]
                    { 
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == InstalledToolkitInfo.PatternModelCustomExtensionName && c.RelativePath == @"Foo.patterndefinition" && c.Attributes == new Dictionary<string, string> { { SchemaResource.AssemblyFileProperty, "Test.dll" } }),
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == "Other" && c.RelativePath == @"Documentation\Other.docx" && c.Attributes == new Dictionary<string, string> { { "IsCustomizable", bool.TrueString } }),
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == "Other" && c.RelativePath == @"Sample.file" && c.Attributes == new Dictionary<string, string> { { "IsCustomizable", bool.TrueString } }),
                    });

                this.extension = mock.Object;
                this.reader = new Mock<ISchemaReader>().Object;
                this.resource = new Mock<ISchemaResource>().Object;
                this.target = new InstalledToolkitInfo(this.extension, this.reader, this.resource);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingOtherContent_ThenProductContentIsRetrieved()
            {
                var content = this.target.GetCustomExtensions(InstalledToolkitInfo.PatternModelCustomExtensionName);

                Assert.Equal(1, content.Count());
                Assert.Equal("Foo.patterndefinition", content.First());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingOtherContentWithNullContentType_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => this.target.GetCustomExtensions(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingOtherContentWithEmptyContentType_ThenThrowsArgumentOutOfRangeException()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => this.target.GetCustomExtensions(string.Empty));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingCustomizableExtensions_ThenGotItemsMarkedAsIsCustomizableOrInDocumentsFolder()
            {
                var contents = this.target.GetCustomizableExtensions();

                Assert.Equal(2, contents.Count());
                Assert.True(contents.Any(c => c.RelativePath.Equals(@"Documentation\Other.docx", StringComparison.OrdinalIgnoreCase)));
                Assert.True(contents.Any(c => c.RelativePath.Equals(@"Sample.file", StringComparison.OrdinalIgnoreCase)));
            }

            [TestMethod, TestCategory("Unit")]
            [Ignore]
            public void WhenGettingToolkitSchema_ThenInitializesSchemaInstanceAndPatternToolkitId()
            {
                // TODO: isolate the schema reading and extracting
                Assert.NotNull(this.target.Schema);
                Assert.Equal(this.target.Extension.Header.Identifier, this.target.Schema.Pattern.ExtensionId);
            }
        }

        [TestClass]
        public class GivenAnInstalledToolkitWithClassification
        {
            internal static readonly IAssertion Assert = new Assertion();

            private IInstalledExtension extension;
            private InstalledToolkitInfo target;
            private ISchemaReader reader;
            private ISchemaResource resource;
            private Mock<IInstalledExtension> mockExtension;

            [TestInitialize]
            public void Initialize()
            {
                this.mockExtension = new Mock<IInstalledExtension>();
                this.extension = mockExtension.Object;
                this.reader = new Mock<ISchemaReader>().Object;
                this.resource = new Mock<ISchemaResource>().Object;
                this.target = new InstalledToolkitInfo(this.extension, this.reader, this.resource);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoClassifaction_ThenDefaultClassification()
            {
                Assert.Equal(String.Empty, target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCategory_ThenDefaultCustomizeVisibility()
            {
                this.mockExtension.Setup(ext => ext.Content).Returns(
                    new[]
                    { 
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == InstalledToolkitInfo.ToolkitClassificationCustomExtensionName && c.Attributes == new Dictionary<string, string> 
                        { 
                            { 
                                InstalledToolkitInfo.CategoryAttributeName, "Foo" 
                            }, 
                        }),
                    });

                Assert.Equal("Foo", target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenVisibility_ThenDefaultCategory()
            {
                this.mockExtension.Setup(ext => ext.Content).Returns(
                    new[]
                    { 
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == InstalledToolkitInfo.ToolkitClassificationCustomExtensionName && c.Attributes == new Dictionary<string, string> 
                        { 
                            {
                                InstalledToolkitInfo.CustomizeVisibilityAttributeName, ExtensionVisibility.Hidden.ToString()
                            } 
                        }),
                    });

                Assert.Equal(String.Empty, target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Hidden, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCLassification_ThenCategoryAndCustomizeVisibility()
            {
                this.mockExtension.Setup(ext => ext.Content).Returns(
                    new[]
                    { 
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == InstalledToolkitInfo.ToolkitClassificationCustomExtensionName && c.Attributes == new Dictionary<string, string> 
                        { 
                            { 
                                InstalledToolkitInfo.CategoryAttributeName, "Foo" 
                            }, 
                            {
                                InstalledToolkitInfo.CustomizeVisibilityAttributeName, ExtensionVisibility.Hidden.ToString()
                            } 
                        }),
                    });

                Assert.Equal("Foo", target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Hidden, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUndefinedVisibility_ThenDefaultCustomizeVisibility()
            {
                this.mockExtension.Setup(ext => ext.Content).Returns(
                    new[]
                    { 
                        Mocks.Of<IExtensionContent>().First(c => c.ContentTypeName == InstalledToolkitInfo.ToolkitClassificationCustomExtensionName && c.Attributes == new Dictionary<string, string> 
                        { 
                            {
                                InstalledToolkitInfo.CustomizeVisibilityAttributeName, "Foo"
                            } 
                        }),
                    });

                Assert.Equal(String.Empty, target.Classification.Category);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CustomizeVisibility);
                Assert.Equal(ExtensionVisibility.Expanded, target.Classification.CreateVisibility);
            }
        }
    }
}
