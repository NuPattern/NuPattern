using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
    [TestClass]
    public class AutomationLibraryVsixPackagingSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private const string DeployedContentDirectory = "Authoring.IntegrationTests.Content";
        private const string DeployedVsixItem = DeployedContentDirectory + "\\PatternToolkitAutomationLibrary.10.0.vsix";

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
                Assert.Equal(@"Pattern Toolkit Automation Library", this.VsixInfo.Header.Name);
                Assert.Equal(@"An extension for creating shared automation libraries for Pattern Toolkit extensions.", this.VsixInfo.Header.Description);
                Assert.Equal(@"Outercurve", this.VsixInfo.Header.Author);
                Assert.Equal("1.3.20.0", this.VsixInfo.Header.Version.ToString());
				
				//License, Icon, PreviewImage, MoreInfoUrl, GettingStartedGuide
				Assert.Equal(@"LICENSE.txt", this.VsixInfo.Header.License);
				Assert.Equal(@"Resources\VsixIconAutomationLibrary.png", this.VsixInfo.Header.Icon);
				Assert.Equal(@"Resources\VsixPreviewAutomationLibrary.png", this.VsixInfo.Header.PreviewImage);
				Assert.Equal(new Uri(@"http://vspat.codeplex.com"), this.VsixInfo.Header.MoreInfoUrl);
				Assert.Equal(new Uri(@"http://vspat.codeplex.com/wikipage?title=Getting%20Started"), this.VsixInfo.Header.GettingStartedGuide);

#if VSVER10
				Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkMinVersion.ToString());
				Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkMaxVersion.ToString());
#endif
#if VSVER11
				Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkVersionRange.Minimum.ToString());
                Assert.Equal(@"4.5", this.VsixInfo.Header.SupportedFrameworkVersionRange.Maximum.ToString());
#endif
                //SupportedProducts
#if VSVER10
                Assert.Equal(1, this.VsixInfo.Header.SupportedVSVersions.Count(t => t.Major.ToString() + "." + t.Minor.ToString() == "10.0"));
#endif
#if VSVER11
                Assert.Equal(1, this.VsixInfo.Targets.Count(t => t.VersionRange.Minimum.ToString() == "11.0"));
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
				Assert.True(this.FolderContainsExclusive(@"Assets\Guidance",
                    new[]
                        {
							"ToolkitGuidance.doc",
						}));
			}
                
			[TestMethod, TestCategory("Integration")]
            public void ThenContainsAssets()
            {
				 //Templates (\\Assets\Templates\*
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Projects\Extensibility",
                    new[]
                        {
							"Library.zip",
						}));
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Items\Extensibility",
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
						}));

				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text",
                    new[]
                        {
							"CollectionImplementation.t4",
							"CollectionInterface.t4",
							"Constants.gen.t4",
							"ElementContainerImplementation.t4",
							"ElementContainerInterface.t4",
							"ElementImplementation.t4",
							"ElementInterface.t4",
							"ExtensionPointImplementation.t4",
							"ExtensionPointInterface.t4",
							"Header.t4",
							"Helpers.t4",
							"NamedElementParentImplementation.t4",
							"NamedElementParentInterface.t4",
							"ProductElementImplementation.t4",
							"ProductElementInterface.t4",
							"ProductImplementation.t4",
							"ProductInterface.t4",
							"ViewImplementation.t4",
							"ViewInterface.t4",
						}));
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsResources()
            {
                //Schema files (\Resources\*)
                Assert.True(this.FolderContainsExclusive(@"Resources",
                    new[]
                        {
                            "VsixIconAutomationLibrary.png",
                            "VsixPreviewAutomationLibrary.png",
                        }));
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsRedistributables()
            {
                //Redist files
                Assert.True(this.FolderContainsExclusive("",
                    new[]
                        {
							"extension.vsixmanifest",
							//"[Content_Types].xml",
                            "LICENSE.txt",

                            //Library Assemblies
                            "Microsoft.VisualStudio.Patterning.Authoring.Library.Toolkit.dll",
                        }));
            }
        }
    }
}
