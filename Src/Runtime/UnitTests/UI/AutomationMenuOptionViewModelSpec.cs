using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Settings;
using NuPattern.Runtime.UI.ViewModels;

namespace NuPattern.Runtime.UnitTests.UI
{
    public class AutomationMenuOptionViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAMenuAutomation
        {
            private Mock<IAutomationExtension> automation;
            private Mock<IAutomationMenuCommand> menu;
            private Mock<ICommandStatus> status;
            private AutomationMenuOptionViewModel target;

            [TestInitialize]
            public void Initialize()
            {
                this.automation = new Mock<IAutomationExtension>();
                this.status = this.automation.As<ICommandStatus>();
                this.menu = automation.As<IAutomationMenuCommand>();
                this.menu.Setup(m => m.Text).Returns("Foo");

                this.target = new AutomationMenuOptionViewModel(this.automation.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenImagePresent_ThenImageIsSetAndTypeIsImage()
            {
                var automation = new Mock<IAutomationExtension>();
                var menu = automation.As<IAutomationMenuCommand>();

                menu.Setup(m => m.Text).Returns("Foo");
                menu.Setup(a => a.IconPath).Returns("pack://application,,,/Assembly;component/path/ico.ico");

                var viewModel = new AutomationMenuOptionViewModel(automation.Object);

                Assert.Equal(IconType.Image, viewModel.IconType);
                Assert.NotNull(viewModel.ImagePath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenImageNotPresent_ThenImageIsSetAndTypeIsNone()
            {
                var automation = new Mock<IAutomationExtension>();
                var menu = automation.As<IAutomationMenuCommand>();

                menu.Setup(m => m.Text).Returns("Foo");
                menu.Setup(a => a.IconPath).Returns<string>(null);

                var viewModel = new AutomationMenuOptionViewModel(automation.Object);

                Assert.Equal(IconType.None, viewModel.IconType);
                Assert.Null(viewModel.ImagePath);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCanExecute_ThenQueriesStatusAndReturnsMenuIsEnabled()
            {
                this.menu.Setup(x => x.Enabled).Returns(true);

                Assert.True(this.target.Command.CanExecute(null));

                this.menu.Setup(x => x.Enabled).Returns(false);

                Assert.False(this.target.Command.CanExecute(null));

                this.status.Verify(x => x.QueryStatus(this.menu.Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecute_ThenInvokesExtension()
            {
                this.target.Command.Execute(null);

                this.automation.Verify(x => x.Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAutomationNotImplementingStatus_ThenCanExecuteIsStillTrue()
            {
                var automation = new Mock<IAutomationExtension>();
                var menu = automation.As<IAutomationMenuCommand>();
                menu.Setup(m => m.Text).Returns("Foo");
                menu.Setup(x => x.Enabled).Returns(true);

                this.target = new AutomationMenuOptionViewModel(automation.Object);
                Assert.True(this.target.Command.CanExecute(null));
            }
        }
    }
}