using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.IntegrationTests;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class RuntimeShellVsixPackagingSpec
    {
        private static readonly IAssertion Assert = new Assertion();
        private const string DeployedContentDirectory = "Runtime.IntegrationTests.Content";
        private const string DeployedVsixItem = DeployedContentDirectory + "\\NuPatternToolkitManager.vsix";

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
                Assert.Equal(@"93373818-600f-414b-8181-3a0cb79fa785", this.VsixInfo.Header.Identifier);
                Assert.Equal(@"NuPattern Toolkit Manager", this.VsixInfo.Header.Name);
                Assert.Equal(@"Includes the 'Solution Builder' window, and the automation framework for accelerating solution development using NuPattern Toolkits.", this.VsixInfo.Header.Description);
                Assert.Equal(@"NuPattern", this.VsixInfo.Header.Author);
                Assert.Equal("1.3.20.0", this.VsixInfo.Header.Version.ToString());
                
                //License, Icon, PreviewImage, MoreInfoUrl, GettingStartedGuide
                Assert.Equal(@"LICENSE.txt", this.VsixInfo.Header.License);
                Assert.Equal(@"Resources\VsixIconRunTime.png", this.VsixInfo.Header.Icon);
                Assert.Equal(@"Resources\VsixPreviewRunTime.png", this.VsixInfo.Header.PreviewImage);
                Assert.Equal(new Uri(@"http://nupattern.codeplex.com"), this.VsixInfo.Header.MoreInfoUrl);
                Assert.Equal(new Uri(@"http://nupattern.codeplex.com/wikipage?title=Getting%20Started"), this.VsixInfo.Header.GettingStartedGuide);

                //SupportedFrameworkRuntimeEdition
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
                this.AssertFolderContainsExclusive(@"GeneratedCode",
                    new[]
                        {
                            "LibrarySchema.xsd",
                            "PatternModelSchema.xsd",
                            "ProductStateSchema.xsd",
                        });
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsGuidance()
            {
                //Guidance (\\GeneratedCode\Gudiance\Content\*
                this.AssertFolderNotEmpty(@"GeneratedCode\Guidance\Content", "*.mht");
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsAssets()
            {
            }

            [TestMethod, TestCategory("Integration")]
            public void ThenContainsResources()
            {
                //Schema files (\Resources\*)
                this.AssertFolderContainsExclusive(@"Resources",
                    new[]
                        {
                            "VsixIconRunTime.png",
                            "VsixPreviewRunTime.png",
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

                            //Auxillary Assemblies
                            "Microsoft.ComponentModel.Composition.Diagnostics.dll",
                            "Newtonsoft.Json.dll",

                            //EntLib Assemblies
                            "Microsoft.Practices.EnterpriseLibrary.Common.dll",
                            "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.HostAdapterV5.dll",
                            "Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime.dll",

                            //Runtime Assemblies
                            "NuPattern.Common.dll",
                            "NuPattern.Modeling.dll",
                            "NuPattern.Presentation.dll",
                            "NuPattern.VisualStudio.dll",
                            "NuPattern.VisualStudio.TemplateWizards.dll",
                            "NuPattern.Runtime.Extensibility.dll",
                            "NuPattern.Runtime.Extensibility.Serialization.dll",
                            "NuPattern.Library.dll",
                            "NuPattern.Runtime.Core.dll",
                            "NuPattern.Runtime.Guidance.dll",
                            "NuPattern.Runtime.Schema.dll",
                            "NuPattern.Runtime.Shell.dll",
                            "NuPattern.Runtime.Shell.pkgdef",
                            "NuPattern.Runtime.Store.dll",

                            // XML documentation
                            "NuPattern.Common.xml",
                            "NuPattern.Modeling.xml",
                            "NuPattern.Presentation.xml",
                            "NuPattern.VisualStudio.xml",
                            //"NuPattern.VisualStudio.TemplateWizards.xml",
                            "NuPattern.Runtime.Extensibility.xml",
                            //"NuPattern.Runtime.Extensibility.Serialization.xml",
                            "NuPattern.Library.xml",
                            //"NuPattern.Runtime.Core.xml",
                            //"NuPattern.Runtime.Schema.xml",
                            //"NuPattern.Runtime.Shell.xml",
                            //"NuPattern.Runtime.Store.xml",
                        });
            }
        }
    }
}
