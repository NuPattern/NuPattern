using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Guidance;
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
            private Mock<IGuidanceManager> guidanceManager;
            private Mock<IUserMessageService> messageService;

            [TestInitialize]
            public void Initialize()
            {
                this.messageService = new Mock<IUserMessageService>();
                this.guidanceManager = new Mock<IGuidanceManager>();
                this.provider = new GuidanceShortcutProvider();
                this.provider.GuidanceManager = this.guidanceManager.Object;
                this.provider.MessageService = this.messageService.Object;
                this.provider.ServiceProvider = Mock.Of<IServiceProvider>();
            }

            [TestMethod, TestCategory("Unit")]
            public void TheReturnsType()
            {
                Assert.Equal(GuidanceShortcut.ShortcutType, provider.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolveWithNullShortcut_TheThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    provider.ResolveShortcut(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolve_TheReturnsAGuidanceShortcut()
            {
                var result = provider.ResolveShortcut(Mock.Of<IShortcut>(sc => sc.Type == GuidanceShortcut.ShortcutType));

                Assert.NotNull(result);
                Assert.Equal(GuidanceShortcut.ShortcutType, result.Type);
                Assert.True(result.GetType() == typeof(GuidanceShortcut));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithNullShortcut_TheThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    provider.Execute(null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecute_ThenReturnsNull()
            {
                var result = provider.Execute(new GuidanceShortcut((Mock.Of<IShortcut>(sc => sc.Type == GuidanceShortcut.ShortcutType))));

                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithUndefined_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, string.Empty},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Undefined.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationAndNoData_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, string.Empty},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithInstantiationAndNoData_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, string.Empty},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Instantiation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationAndInstanceNotFound_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, string.Empty},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, "foo"},
                        });
                this.guidanceManager.Setup(gm => gm.InstantiatedGuidanceExtensions).Returns(Enumerable.Empty<IGuidanceExtension>());

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationAndExtensionNotFound_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });
                this.guidanceManager.Setup(gm => gm.InstalledGuidanceExtensions).Returns(Enumerable.Empty<IGuidanceExtensionRegistration>());

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithInstantiationAndExtensionNotFound_ThenDisplayError()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Instantiation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });
                this.guidanceManager.Setup(gm => gm.InstalledGuidanceExtensions).Returns(Enumerable.Empty<IGuidanceExtensionRegistration>());

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.Verify(gm => gm.ActiveGuidanceExtension, Times.Never());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Once());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationOfNamedInstance_ThenActivates()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, string.Empty},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, "foo"},
                        });
                var instance = Mock.Of<IGuidanceExtension>(ge => ge.InstanceName == "foo");
                this.guidanceManager.Setup(gm => gm.InstantiatedGuidanceExtensions).Returns(new[] { instance });

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.VerifySet(gm => gm.ActiveGuidanceExtension = instance, Times.Once());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Never());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationOfNotExistingSharedInstance_ThenInstantiatesActivates()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });
                var instance = Mock.Of<IGuidanceExtension>(ge => ge.InstanceName == "bar");
                this.guidanceManager.Setup(gm => gm.InstalledGuidanceExtensions).Returns(new[] { Mock.Of<IGuidanceExtensionRegistration>(ger => ger.ExtensionId == "foo") });
                this.guidanceManager.Setup(gm => gm.Instantiate("foo", It.IsAny<string>())).Returns(instance);

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.VerifySet(gm => gm.ActiveGuidanceExtension = instance, Times.Once());
                this.guidanceManager.Verify(gm => gm.Instantiate("foo", It.IsAny<string>()), Times.Once());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Never());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithActivationOfExistingSharedInstance_ThenInstantiatesActivates()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, string.Empty},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });
                var instance = Mock.Of<IGuidanceExtension>(ge => ge.InstanceName == "bar" && ge.ExtensionId == "foo");
                this.guidanceManager.Setup(gm => gm.InstalledGuidanceExtensions).Returns(new[] { Mock.Of<IGuidanceExtensionRegistration>(ger => ger.ExtensionId == "foo") });
                this.guidanceManager.Setup(gm => gm.InstantiatedGuidanceExtensions).Returns(new[] { instance });

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.VerifySet(gm => gm.ActiveGuidanceExtension = instance, Times.Once());
                this.guidanceManager.Verify(gm => gm.Instantiate(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Never());
                Assert.Null(result);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecuteWithInstantiationNewInstance_ThenInstantiatesActivates()
            {
                var shortcut = Mock.Of<IShortcut>(
                    sc => sc.Type == GuidanceShortcut.ShortcutType
                    && sc.Parameters == new Dictionary<string, string>
                        {
                            {GuidanceShortcut.ExtensionIdParameterName, "foo"},
                            {GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Instantiation.ToString().ToLowerInvariant()},
                            {GuidanceShortcut.DefaultNameParameterName, "bar"},
                            {GuidanceShortcut.InstanceNameParameterName, string.Empty},
                        });
                var instance = Mock.Of<IGuidanceExtension>(ge => ge.InstanceName == "bar" && ge.ExtensionId == "foo");
                this.guidanceManager.Setup(gm => gm.InstalledGuidanceExtensions).Returns(new[] { Mock.Of<IGuidanceExtensionRegistration>(ger => ger.ExtensionId == "foo" && ger.DefaultName == "foobar") });
                this.guidanceManager.Setup(gm => gm.Instantiate("foo", It.IsAny<string>())).Returns(instance);

                var result = provider.Execute(new GuidanceShortcut(shortcut));

                this.guidanceManager.VerifySet(gm => gm.ActiveGuidanceExtension = instance, Times.Once());
                this.guidanceManager.Verify(gm => gm.Instantiate("foo", "bar"), Times.Once());
                this.messageService.Verify(ms => ms.ShowError(It.IsAny<string>()), Times.Never());
                Assert.Null(result);
            }
        }
    }
}
