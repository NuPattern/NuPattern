using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Guidance.Properties;
using NuPattern.Runtime.Guidance.ShortcutProviders;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UnitTests.Guidance.ShortcutProviders
{
    public class GuidanceShortcutProviderSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAShortcut
        {
            private GuidanceShortcutProvider provider;
            private Mock<IUserMessageService> messageService;
            private Mock<IGuidanceProvider> guidanceProvider;
            private Mock<IShortcut> shortcut;

            [TestInitialize]
            public void Initialize()
            {
                this.shortcut = new Mock<IShortcut>();
                this.guidanceProvider = new Mock<IGuidanceProvider>();
                this.messageService = new Mock<IUserMessageService>();
                this.provider = new GuidanceShortcutProvider
                {
                    MessageService = this.messageService.Object,
                    GuidanceProvider = guidanceProvider.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenTypeReturnsGuidance()
            {
                Assert.Equal("guidance", provider.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNoParameters_ThenDisplaysError()
            {
                provider.Execute((new GuidanceShortcut(new Mock<IShortcut>().Object)));

                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(Resources.GuidanceShortcutProvider_InvalidParameters), Times.Once());
            }
        }

        [TestClass]
        public class GivenARegisteredShortcutWithAnInstanceName
        {
            private GuidanceShortcutProvider provider;
            private Mock<IUserMessageService> messageService;
            private Mock<IGuidanceProvider> guidanceProvider;
            private Mock<IShortcut> shortcut;
            private Dictionary<string, string> parameters;

            [TestInitialize]
            public void Initialize()
            {
                this.shortcut = new Mock<IShortcut>();
                this.parameters = new Dictionary<string, string>
                    {
                        {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                        {GuidanceShortcut.InstanceNameParameterName, "bar"},
                    };
                shortcut.Setup(s => s.Parameters).Returns(this.parameters);
                this.guidanceProvider = new Mock<IGuidanceProvider>();
                this.messageService = new Mock<IUserMessageService>();
                this.provider = new GuidanceShortcutProvider
                {
                    MessageService = this.messageService.Object,
                    GuidanceProvider = guidanceProvider.Object,
                };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedRegisteredExistingInstance_ThenCreatesNewUniqueNamedInstance()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.GetUniqueInstanceName("bar")).Returns("bar1");

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "bar1"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithUnregisteredExistingInstance_ThenActivatesExistingInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(true);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                guidanceProvider.Verify(gp => gp.ActivateInstance("bar"), Times.Once());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithRegisteredExistingInstance_ThenActivatesExistingInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(true);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance("bar"), Times.Once());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithRegisteredNotExistingInstance_ThenCreateNewInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "bar"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedRegisteredNotExistingInstance_ThenCreateNewInstance()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "bar"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedUnregistered_ThenDisplaysError()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "bar")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(Resources.GuidanceShortcutProvider_NotRegistered.FormatWith("foo", "bar")), Times.Once());
            }
        }

        [TestClass]
        public class GivenARegisteredShortcutWithNoInstanceName
        {
            private GuidanceShortcutProvider provider;
            private Mock<IUserMessageService> messageService;
            private Mock<IGuidanceProvider> guidanceProvider;
            private Mock<IShortcut> shortcut;
            private Dictionary<string, string> parameters;

            [TestInitialize]
            public void Initialize()
            {
                this.shortcut = new Mock<IShortcut>();
                this.parameters = new Dictionary<string, string>
                    {
                        {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                    };
                shortcut.Setup(s => s.Parameters).Returns(this.parameters);
                this.guidanceProvider = new Mock<IGuidanceProvider>();
                this.messageService = new Mock<IUserMessageService>();
                this.provider = new GuidanceShortcutProvider
                    {
                        MessageService = this.messageService.Object,
                        GuidanceProvider = guidanceProvider.Object,
                    };
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedRegisteredExistingDefaultInstance_ThenCreatesNewUniqueDefaultNamedInstance()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.GetUniqueInstanceName("default")).Returns("default1");

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "default1"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithUnregisteredExistingDefaultInstance_ThenActivatesExistingDefaultInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(true);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                guidanceProvider.Verify(gp => gp.ActivateInstance("default"), Times.Once());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithRegisteredExistingDefaultInstance_ThenActivatesExistingDefaultInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(true);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance("default"), Times.Once());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithRegisteredNotExistingInstance_ThenCreateNewDefaultInstance()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "default"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedRegisteredNotExistingInstance_ThenCreateNewDefaultInstance()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(true);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance("foo", "default"), Times.Once());
                this.messageService.Verify(ums => ums.ShowError(It.IsAny<string>()), Times.Never());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedUnregisteredExistingDefaultInstance_ThenDisplaysError()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(true);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(Resources.GuidanceShortcutProvider_NotRegistered.FormatWith("foo", "bar")), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithForcedUnregistered_ThenDisplaysError()
            {
                this.parameters.Add(GuidanceShortcut.AlwaysCreateParameterName, "true");
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(Resources.GuidanceShortcutProvider_NotRegistered.FormatWith("foo", "bar")), Times.Once());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteUnregistered_ThenDisplaysError()
            {
                this.guidanceProvider.Setup(gp => gp.IsRegistered("foo")).Returns(false);
                this.guidanceProvider.Setup(gp => gp.GetDefaultInstanceName("foo")).Returns("default");
                this.guidanceProvider.Setup(gp => gp.InstanceExists("foo", "default")).Returns(false);

                provider.Execute((new GuidanceShortcut(shortcut.Object)));

                this.guidanceProvider.Verify(gp => gp.ActivateInstance(It.IsAny<string>()), Times.Never());
                this.guidanceProvider.Verify(gp => gp.CreateInstance(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ums => ums.ShowError(Resources.GuidanceShortcutProvider_NotRegisteredNoInstance.FormatWith("foo")), Times.Once());
            }
        }
    }
}
