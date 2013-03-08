using System.Linq;
using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.UI;

namespace NuPattern.Runtime.UnitTests.UI
{
    [TestClass]
    public class FilteredItemContainerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAContainerWithTopLevelAsMatchingItem
        {
            private FilteredItemContainer container;
            private IItemContainer item;
            private IPickerFilter filter;

            [TestInitialize]
            public void InitializeContext()
            {
                this.item = Mock.Of<IItemContainer>(i =>
                    i.Items == Enumerable.Empty<IItemContainer>()
                    && i.Icon == System.Drawing.SystemIcons.Application);
                this.filter = Mock.Of<IPickerFilter>(pf =>
                    pf.ApplyFilter(this.item) == false
                    && pf.MatchesFilter(this.item) == true);
                this.container = new FilteredItemContainer(this.item, this.filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenParentIsNull()
            {
                Assert.Null(this.container.Parent);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIconIsItemIcon()
            {
                Assert.NotNull(this.container.Icon);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenItemIsItem()
            {
                Assert.Equal(this.item, this.container.Item);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenItemsIsEmpty()
            {
                Assert.False(this.container.Items.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsSelectableIsTrue()
            {
                Assert.True(this.container.IsSelectable);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsExpandedIsFalse()
            {
                Assert.False(this.container.IsExpanded);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsSelectedIsFalse()
            {
                Assert.False(this.container.IsSelected);
            }
        }

        [TestClass]
        public class GivenAContainerWithTopLevelAsNonMatchingItem
        {
            private FilteredItemContainer container;
            private IItemContainer item;
            private IPickerFilter filter;

            [TestInitialize]
            public void InitializeContext()
            {
                this.item = Mock.Of<IItemContainer>(i =>
                    i.Items == Enumerable.Empty<IItemContainer>());
                this.filter = Mock.Of<IPickerFilter>(pf =>
                    pf.ApplyFilter(this.item) == false
                    && pf.MatchesFilter(this.item) == false);
                this.container = new FilteredItemContainer(this.item, this.filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsSelectableIsFalse()
            {
                Assert.False(this.container.IsSelectable);
            }
        }

        [TestClass]
        public class GivenAContainerWithTopLevelParentAndMatchingChild
        {
            private FilteredItemContainer container;
            private IItemContainer item;
            private IPickerFilter filter;
            private IItemContainer childItem;

            [TestInitialize]
            public void InitializeContext()
            {
                this.childItem = Mock.Of<IItemContainer>();
                this.item = Mock.Of<IItemContainer>(i =>
                    i.Items == new[] { this.childItem });
                this.filter = Mock.Of<IPickerFilter>(pf =>
                    pf.ApplyFilter(this.childItem) == true
                    && pf.MatchesFilter(this.childItem) == true);
                this.container = new FilteredItemContainer(this.item, this.filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenItemsContainsChild()
            {
                Assert.True(this.container.Items.Any());
                Assert.Equal(this.childItem, this.container.Items.First().Item);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsSelectableIsFalse()
            {
                Assert.False(this.container.IsSelectable);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenChildItemIsSelectableIsTrue()
            {
                Assert.True(this.container.Items.First().IsSelectable);
            }
        }

        [TestClass]
        public class GivenAContainerWithTopLevelParentAndNonMatchingChild
        {
            private FilteredItemContainer container;
            private IItemContainer item;
            private IPickerFilter filter;
            private IItemContainer childItem;

            [TestInitialize]
            public void InitializeContext()
            {
                this.childItem = Mock.Of<IItemContainer>();
                this.item = Mock.Of<IItemContainer>(i =>
                    i.Items == new[] { this.childItem });
                this.filter = Mock.Of<IPickerFilter>(pf =>
                    pf.ApplyFilter(this.childItem) == false
                    && pf.MatchesFilter(this.childItem) == false);
                this.container = new FilteredItemContainer(this.item, this.filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenItemsNotContainsChild()
            {
                Assert.False(this.container.Items.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsSelectableIsFalse()
            {
                Assert.False(this.container.IsSelectable);
            }
        }
    }
}
