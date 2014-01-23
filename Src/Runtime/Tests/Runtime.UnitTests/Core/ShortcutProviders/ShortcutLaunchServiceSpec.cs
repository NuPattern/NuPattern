using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.ShortcutProviders;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.UnitTests.Core.ShortcutProviders
{
    public class ShortcutLaunchServiceSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoProviders
        {
            private ShortcutLaunchService service;

            [TestInitialize]
            public void Initialize()
            {
                this.service = new ShortcutLaunchService(new List<IShortcutProvider>());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsRegisteredType_ThenReturnsFalse()
            {
                Assert.False(this.service.IsTypeRegistered("test"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolveShortcut_ThenReturnsNull()
            {
                Assert.Null(this.service.ResolveShortcut(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolveShortcutSpecific_ThenReturnsNull()
            {
                Assert.Null(this.service.ResolveShortcut<TestShortcut>(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenExecute_ThenThrows()
            {
                Assert.Throws<NotSupportedException>(() =>
                    this.service.Execute(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCanCreateShortcut_ThenReturnsFalse()
            {
                Assert.False(this.service.CanCreateShortcut(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateShortcut_ThenThrows()
            {
                Assert.Throws<NotSupportedException>(() =>
                    this.service.CreateShortcut(Mock.Of<IShortcut>()));
            }
        }

        [TestClass]
        public class GivenAProvider
        {
            private ShortcutLaunchService service;
            private Mock<IShortcutProvider<TestShortcut>> provider;

            [TestInitialize]
            public void Initialize()
            {
                this.provider = new Mock<IShortcutProvider<TestShortcut>>();
                this.provider.Setup(p => p.Type).Returns("test");
                this.service = new ShortcutLaunchService(new[] { provider.Object });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenIsRegisteredType_ThenReturnsTrue()
            {
                Assert.True(this.service.IsTypeRegistered("test"));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolveShortcut_ThenReturnsTrue()
            {
                var shortcut = new TestShortcut();
                this.provider.Setup(p => p.ResolveShortcut(It.IsAny<IShortcut>())).Returns(shortcut);

                Assert.Equal(shortcut, this.service.ResolveShortcut(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenResolveShortcutSpecific_ThenReturnsShortcut()
            {
                var shortcut = new TestShortcut();
                this.provider.Setup(p => p.ResolveShortcut(It.IsAny<IShortcut>())).Returns(shortcut);

                Assert.Equal(shortcut, this.service.ResolveShortcut<TestShortcut>(Mock.Of<IShortcut>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreateShortcut_ThenReturnsShortcut()
            {
                var shortcut = new TestShortcut();
                this.provider.Setup(p => p.CreateShortcut(shortcut)).Returns(shortcut);

                Assert.Equal(shortcut, this.service.CreateShortcut(shortcut));
            }
        }

        public class TestShortcut : IShortcut
        {
            public string Type
            {
                get { return "test"; }
            }

            public string Description
            {
                get { throw new System.NotImplementedException(); }
            }

            public System.Collections.Generic.IDictionary<string, string> Parameters
            {
                get { throw new System.NotImplementedException(); }
            }
        }
    }
}
