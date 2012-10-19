using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.IntegrationTests
{
    public class SettingsManagerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenASettingsManager
        {
            private ISettingsManager manager;

            [TestInitialize]
            public void Initialize()
            {
                var components = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();

                var manager = new ShellSettingsManager(VsIdeTestHostContext.ServiceProvider);
                var store = manager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (store.CollectionExists(Microsoft.VisualStudio.Patterning.Runtime.Constants.RegistrySettingsKeyName))
                {
                    store.DeleteCollection(Microsoft.VisualStudio.Patterning.Runtime.Constants.RegistrySettingsKeyName);
                }

                this.manager = components.GetService<ISettingsManager>();
            }

            [HostType("VS IDE")]
            [TestMethod]
            public void WhenReadingTracesFirstTime_ThenReturnsEmptySourceSettings()
            {
                var setting = this.manager.Read();
                Assert.Equal(0, setting.Tracing.TraceSources.Count);
            }

            [HostType("VS IDE")]
            [TestMethod]
            public void WhenSettingRootSourceLevel_ThenCanReadIt()
            {
                var setting = this.manager.Read();
                setting.Tracing.RootSourceLevel = SourceLevels.Verbose;

                this.manager.Save(setting);

                setting = this.manager.Read();

                Assert.True(setting.Tracing.RootSourceLevel == SourceLevels.Verbose);
            }

            [HostType("VS IDE")]
            [TestMethod]
            public void WhenSettingTraceSetting_ThenCanReadIt()
            {
                var setting = this.manager.Read();
                setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Verbose));

                this.manager.Save(setting);

                setting = this.manager.Read();

                Assert.True(setting.Tracing.TraceSources.Any(x => x.SourceName == "Foo"));
            }

            [HostType("VS IDE")]
            [TestMethod]
            public void WhenSettingIsSaved_ThenSettingsChangedPublishesOldAndNewValue()
            {
                RuntimeSettings oldValue = null;
                RuntimeSettings newValue = null;

                this.manager.SettingsChanged += (sender, args) => { oldValue = args.OldValue; newValue = args.NewValue; };

                var setting = this.manager.Read();
                setting.Tracing.TraceSources.Add(new TraceSourceSetting("Foo", SourceLevels.Verbose));

                this.manager.Save(setting);

                Assert.NotNull(oldValue);
                Assert.NotNull(newValue);

                Assert.False(oldValue.Tracing.TraceSources.Any(x => x.SourceName == "Foo"));
                Assert.True(newValue.Tracing.TraceSources.Any(x => x.SourceName == "Foo"));
            }
        }
    }
}