using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Runtime.Shell.Shortcuts;

namespace NuPattern.Runtime.IntegrationTests.Shell.Shortcuts
{
    [TestClass]
    public class ShortcutFileHandlerSpec
    {
        private static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [DeploymentItem(@"Runtime.IntegrationTests.Content\Shortcuts", @"Runtime.IntegrationTests.Content\Shortcuts")]
        public class GivenAShortcutFile : IntegrationTest
        {
            [TestMethod, TestCategory("Integration")]
            public void WhenShortcutFilePathEmpty_ThenReadShortcutThrows()
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new ShortcutFileHandler(string.Empty).ReadShortcut());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShortcutFileNotExist_ThenReadShortcutThrows()
            {
                Assert.Throws<FileNotFoundException>(() => new ShortcutFileHandler(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\foo.shortcut")).ReadShortcut());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShortcutFileEmpty_ThenReadShortcutThrows()
            {
                Assert.Throws<ShortcutFileFormatException>(() => new ShortcutFileHandler(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\EmptyShortcut.shortcut")).ReadShortcut());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShortcutFileInvalid_ThenReadShortcutThrows()
            {
                Assert.Throws<ShortcutFileFormatException>(() => new ShortcutFileHandler(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\InvalidShortcut.shortcut")).ReadShortcut());
            }

            [TestMethod, TestCategory("Integration")]
            public void WhenShortcutFileHasShortcut_ThenReadShortcutReturns()
            {
                var shortcut = new ShortcutFileHandler(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\ValidShortcut.shortcut")).ReadShortcut();

                Assert.NotNull(shortcut);
                Assert.Equal("custom", shortcut.Type);
                Assert.Equal("somedescription", shortcut.Description);
                Assert.Equal(2, shortcut.Parameters.Count);
                Assert.Equal("foo", shortcut.Parameters.First().Key);
                Assert.Equal("bar", shortcut.Parameters.First().Value);
            }
        }

        [TestClass]
        [DeploymentItem(@"Runtime.IntegrationTests.Content\Shortcuts", @"Runtime.IntegrationTests.Content\Shortcuts")]
        public class GivenNoShortcutFile : IntegrationTest
        {
            [TestMethod, TestCategory("Integration")]
            public void WhenValidShortcutWrittenAndRead_ThenReadShortcutReturns()
            {
                var handler = new ShortcutFileHandler(this.PathTo(@"Runtime.IntegrationTests.Content\Shortcuts\NewShortcut.shortcut"));
                var shortcut = new Shortcut
                {
                    Type = "custom",
                    Description = "somedescription",
                };
                shortcut.Parameters.Add("foo", "bar");

                handler.WriteShortcut(shortcut);

                Assert.True(File.Exists(handler.FilePath));

                var loadedShortcut = handler.ReadShortcut();

                Assert.NotNull(loadedShortcut);
                Assert.Equal("custom", loadedShortcut.Type);
                Assert.Equal("somedescription", loadedShortcut.Description);
                Assert.Equal(1, loadedShortcut.Parameters.Count);
                Assert.Equal("foo", loadedShortcut.Parameters.First().Key);
                Assert.Equal("bar", loadedShortcut.Parameters.First().Value);
            }
        }
    }
}
