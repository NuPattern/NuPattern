using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Guidance.ShortcutProviders;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.UnitTests.Guidance.ShortcutProviders
{
    public class GuidanceShortcutSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenAShortcutWithNoParameters
        {
            private Mock<IShortcut> shortcut;
            private GuidanceShortcut guidanceShortcut;

            [TestInitialize]
            public void Initialize()
            {
                this.shortcut = new Mock<IShortcut>();
                this.shortcut.Setup(sc => sc.Type).Returns("foo");
                this.shortcut.Setup(sc => sc.Description).Returns("bar");
                this.shortcut.Setup(sc => sc.Parameters).Returns(new Dictionary<string, string> { { "foo", "bar" } });

                this.guidanceShortcut = new GuidanceShortcut(this.shortcut.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenTypeIsSet()
            {
                Assert.Equal(GuidanceShortcut.ShortcutType, this.guidanceShortcut.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDescriptionIsSet()
            {
                Assert.Equal("bar", this.guidanceShortcut.Description);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenParametersAreSet()
            {
                Assert.Equal(1, this.guidanceShortcut.Parameters.Count);
                Assert.Equal("foo", this.guidanceShortcut.Parameters.First().Key);
                Assert.Equal("bar", this.guidanceShortcut.Parameters.First().Value);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCommandTypeUndefined()
            {
                Assert.Equal(GuidanceShortcutCommandType.Undefined, this.guidanceShortcut.CommandType);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDefaultNameNull()
            {
                Assert.Null(this.guidanceShortcut.DefaultName);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenInstanceNameIdNull()
            {
                Assert.Null(this.guidanceShortcut.InstanceName);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenExtensionIdNull()
            {
                Assert.Null(this.guidanceShortcut.GuidanceExtensionId);
            }
        }

        [TestClass]
        public class GivenAShortcutWithGuidanceParameters
        {
            private Mock<IShortcut> shortcut;
            private GuidanceShortcut guidanceShortcut;

            [TestInitialize]
            public void Initialize()
            {
                this.shortcut = new Mock<IShortcut>();
                this.shortcut.Setup(sc => sc.Type).Returns("foo");
                this.shortcut.Setup(sc => sc.Description).Returns("bar");
                this.shortcut.Setup(sc => sc.Parameters).Returns(new Dictionary<string, string>
                    {
                        { GuidanceShortcut.CommandTypeParameterName, GuidanceShortcutCommandType.Activation.ToString().ToLowerInvariant() },
                        { GuidanceShortcut.DefaultNameParameterName, "bar" },
                        { GuidanceShortcut.InstanceNameParameterName, "bar2" },
                        { GuidanceShortcut.ExtensionIdParameterName, "bar3" },
                    });

                this.guidanceShortcut = new GuidanceShortcut(this.shortcut.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenTypeIsSet()
            {
                Assert.Equal(GuidanceShortcut.ShortcutType, this.guidanceShortcut.Type);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDescriptionIsSet()
            {
                Assert.Equal("bar", this.guidanceShortcut.Description);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenParametersAreSet()
            {
                Assert.Equal(4, this.guidanceShortcut.Parameters.Count);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenCommandTypeAssigned()
            {
                Assert.Equal(GuidanceShortcutCommandType.Activation, this.guidanceShortcut.CommandType);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenDefaultNameAssigned()
            {
                Assert.Equal("bar", this.guidanceShortcut.DefaultName);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenInstanceNameAssigned()
            {
                Assert.Equal("bar2", this.guidanceShortcut.InstanceName);
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenExtensionIdAssigned()
            {
                Assert.Equal("bar3", this.guidanceShortcut.GuidanceExtensionId);
            }
        }

    }
}
