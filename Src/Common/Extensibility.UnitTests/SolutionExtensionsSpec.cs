using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Extensibility.UnitTests
{
    public class SolutionExtensionsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenASolution
        {
            protected ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                this.solution = new Solution
                {
                    Name = "Solution.sln",
                    PhysicalPath = "C:\\Temp",
                    Items = 
					{
						new Item
						{
							Name = "Item.cs"
						},
						new Item
						{
							Name = "Item1.cs"
						},
						new Item
						{
							Name = "ItemNoExtension"
						},
						new SolutionFolder
						{
							Name = "Solution Items", 
							Items = 
							{
								new Item { Name = "Item.cs" }
							}
						},
						new Project
						{
							Name = "Project",
							Items = 
							{
								new Folder { Name = "Folder" },
								new Folder { Name = "Folder1" },
								new Item
								{
									Name = "ItemNoExtension"
								},
								new Item
								{
									Name = "Item.cs"
								},
								new Item
								{
									Name = "Item1.cs"
								},
							}
						},
						new Project
						{
							Name = "Project1",
							Items = 
							{
								new Folder { Name = "Folder" },
								new Folder { Name = "Folder1" },
							}
						},
					}
                };
            }
        }

        [TestClass]
        public class GivenASolutionForFindOrCreate : GivenASolution
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenEmptySolutionPath_ThenItIsCreatedAtSolution()
            {
                var evaluatedPath = string.Empty;

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(this.solution, target);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSlashSolutionPath_ThenItIsCreatedAtSolution()
            {
                var evaluatedPath = Path.DirectorySeparatorChar.ToString();

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(this.solution, target);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProjectFolderPath_ThenItIsCreated()
            {
                var evaluatedPath = "Project\\GeneratedCode\\Services\\Foo";

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(evaluatedPath, target.GetLogicalPath());
                Assert.Equal(ItemKind.Folder, target.Kind);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFolderPath_ThenItIsCreated()
            {
                var evaluatedPath = "Project\\Folder\\GeneratedCode\\Services\\Foo";

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(evaluatedPath, target.GetLogicalPath());
                Assert.Equal(ItemKind.Folder, target.Kind);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionRelativePath_ThenItIsCreated()
            {
                var evaluatedPath = "GeneratedCode\\Services\\Foo";

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(evaluatedPath, target.GetLogicalPath());
                Assert.Equal(ItemKind.SolutionFolder, target.Kind);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionFolderRelativePath_ThenItIsCreated()
            {
                var evaluatedPath = "Solution Items\\GeneratedCode\\Services\\Foo";

                var target = this.solution.FindOrCreate(evaluatedPath);

                Assert.NotNull(target);
                Assert.Equal(evaluatedPath, target.GetLogicalPath());
                Assert.Equal(ItemKind.SolutionFolder, target.Kind);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItemRelativePath_ThenThrowsNotSupportedException()
            {
                var evaluatedPath = "Solution Items\\Item.cs\\Services\\Foo";

                Assert.Throws<NotSupportedException>(() => this.solution.FindOrCreate(evaluatedPath));
            }
        }

        [TestClass]
        public class GivenASolutionForCalculateNextUniqueChildItemName : GivenASolution
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionChildItems_ThenThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => this.solution.CalculateNextUniqueChildItemName<ISolution>("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFolderChildItems_ThenThrows()
            {
                Assert.Throws<InvalidOperationException>(
                    () => this.solution.CalculateNextUniqueChildItemName<IFolder>("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNoSiblingProjects_ThenReturnsSeededName()
            {
                Assert.Equal("Foo", this.solution.CalculateNextUniqueChildItemName<IProject>("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOtherNamedSiblingProjects_ThenReturnsSeededName()
            {
                Assert.Equal("Foo", this.solution.CalculateNextUniqueChildItemName<IProject>("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSiblingProjects_ThenReturnsUniqueName()
            {
                Assert.Equal("Project2", this.solution.CalculateNextUniqueChildItemName<IProject>("Project"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSiblingSolutionFolders_ThenReturnsUniqueName()
            {
                Assert.Equal("Solution Items1", this.solution.CalculateNextUniqueChildItemName<ISolutionFolder>("Solution Items"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenOtherNamedSiblingItems_ThenReturnsUniqueName()
            {
                Assert.Equal("Foo.cs", this.solution.CalculateNextUniqueChildItemName<IItem>("Foo.cs"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSiblingItems_ThenReturnsUniqueName()
            {
                Assert.Equal("Item2.cs", this.solution.CalculateNextUniqueChildItemName<IItem>("Item.cs"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSolutionFolderChildItems_ThenThrows()
            {
                var project = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Throws<InvalidOperationException>(
                    () => project.CalculateNextUniqueChildItemName<ISolutionFolder>("Foo"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSameNamedSiblingItemsOfProject_ThenReturnsUniqueName()
            {
                var project = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("Item2.cs", project.CalculateNextUniqueChildItemName<IItem>("Item.cs"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDifferentNamedSiblingItemsOfProject_ThenReturnsSameName()
            {
                var project = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("Item3.cs", project.CalculateNextUniqueChildItemName<IItem>("Item3.cs"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCalculateNextUniqueChildItemNameNoExtensionWithSameNamedSiblingItemsOfProject_ThenReturnsUniqueName()
            {
                var project = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("ItemNoExtension1", project.CalculateNextUniqueChildItemName<IItem>("ItemNoExtension"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCalculateNextUniqueChildItemNameNoExtensionWithDifferentNamedSiblingItemsOfProject_ThenReturnsSameName()
            {
                var project = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("Item3", project.CalculateNextUniqueChildItemName<IItem>("Item3"));
            }
        }

        [TestClass]
        public class GivenASolutionForRename : GivenASolution
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenEmptyName_ThenThrows()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();

                Assert.Throws<ArgumentOutOfRangeException>(
                    () => item.Rename(string.Empty));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProposedNameIsSameAsItem_ThenReturnsName()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();
                Assert.Equal(item.Name, "Item.cs");

                Assert.Equal(item.Rename("Item.cs"), "Item.cs");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProposedNameHasDifferentExtension_ThenReturnsNameWithExtension()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();
                Assert.Equal(item.Name, "Item.cs");

                Assert.Equal(item.Rename("Foo.txt"), "Foo.txt");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProposedNameHasNoExtension_ThenReturnsNameWithExtension()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();
                Assert.Equal(item.Name, "Item.cs");

                Assert.Equal(item.Rename("Foo"), "Foo.cs");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProposedNameHasNoExtensionAndItemHasNoExtension_ThenReturnsNameWithoutExtension()
            {
                var item = this.solution.Items.OfType<IItem>().Where(x => x.Name == "ItemNoExtension").FirstOrDefault();
                Assert.Equal(item.Name, "ItemNoExtension");

                Assert.Equal(item.Rename("Foo"), "Foo");
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProposedNameAlreadyExists_ThenThrows()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();
                Assert.Equal(item.Name, "Item.cs");

                Assert.Throws<InvalidOperationException>(
                    () => item.Rename("Item1.cs"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenItem_ThenItemIsRenamed()
            {
                var parent = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("Project", parent.Name);

                var item = new Mock<IItem>();
                item.Setup(i => i.Name).Returns("Bar.cs");
                item.Setup(i => i.Parent).Returns(parent);
                var vsItem = new Mock<EnvDTE.ProjectItem>();
                vsItem.SetupAllProperties();
                item.Setup(i => i.As<EnvDTE.ProjectItem>()).Returns(vsItem.Object);

                var result = item.Object.Rename("Foo");

                vsItem.VerifySet(v => v.Name = "Foo.cs");
                Assert.Equal("Foo.cs", result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProject_ThenProjectIsRenamed()
            {
                var parent = this.solution.Items.OfType<IProject>().FirstOrDefault();
                Assert.Equal("Project", parent.Name);

                var project = new Mock<IProject>();
                project.Setup(i => i.Name).Returns("Bar");
                project.Setup(i => i.Parent).Returns(parent);
                var vsProject = new Mock<EnvDTE.Project>();
                vsProject.SetupAllProperties();
                project.Setup(i => i.As<EnvDTE.Project>()).Returns(vsProject.Object);

                var result = project.Object.Rename("Foo");

                vsProject.VerifySet(v => v.Name = "Foo");
                Assert.Equal("Foo", result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenUniquenessAndProposedNameExists_ThenReturnsUniqueName()
            {
                var item = this.solution.Items.OfType<IItem>().FirstOrDefault();
                Assert.Equal(item.Name, "Item.cs");

                Assert.Equal("Item11.cs", item.Rename("Item1.cs", true));
            }
        }
    }
}
