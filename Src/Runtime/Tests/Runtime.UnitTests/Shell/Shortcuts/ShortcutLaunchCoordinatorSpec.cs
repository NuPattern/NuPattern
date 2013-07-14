using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Shell.Shortcuts;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UnitTests.Shell.Shortcuts
{
    public class ShortcutLaunchCoordinatorSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private Mock<IServiceProvider> serviceProvider;
            private Mock<IShortcutLaunchService> launchService;
            private Mock<IUserMessageService> msgService;
            private Mock<IShortcutPersistenceHandler> fileHandler;

            [TestInitialize]
            public void Initialize()
            {
                this.fileHandler = new Mock<IShortcutPersistenceHandler>();
                this.launchService = new Mock<IShortcutLaunchService>();
                this.serviceProvider = new Mock<IServiceProvider>();
                this.msgService = new Mock<IUserMessageService>();
                this.serviceProvider.Setup(sp => sp.GetService(typeof(IUserMessageService))).Returns(this.msgService.Object);
                this.serviceProvider.Setup(sp => sp.GetService(typeof(IShortcutLaunchService))).Returns(this.launchService.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFileHandlerThrowsFileNotFoundException_ThenDisplaysErrorAndReturnsFalse()
            {
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Throws<FileNotFoundException>();

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                this.msgService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenFileHandlerThrowsShortcutFormatException_ThenDisplaysErrorAndReturnsFalse()
            {
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Throws<ShortcutFileFormatException>();

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                this.msgService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenProviderNotRegistered_ThenDisplaysErrorAndReturnsFalse()
            {
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.IsTypeRegistered(It.IsAny<string>())).Returns(false);

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                this.msgService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenShortcutNotResolved_ThenDisplaysErrorAndReturnsFalse()
            {
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.IsTypeRegistered(It.IsAny<string>())).Returns(true);
                this.launchService.Setup(ls => ls.ResolveShortcut(It.IsAny<IShortcut>())).Returns((IShortcut)null);

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                this.msgService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.False(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenShortcutResolvedAndNotUpdated_ThenReturnsTrue()
            {
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.IsTypeRegistered(It.IsAny<string>())).Returns(true);
                this.launchService.Setup(ls => ls.ResolveShortcut(It.IsAny<IShortcut>())).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.Execute(It.IsAny<IShortcut>(), null)).Returns((IShortcut)null);

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                fileHandler.Verify(ms => ms.WriteShortcut(It.IsAny<IShortcut>()), Times.Never());
                Assert.True(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenShortcutResolvedAndUpdated_ThenShortcutSavedAndReturnsTrue()
            {
                var updatedShortcut = Mock.Of<IShortcut>();
                this.fileHandler.Setup(fh => fh.ReadShortcut()).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.IsTypeRegistered(It.IsAny<string>())).Returns(true);
                this.launchService.Setup(ls => ls.ResolveShortcut(It.IsAny<IShortcut>())).Returns(Mock.Of<IShortcut>());
                this.launchService.Setup(ls => ls.Execute(It.IsAny<IShortcut>(), null)).Returns(updatedShortcut);

                var result = ShortcutLaunchCoordinator.LaunchShortcut(this.serviceProvider.Object, fileHandler.Object);

                fileHandler.Verify(ms => ms.WriteShortcut(updatedShortcut), Times.Once());
                Assert.True(result);
            }
        }
    }
}
