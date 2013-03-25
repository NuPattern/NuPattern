using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.UI;

namespace NuPattern.Runtime.UnitTests.UI
{
    [TestClass]
    public class PickerFilterSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private PickerFilter filter;

            private Solution solution = new Solution
                {
                    Items = 
                        {
                            new Project
                                {
                                    Name = "ProjectWithFiles",
                                    PhysicalPath = @"C:\Temp\ProjectWithFiles.proj",
                                    Items =
                                        {
                                            new Item
                                                {
                                                    Name = "File.withkids",
                                                    PhysicalPath = @"C:\Temp\File.withkids",
                                                    Items =
                                                        {
                                                            new Item
                                                                {
                                                                    Name = "NestedFile.nested",
                                                                    PhysicalPath = @"C:\Temp\NestedFile.nested",
                                                                },
                                                        },
                                                },
                                            new Item
                                                {
                                                    Name = "File.nokids",
                                                    PhysicalPath = @"C:\Temp\File.nokids",
                                                },
                                        }
                                },
                            new Project
                                {
                                    Name = "ProjectWithNoFiles",
                                    PhysicalPath = @"C:\Temp\ProjectWithNoFiles.proj",
                                },
                        }
                };

            [TestInitialize]
            public void InitializeContext()
            {
                this.filter = new PickerFilter();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenKindIsAllKinds()
            {
                Assert.Equal(PickerFilter.AllKinds, this.filter.Kind);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIncludeFileExtensionsIsEmpty()
            {
                Assert.Equal(string.Empty, this.filter.MatchFileExtensions);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIncludeEmptyContainersIsFalse()
            {
                Assert.False(this.filter.IncludeEmptyContainers);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIncludedItemsEmpty()
            {
                Assert.NotNull(this.filter.MatchItems);
                Assert.False(this.filter.MatchItems.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterToItemIncludingItems_ThenReturnsTrue()
            {
                this.filter.Kind = ItemKind.Item;

                Assert.True(this.filter.MatchesFilter(this.solution.Find<IItem>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterToItemExcludingItems_ThenReturnsFalse()
            {
                this.filter.Kind = ItemKind.Unknown;

                Assert.False(this.filter.MatchesFilter(this.solution.Find<IItem>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterToProjectContainingItemIncludingItemsOnly_ThenReturnsFalse()
            {
                this.filter.Kind = ItemKind.Item;

                Assert.False(this.filter.MatchesFilter(this.solution.Find<IProject>(p => p.Name == "ProjectWithFiles").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithIncludedItemNotSame_ThenReturnsFalse()
            {
                this.filter.MatchItems = new List<IItemContainer>(this.solution.Find<IItem>(p => p.Name == "File.withkids"));

                Assert.False(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.nokids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithIncludedItemSame_ThenReturnsTrue()
            {
                this.filter.MatchItems = new List<IItemContainer>(this.solution.Find<IItem>(p => p.Name == "File.withkids"));

                Assert.True(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithExtensionFilterNotMatch_ThenReturnsFalse()
            {
                this.filter.MatchFileExtensions = ".bar";

                Assert.False(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithSingleExtensionFilterMatch_ThenReturnsTrue()
            {
                this.filter.MatchFileExtensions = ".withkids";

                Assert.True(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithMultipleExtensionFilterMatch_ThenReturnsTrue()
            {
                this.filter.MatchFileExtensions = ".dll.withkids.exe";

                Assert.True(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithInvalidExtensionFilterMatch_ThenReturnsFalse()
            {
                this.filter.MatchFileExtensions = "top";

                Assert.False(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithIncludedItemNotSameAndExtensionFilterMatch_ThenReturnsTrue()
            {
                this.filter.MatchItems = new List<IItemContainer>(this.solution.Find<IItem>(p => p.Name == "File.withkids"));
                this.filter.MatchFileExtensions = ".nokids";

                Assert.True(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.nokids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMatchesFilterWithIncludedItemSameAndExtensionFilterNotMatch_ThenReturnsTrue()
            {
                this.filter.MatchItems = new List<IItemContainer>(this.solution.Find<IItem>(p => p.Name == "File.withkids"));
                this.filter.MatchFileExtensions = ".bar";

                Assert.True(this.filter.MatchesFilter(this.solution.Find<Item>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithNotAllowedKind_ThenReturnsFalse()
            {
                this.filter.MatchItems = new[] { Mock.Of<IItemContainer>() };
                this.filter.Kind = ItemKind.Unknown;

                Assert.False(this.filter.ApplyFilter(this.solution.Find<IItem>(p => p.Name == "File.nokids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotNotionalContainerKindWithNoChildren_ThenReturnsFalse()
            {
                this.filter.MatchItems = new[] { Mock.Of<IItemContainer>() };
                this.filter.Kind = ItemKind.Item;

                Assert.False(this.filter.ApplyFilter(this.solution.Find<IItem>(p => p.Name == "File.nokids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotNotionalContainerKindWithChildrenThatMatch_ThenReturnsTrue()
            {
                this.filter.MatchFileExtensions = ".nested";
                this.filter.Kind = ItemKind.Item;

                Assert.True(this.filter.ApplyFilter(this.solution.Find<IItem>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotNotionalContainerKindWithChildrenThatDontMatch_ThenReturnsFalse()
            {
                this.filter.MatchFileExtensions = ".foo";
                this.filter.Kind = ItemKind.Item;

                Assert.False(this.filter.ApplyFilter(this.solution.Find<IItem>(p => p.Name == "File.withkids").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotionalContainerKindWithChildrenThatMatch_ThenReturnsTrue()
            {
                this.filter.MatchFileExtensions = ".withkids";
                this.filter.Kind = ItemKind.Project;

                Assert.True(this.filter.ApplyFilter(this.solution.Find<IProject>(p => p.Name == "ProjectWithFiles").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotionalContainerKindWithChildrenThatDontMatch_ThenReturnsFalse()
            {
                this.filter.MatchFileExtensions = ".foo";
                this.filter.Kind = ItemKind.Project;

                Assert.False(this.filter.ApplyFilter(this.solution.Find<IProject>(p => p.Name == "ProjectWithFiles").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotionalContainerKindWithNoChildrenAndAllowEmptyContainers_ThenReturnsTrue()
            {
                this.filter.MatchFileExtensions = ".foo";
                this.filter.IncludeEmptyContainers = true;
                this.filter.Kind = ItemKind.Project;

                Assert.True(this.filter.ApplyFilter(this.solution.Find<IProject>(p => p.Name == "ProjectWithNoFiles").First()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenApplyFilterToNonMatchWithAllowedKindAndNotionalContainerKindWithNoChildrenAndNotAllowEmptyContainers_ThenReturnsFalse()
            {
                this.filter.MatchFileExtensions = ".foo";
                this.filter.IncludeEmptyContainers = false;
                this.filter.Kind = ItemKind.Project;

                Assert.False(this.filter.ApplyFilter(this.solution.Find<IProject>(p => p.Name == "ProjectWithNoFiles").First()));
            }
        }
    }
}