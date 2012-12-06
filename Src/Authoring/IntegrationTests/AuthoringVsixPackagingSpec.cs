using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.IntegrationTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Authoring.IntegrationTests
{
    [TestClass]
    public class AuthoringVsixPackagingSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private const string DeployedContentDirectory = "Authoring.IntegrationTests.Content";
        private const string DeployedVsixItem = DeployedContentDirectory + "\\PatternToolkitBuilder.11.0.vsix";

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
                Assert.Equal(@"84031a32-b20f-479c-a620-beacd982ea13", this.VsixInfo.Header.Identifier);
                Assert.Equal(@"Pattern Toolkit Builder VS2012", this.VsixInfo.Header.Name);
                Assert.Equal(@"An extension for designing and building Pattern Toolkit extensions, that combine automation and guidance with design patterns for repeatable solution development.", this.VsixInfo.Header.Description);
                Assert.Equal(@"Outercurve", this.VsixInfo.Header.Author);
                Assert.Equal("1.3.20.0", this.VsixInfo.Header.Version.ToString());
				
				//License, Icon, PreviewImage, MoreInfoUrl, GettingStartedGuide
				Assert.Equal(@"LICENSE.txt", this.VsixInfo.Header.License);
				Assert.Equal(@"Resources\VsixIconAuthoring.png", this.VsixInfo.Header.Icon);
				Assert.Equal(@"Resources\VsixPreviewAuthoring.png", this.VsixInfo.Header.PreviewImage);
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
                //Schema files (\GeneratedCode\*)
                Assert.True(this.FolderContainsExclusive(@"GeneratedCode",
                    new[]
                        {
                            "WorkflowDesignSchema.xsd",
                        }));
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsGuidance()
            {
                //Guidance (\\GeneratedCode\Gudiance\Content\*
				Assert.True(this.FolderNotEmpty(@"GeneratedCode\Guidance\Content", "*.mht"));

                //Assets (\\Assets\Guidance\*
				Assert.True(this.FolderContainsExclusive(@"Assets\Guidance",
                    new[]
                        {
							"AuthoringToolkitGuidance.pdf",
							"PatternToolkitGuidanceTemplate.dotm",
						}));
			}
                
			[TestMethod, TestCategory("Integration")]
            public void ThenContainsAssets()
            {
				 //Templates (\\Assets\Templates\*
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Projects\Extensibility",
                    new[]
                        {
							"Authoring.zip",
						}));
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Items\Extensibility",
                    new[]
                        {
							"ItemTemplate.zip",
							"PatternTooling.zip",
							"ProjectTemplate.zip",
							"TextTemplate.zip",
							"Wizard.zip",
							"WizardPage.zip",
						}));

				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text",
                    new[]
                        {
							"Header.t4",
						}));

				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text\Guidance",
                    new[]
                        {
							"GuidanceWorkflow.t4",
							"ToolkitGuidance.t4",
						}));

				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text\ItemTemplate",
                    new[]
                        {
							"ItemTemplate.vstemplate.t4",
						}));

				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text\PatternModel",
                    new[]
                        {
							"PatternModel.patterndefinition.t4",
						}));
				
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text\ProjectTemplate",
                    new[]
                        {
							"ProjectTemplate.vstemplate.t4",
						}));
				
				Assert.True(this.FolderContainsExclusive(@"Assets\Templates\Text\VsixManifest",
                    new[]
                        {
							"source.extension.t4",
							"source.include.t4",
						}));
			}

			[TestMethod, TestCategory("Integration")]
            public void ThenContainsResources()
            {
                //Schema files (\Resources\*)
                Assert.True(this.FolderContainsExclusive(@"Resources",
                    new[]
                        {
                            "VsixIconAuthoring.png",
                            "VsixPreviewAuthoring.png",
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
							"redist.txt",
							"Release%20Notes.docx",

                            //Auxillary Assemblies
							"Microsoft.VisualStudio.Patterning.Runtime.Schema.dll",

                            //Authoring Assemblies
                            "Microsoft.VisualStudio.Patterning.Authoring.Authoring.Toolkit.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.Authoring.Toolkit.pkgdef",
                            "Microsoft.VisualStudio.Patterning.Authoring.Guidance.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.PatternToolkit.targets",
                            "Microsoft.VisualStudio.Patterning.Authoring.Toolkit.Automation.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.PatternModelDesign.Shell.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.PatternModelDesign.Shell.pkgdef",
                            "Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Interfaces.dll",
                            "Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Shell.dll",
							"Microsoft.VisualStudio.Patterning.Authoring.WorkflowDesign.Shell.pkgdef",

							//Embedded VSIXes
							"PatternToolkitAutomationLibrary.11.0.vsix",
							"PatternToolkitManager.11.0.vsix",
                        }));
            }
        }
    }
}
