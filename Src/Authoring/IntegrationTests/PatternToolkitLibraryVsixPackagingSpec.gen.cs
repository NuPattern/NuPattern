using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.IntegrationTests;

namespace NuPattern.Authoring.IntegrationTests
{
    [TestClass]
    public class PatternToolkitLibraryVsixPackagingSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private const string DeployedContentDirectory = "Authoring.IntegrationTests.Content";
        private const string DeployedVsixItem = DeployedContentDirectory + "\\NuPatternToolkitLibrary.vsix";

        [TestClass]
        [DeploymentItem(DeployedContentDirectory, DeployedContentDirectory)]
        public class GivenTheCompiledVsix : VsixPackagingSpec.GivenAVsix
        {
            /// <summary>
            /// Returns the relative path to the deployed Vsix file in the project
            /// </summary>
            protected override string DeployedVsixItemPath
            {
                get
                {
                    return DeployedVsixItem;
                }
            }
         
            [TestMethod, TestCategory("Integration")]
            public void ThenVsixInfoCorrect()
            {
                //Identifier, Name, Author, Version
                Assert.Equal(@"97bd7ab2-964b-43f1-8a08-be6db68b018b", this.VsixInfo.Header.Identifier);
                Assert.Equal(@"NuPattern Toolkit Library", this.VsixInfo.Header.Name);
                Assert.Equal(@"An extension for creating shared automation libraries for NuPattern Toolkits.", this.VsixInfo.Header.Description);
                Assert.Equal(@"NuPattern", this.VsixInfo.Header.Author);
                Assert.Equal("1.4.24.0", this.VsixInfo.Header.Version.ToString());
                
                //License, Icon, PreviewImage, MoreInfoUrl, GettingStartedGuide
                Assert.Equal(@"LICENSE.txt", this.VsixInfo.Header.License);
                Assert.Equal(@"Resources\VsixIconPatternToolkitLibrary.png", this.VsixInfo.Header.Icon);
                Assert.Equal(@"Resources\VsixPreviewPatternToolkitLibrary.png", this.VsixInfo.Header.PreviewImage);
                Assert.Equal(new Uri(@"http://nupattern.codeplex.com"), this.VsixInfo.Header.MoreInfoUrl);
                Assert.Equal(new Uri(@"http://nupattern.codeplex.com/wikipage?title=Getting%20Started"), this.VsixInfo.Header.GettingStartedGuide);

#if VSVER10
                Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkMinVersion.ToString());
                Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkMaxVersion.ToString());
#endif
#if VSVER11 || VSVER12
                Assert.Equal(@"4.5", this.VsixInfo.Header.SupportedFrameworkVersionRange.Minimum.ToString());
                Assert.Null(this.VsixInfo.Header.SupportedFrameworkVersionRange.Maximum);
#endif
                //SupportedProducts
#if VSVER10
                Assert.Equal(1, this.VsixInfo.Header.SupportedVSVersions.Count(t => t.Major.ToString() + "." + t.Minor.ToString() == "10.0"));
#endif
#if VSVER11
                Assert.Equal(1, this.VsixInfo.Targets.Count(t => t.VersionRange.Minimum.ToString() == "11.0"));
#endif
#if VSVER12
                Assert.Equal(1, this.VsixInfo.Targets.Count(t => t.VersionRange.Minimum.ToString() == "12.0"));
#endif
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsSchemas()
            {
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsGuidance()
            {
                //Assets (\\Assets\Guidance\*
                this.AssertFolderContainsExclusive(@"Assets\Guidance",
                    new[]
                        {
                            "ToolkitGuidance.doc",
                        });
            }
                
            [TestMethod, TestCategory("Integration")]
            public void ThenContainsAssets()
            {
                 //Templates (\\Assets\Templates\*
                this.AssertFolderContainsExclusive(@"Assets\Templates\Projects\Extensibility",
                    new[]
                        {
                            "Library.zip",
                        });
                this.AssertFolderContainsExclusive(@"Assets\Templates\Items\Extensibility",
                    new[]
                        {
                            "Command.zip",
                            "Condition.zip",
                            "DataTypeConverter.zip",
                            "ElementOrderingComparer.zip",
                            "ElementValidationRule.zip",
                            "EnumTypeConverter.zip",
                            "Event.zip",
                            "PropertyValidationRule.zip",
                            "TypePickerEditor.zip",
                            "UIEditor.zip",
                            "ValueProvider.zip",
                        });

                this.AssertFolderContainsExclusive(@"Assets\Templates\Text",
                    new[]
                        {
                            "CollectionImplementation.t4",
                            "CollectionInterface.t4",
                            "Common.t4include",
                            "Constants.t4include",
                            "ElementContainer.t4include",
                            "ElementImplementation.t4",
                            "ElementInterface.t4",
                            "ExtensionPointImplementation.t4",
                            "ExtensionPointInterface.t4",
                            "Header.t4include",
                            "NamedElementParent.t4include",
                            "ProductElement.t4include",
                            "ProductImplementation.t4",
                            "ProductInterface.t4",
                            "Utilities.t4include",
                            "ViewImplementation.t4",
                            "ViewInterface.t4",
                        });
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsResources()
            {
                //Schema files (\Resources\*)
                this.AssertFolderContainsExclusive(@"Resources",
                    new[]
                        {
                            "VsixIconPatternToolkitLibrary.png",
                            "VsixPreviewPatternToolkitLibrary.png",
                        });
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsRedistributables()
            {
                //Redist files
                this.AssertFolderContainsExclusive("",
                    new[]
                        {
                            "extension.vsixmanifest",
                            //"[Content_Types].xml",
                            "LICENSE.txt",

                            //Library Assemblies
                            "NuPattern.Authoring.PatternToolkitLibrary.dll",
                        });
            }
        }
    }
}
