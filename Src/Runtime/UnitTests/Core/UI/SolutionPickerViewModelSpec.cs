using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.UI;
using NuPattern.Runtime.UI.ViewModels;

namespace NuPattern.Runtime.UnitTests.UI
{
    [TestClass]
    public class SolutionPickerViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoFilterSolution
        {
            private SolutionPickerViewModel viewModel;
            private IPickerFilter filter;
            private FilteredItemContainer root;
            private Solution solution = new Solution
                {
                    Name = "Solution.sln",
                    PhysicalPath = "C:\\Temp",
                };

            [TestInitialize]
            public void InitializeContext()
            {
                this.filter = Mock.Of<IPickerFilter>(pf => pf.ApplyFilter(this.solution) == false);
                this.root = new FilteredItemContainer(this.solution, filter);
                this.viewModel = new SolutionPickerViewModel(this.root, filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenFilterNotNull()
            {
                Assert.Equal(this.filter, this.viewModel.Filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenEmptyItemsMessageNotNull()
            {
                Assert.NotNull(this.viewModel.EmptyItemsMessage);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenItemsEmpty()
            {
                Assert.False(this.viewModel.Items.Any());
            }
        }

        [TestClass]
        public class GivenAFilterSolution
        {
            private SolutionPickerViewModel viewModel;
            private IPickerFilter filter;
            private FilteredItemContainer root;
            private Solution solution = new Solution
            {
                Name = "Solution.sln",
                PhysicalPath = "C:\\Temp",
            };

            [TestInitialize]
            public void InitializeContext()
            {
                this.filter = Mock.Of<IPickerFilter>(pf => pf.ApplyFilter(this.solution) == true);
                this.root = new FilteredItemContainer(this.solution, filter);
                this.viewModel = new SolutionPickerViewModel(this.root, filter);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenSolutionItem()
            {
                Assert.Equal(1, this.viewModel.Items.Count());
            }
        }
    }
}
