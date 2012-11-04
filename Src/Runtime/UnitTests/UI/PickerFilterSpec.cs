using Microsoft.VisualStudio.TeamArchitect.PowerTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI.UnitTests
{
	[TestClass]
	public class PickerFilterSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestMethod]
		public void WhenCreatingANewPickerFilter_ThenKindIsAllItemsByDefault()
		{
			const ItemKind AllKinds = ItemKind.Folder | ItemKind.Item | ItemKind.Project | ItemKind.Reference |
				ItemKind.ReferencesFolder | ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Unknown;

			var target = new PickerFilter();

			Assert.Equal(AllKinds, target.Kind);
		}

		[TestMethod]
		public void WhenDefininingKindContainingItem_ThenAppliesFilterReturnsTrue()
		{
			var child = new Mock<IItemContainer>();
			child.SetupGet(item => item.Kind).Returns(ItemKind.Item);

			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Project);
			container.SetupGet(item => item.Items).Returns(new[] { child.Object });

			var target = new PickerFilter { Kind = ItemKind.Project | ItemKind.Item };

			Assert.True(target.ApplyFilter(container.Object));
		}

		[TestMethod]
		public void WhenDefininingKindNotContainingItem_ThenAppliesFilterReturnsFalse()
		{
			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Project);

			var target = new PickerFilter { Kind = ItemKind.Item | ItemKind.Folder };

			Assert.False(target.ApplyFilter(container.Object));
		}

		[TestMethod]
		public void WhenNotIncludingEmptyFoldersAndItemDoesNotHaveChildren_ThenAppliesFilerReturnsFalse()
		{
			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Project);
			container.SetupGet(item => item.Items).Returns(new IItemContainer[0]);

			var target = new PickerFilter { Kind = ItemKind.Project | ItemKind.Folder, IncludeEmptyContainers = false };

			Assert.False(target.ApplyFilter(container.Object));
		}

		[TestMethod]
		public void WhenKindIsItemAndDefininingFileExtensionsAndItemMachOneInTheList_ThenAppliesFilterReturnsTrue()
		{
			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Item);
			container.SetupGet(item => item.PhysicalPath).Returns(@"X:\myfile.foo");

			var target = new PickerFilter { Kind = ItemKind.Item, IncludeEmptyContainers = true, IncludeFileExtensions = ".foo .bar" };

			Assert.True(target.ApplyFilter(container.Object));
		}

		[TestMethod]
		public void WhenKindIsItemAndDefininingFileExtensionsAndItemDoesNotMachOneInTheList_ThenAppliesFilterReturnsFalse()
		{
			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Item);
			container.SetupGet(item => item.PhysicalPath).Returns(@"X:\myfile.bar");

			var target = new PickerFilter { Kind = ItemKind.Item, IncludeEmptyContainers = true, IncludeFileExtensions = ".foo" };

			Assert.False(target.ApplyFilter(container.Object));
		}

		[TestMethod]
		public void WhenNotIncludingEmptyFolderAndItemHasChildrenThatNotMatchFilter_ThenAppliesFilterReturnsFalse()
		{
			var child = new Mock<IItemContainer>();
			child.SetupGet(item => item.Kind).Returns(ItemKind.Unknown);

			var container = new Mock<IItemContainer>();
			container.SetupGet(item => item.Kind).Returns(ItemKind.Folder);
			container.SetupGet(item => item.Items).Returns(new[] { child.Object });

			var target = new PickerFilter { Kind = ItemKind.Folder | ItemKind.Item, IncludeEmptyContainers = false };

			Assert.False(target.ApplyFilter(container.Object));
		}
	}
}