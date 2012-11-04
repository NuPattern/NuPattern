using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI.UnitTests
{
    public class AddNewProductViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullPatternManager_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new AddNewProductViewModel(null, new Mock<IUserMessageService>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullUserMessageService_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(
                    () => new AddNewProductViewModel(new Mock<IPatternManager>().Object, null));
            }
        }

        [TestClass]
        public class GivenAnEmptyProductState
        {
            private AddNewProductViewModel target;
            private Mock<IPatternManager> patternManager;

            [TestInitialize]
            public void Initialize()
            {
                var products = new List<IProduct>();

                var factories = new[] { GetInstalledToolkit("Foo"), GetInstalledToolkit("Bar") };

                this.patternManager = new Mock<IPatternManager>();
                this.patternManager.Setup(pm => pm.InstalledToolkits).Returns(factories);
                this.patternManager.Setup(pm => pm.CreateProduct(It.IsAny<IInstalledToolkitInfo>(), It.IsAny<string>(), true))
                    .Callback<IInstalledToolkitInfo, string, bool>((f, n, r) => products.Add(Mocks.Of<IProduct>().First(p => p.InstanceName == n)));
                this.patternManager.Setup(pm => pm.Products)
                    .Returns(products);

                this.target = new AddNewProductViewModel(this.patternManager.Object, new Mock<IUserMessageService>().Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenFactoriesIsExposed()
            {
                Assert.Equal(2, this.target.Toolkits.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenCurrentToolkitIsTurnedToFirstItem()
            {
                Assert.Same(this.target.Toolkits.First(), this.target.CurrentToolkit);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenProductNameTurnedToDefaultName()
            {
                Assert.Equal("Foo1", this.target.ProductName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCurrentToolkitChanging_ThenPropertyChangedIsRaised()
            {
                var eventRaised = false;

                this.target.CurrentToolkit = this.target.Toolkits.ElementAt(1);
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewProductViewModel>.GetProperty(x => x.CurrentToolkit).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.CurrentToolkit = this.target.Toolkits.First();

                Assert.True(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCurrentToolkitChangingToTheSameValue_ThenPropertyChangedIsNotRaised()
            {
                var eventRaised = false;

                this.target.CurrentToolkit = this.target.Toolkits.First();
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewProductViewModel>.GetProperty(x => x.CurrentToolkit).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.CurrentToolkit = this.target.Toolkits.First();

                Assert.False(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductNameChanging_ThenPropertyChangedIsRaised()
            {
                var eventRaised = false;

                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewProductViewModel>.GetProperty(x => x.ProductName).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.ProductName = "Foo";

                Assert.True(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductNameChangingToTheSameValue_ThenPropertyChangedIsNotRaised()
            {
                var eventRaised = false;

                this.target.ProductName = "Foo";
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewProductViewModel>.GetProperty(x => x.ProductName).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.ProductName = "Foo";

                Assert.False(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCurrentToolkitIsNull_ThenErrorMessageIsAdded()
            {
                this.target.CurrentToolkit = null;

                Assert.True(
                    this.target[Reflector<AddNewProductViewModel>.GetProperty(x => x.CurrentToolkit).Name].Length > 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductNameIsNull_ThenErrorMessageIsAdded()
            {
                this.target.ProductName = null;

                Assert.True(
                    this.target[Reflector<AddNewProductViewModel>.GetProperty(x => x.ProductName).Name].Length > 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductNameIsEmpty_ThenErrorMessageIsAdded()
            {
                this.target.ProductName = string.Empty;

                Assert.True(
                    this.target[Reflector<AddNewProductViewModel>.GetProperty(x => x.ProductName).Name].Length > 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCurrentToolkitIsNull_ThenSelectToolkitCommandCanNotExecute()
            {
                this.target.CurrentToolkit = null;
                this.target.ProductName = "Foo";

                Assert.False(this.target.SelectToolkitCommand.CanExecute(new Mock<IDialogWindow>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProductNameIsNullOrEmpty_ThenSelectToolkitCommandCanNotExecute()
            {
                this.target.CurrentToolkit = this.target.Toolkits.First();
                this.target.ProductName = null;

                Assert.False(this.target.SelectToolkitCommand.CanExecute(new Mock<IDialogWindow>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDataAreValid_ThenSelectToolkitCommandCanExecute()
            {
                this.target.CurrentToolkit = this.target.Toolkits.First();
                this.target.ProductName = "foo";

                Assert.True(this.target.SelectToolkitCommand.CanExecute(new Mock<IDialogWindow>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDataAreValidAndSelectToolkitCommandExecuting_ThenDialogIsClosed()
            {
                var dialog = new Mock<IDialogWindow>();

                this.target.CurrentToolkit = this.target.Toolkits.First();
                this.target.ProductName = "foo";

                this.target.SelectToolkitCommand.Execute(dialog.Object);

                dialog.VerifySet(d => d.DialogResult = true);
                dialog.Verify(d => d.Close());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSelectingANewProduct_ThenProductNameIsChanged()
            {
                this.target.CurrentToolkit = this.target.Toolkits.First();

                Assert.Equal("Foo1", this.target.ProductName);

                this.target.CurrentToolkit = this.target.Toolkits.ElementAt(1);

                Assert.Equal("Bar1", this.target.ProductName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSettingProductNameExternallyAndSelectingANewProduct_ThenProductNameIsNotChanged()
            {
                this.target.CurrentToolkit = this.target.Toolkits.First();
                this.target.ProductName = "Sample";
                this.target.CurrentToolkit = this.target.Toolkits.ElementAt(1);

                Assert.Equal("Sample", this.target.ProductName);
            }
        }

        [TestClass]
        public class GivenAProductStateWithTwoProducts
        {
            private AddNewProductViewModel target;
            private Mock<IPatternManager> patternManager;
            private Mock<IUserMessageService> userMessageService;

            [TestInitialize]
            public void Initialize()
            {
                var products = new List<IProduct> { Mocks.Of<IProduct>().First(p => p.InstanceName == "Foo1"), Mocks.Of<IProduct>().First(p => p.InstanceName == "Foo2") };

                var factories = new[] { GetInstalledToolkit("Foo"), GetInstalledToolkit("Bar") };

                this.patternManager = new Mock<IPatternManager>();
                this.patternManager.Setup(pm => pm.InstalledToolkits).Returns(factories);
                this.patternManager.Setup(pm => pm.CreateProduct(It.IsAny<IInstalledToolkitInfo>(), It.IsAny<string>(), true))
                    .Callback<IInstalledToolkitInfo, string, bool>((f, n, r) => products.Add(Mocks.Of<IProduct>().First(p => p.InstanceName == n)));
                this.patternManager.Setup(pm => pm.Products)
                    .Returns(products);

                this.userMessageService = new Mock<IUserMessageService>();

                this.target = new AddNewProductViewModel(this.patternManager.Object, this.userMessageService.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializingAndThereIsAProductWithTheSameName_ThenIncrementorIsUsedWithDefaultName()
            {
                Assert.Equal("Foo3", this.target.ProductName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAcceptingDialogAndProductNameExistsInPatternManager_ThenErrorMessageIsShownAndDialogIsNotClosed()
            {
                var dialog = new Mock<IDialogWindow>();

                this.target.ProductName = "Foo1";

                this.target.SelectToolkitCommand.Execute(dialog.Object);

                dialog.Verify(d => d.Close(), Times.Never());
                this.userMessageService.Verify(um => um.ShowError(It.IsAny<string>()), Times.Once());
            }
        }

        private static IInstalledToolkitInfo GetInstalledToolkit(string productName)
        {
            var toolkit = new Mock<IInstalledToolkitInfo>();
            toolkit.Setup(f => f.Schema.Pattern.DisplayName).Returns(productName);
            return toolkit.Object;
        }
    }
}