using System.Linq;
using Microsoft.VisualStudio.Patterning.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
    [TestClass]
    public class RuntimeShellVsixPackagingSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private const string DeployedContentDirectory = "Runtime.IntegrationTests.Content";
        private const string DeployedVsixItem = DeployedContentDirectory + "\\PatternToolkitManager.11.0.vsix";

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
                Assert.Equal(@"c869918e-f94e-4e7a-ab25-b076ff4e751b", this.VsixInfo.Header.Identifier);
                Assert.Equal(@"Pattern Toolkit Manager", this.VsixInfo.Header.Name);
                Assert.Equal(@"The Solution Builder tool window for accelerating solution development using Pattern Toolkit extensions.", this.VsixInfo.Header.Description);
                Assert.Equal(@"Outercurve", this.VsixInfo.Header.Author);
                Assert.Equal("1.3.20.0", this.VsixInfo.Header.Version.ToString());
				
				//License, Icon, PreviewImage, MoreInfoUrl, GettingStartedGuide
				Assert.Equal(@"LICENSE.txt", this.VsixInfo.Header.License);
				Assert.Equal(@"Resources\RunTimeVsix.png", this.VsixInfo.Header.Icon);
				Assert.Equal(@"Resources\RunTimeVsixPreview.png", this.VsixInfo.Header.PreviewImage);
				Assert.Equal(@"http://visualstudiogallery.msdn.microsoft.com/332f060b-2352-41c9-b8dc-95d8ad21329b", this.VsixInfo.Header.MoreInfoUrl.ToString());
				Assert.Equal(@"http://visualstudiogallery.msdn.microsoft.com/332f060b-2352-41c9-b8dc-95d8ad21329b", this.VsixInfo.Header.GettingStartedGuide.ToString());

                //SupportedFrameworkRuntimeEdition
				Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkVersionRange.Minimum.ToString());
#if VSVER10
				Assert.Equal(@"4.0", this.VsixInfo.Header.SupportedFrameworkVersionRange.Maximum.ToString());
#endif
#if VSVER11
                Assert.Equal(@"4.5", this.VsixInfo.Header.SupportedFrameworkVersionRange.Maximum.ToString());
#endif
                //SupportedProducts
#if VSVER10
                Assert.Equal(1, this.VsixInfo.Targets.Count(t => t.VersionRange.Minimum.ToString() == "10.0"));
#endif
#if VSVER11
                Assert.Equal(1, this.VsixInfo.Targets.Count(t => t.VersionRange.Minimum.ToString() == "11.0"));
#endif
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsSchemas()
            {
                //Schema files (\GeneratedCode\*)
                Assert.True(this.FolderContainsExclusive(@"GeneratedCode",
                    new[]
                        {
                            "LibrarySchema.xsd",
                            "PatternModelSchema.xsd",
                            "ProductStateSchema.xsd",
                        }));
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsGuidance()
            {
                //Guidance (\\GeneratedCode\Gudiance\Content\*
				Assert.True(this.FolderNotEmpty(@"GeneratedCode\Guidance\Content", "*.mht"));
            }

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsAssets()
            {
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsResources()
            {
                //Schema files (\Resources\*)
                Assert.True(this.FolderContainsExclusive(@"Resources",
                    new[]
                        {
                            "RunTimeVsix.png",
                            "RunTimeVsixPreview.png",
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

                            //Auxillary Assemblies
                            "Microsoft.ComponentModel.Composition.Diagnostics.dll",
                            "Newtonsoft.Json.dll",

                            //Feature Extension Assemblies
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.dll",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.dll",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Library.dll",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Shell.dll",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Features.Shell.pkgdef",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Shell.dll",
                            "Microsoft.VisualStudio.TeamArchitect.PowerTools.Shell.pkgdef",

                            //EntLib Assemblies
                            "Microsoft.Practices.EnterpriseLibrary.Common.dll",
                            "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.HostAdapterV5.dll",
                            "Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime.dll",

                            //Runtime Assemblies
                            "Microsoft.VisualStudio.Patterning.Common.Presentation.dll",
                            "Microsoft.VisualStudio.Patterning.Extensibility.dll",
                            "Microsoft.VisualStudio.Patterning.Extensibility.Serialization.dll",
                            "Microsoft.VisualStudio.Patterning.Library.dll",
                            "Microsoft.VisualStudio.Patterning.Runtime.dll",
                            "Microsoft.VisualStudio.Patterning.Runtime.Interfaces.dll",
                            "Microsoft.VisualStudio.Patterning.Runtime.Schema.dll",
                            "Microsoft.VisualStudio.Patterning.Runtime.Shell.dll",
                            "Microsoft.VisualStudio.Patterning.Runtime.Shell.pkgdef",
                            "Microsoft.VisualStudio.Patterning.Runtime.Store.dll",

							// XML documentation
                            //"Microsoft.VisualStudio.Patterning.Common.Presentation.xml",
                            "Microsoft.VisualStudio.Patterning.Extensibility.xml",
                            //"Microsoft.VisualStudio.Patterning.Extensibility.Serialization.xml",
                            "Microsoft.VisualStudio.Patterning.Library.xml",
                            "Microsoft.VisualStudio.Patterning.Runtime.xml",
                            "Microsoft.VisualStudio.Patterning.Runtime.Interfaces.xml",
                            "Microsoft.VisualStudio.Patterning.Runtime.Schema.xml",
                            //"Microsoft.VisualStudio.Patterning.Runtime.Shell.xml",
                            //"Microsoft.VisualStudio.Patterning.Runtime.Store.xml",
                        }));
            }
        }
    }
}
