using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Design;
using NuPattern.Runtime;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.UnitTests
{
    public class TextTemplateUriEditorSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [DeploymentItem("Library.UnitTests.Content\\extension.vsixmanifest", "Library.UnitTests.Content\\TextTemplateUriEditorSpec\\GivenAVsixProject")]
        public class GivenAVsixProject
        {
            private ISolution solution;

            [TestInitialize]
            public void InitializeContext()
            {
                var manifest = new Item
                {
                    Name = "source.extension.vsixmanifest",
                    PhysicalPath = "Library.UnitTests.Content\\TextTemplateUriEditorSpec\\GivenAVsixProject\\extension.vsixmanifest",
                };
                manifest.Data.IsToolkitManifest = "true";

                this.solution = new Solution
                {
                    Name = "Solution.sln",
                    PhysicalPath = "C:\\Temp",
                    Items = 
                    {
                        new SolutionFolder
                        {
                            Name = "Solution Items", 
                            Items = 
                            {
                                new Item { Name = "SolutionItem.tt" }
                            }
                        },
                        new Project
                        {
                            Name = "Project",
                            Items = 
                            {
                                new Item { Name = "NormalProject.tt" }
                            }
                        },
                        new Project
                        {
                            Name = "VsixProject",
                            Items = 
                            {
                                new Folder
                                {
                                    Name = "Templates", 
                                    Items = 
                                    {
                                        new Folder
                                        {
                                            Name = "Text", 
                                            Items = 
                                            {
                                                new Item
                                                {
                                                    Name = "VsixTemplate.tt",
                                                },
                                                new Item
                                                {
                                                    Name = "VsixTemplateAs.foo.tt",
                                                }
                                            }
                                        }
                                    }
                                }, 
                                manifest
                            }
                        }
                    }
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingUriForTemplateInProject_ThenUsesVsixIdentifierAndRelativePathToFile()
            {
                var uri = TextTemplateUriEditor.BuildUri(this.solution.Traverse().First(i => i.Name == "VsixTemplate.tt"));

                Assert.Equal("t4", uri.Scheme);
                Assert.Equal(TextTemplateUri.ExtensionRelativeHost, uri.Host);
                Assert.Equal("/ef4561f7-a3ea-4666-a080-bc2f195451e3/Templates/Text/VsixTemplate.tt", uri.PathAndQuery);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingUriForTemplateOutsideAProject_ThenThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => TextTemplateUriEditor.BuildUri(solution.Traverse().First(i => i.Name == "SolutionItem.tt")));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingUriForTemplateFromNonVsixProject_ThenThrowsArgumentException()
            {
                Assert.Throws<ArgumentException>(() => TextTemplateUriEditor.BuildUri(solution.Traverse().First(i => i.Name == "NormalProject.tt")));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenBuildingUriForTemplateWithIncludeInVSIXAs_ThenUsesFilenameFromIncludeInVSIXAs()
            {
                var item = this.solution.Traverse().First(i => i.Name == "VsixTemplateAs.foo.tt") as IItem;
                item.Data.IncludeInVSIXAs = "NewTemplate.t4";

                var uri = TextTemplateUriEditor.BuildUri(item);

                Assert.Equal("t4", uri.Scheme);
                Assert.Equal(TextTemplateUri.ExtensionRelativeHost, uri.Host);
                Assert.Equal("/ef4561f7-a3ea-4666-a080-bc2f195451e3/Templates/Text/NewTemplate.t4", uri.PathAndQuery);
            }
        }

    }
}