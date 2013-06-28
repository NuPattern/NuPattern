using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.ComponentModel.Composition;
using NuPattern.Runtime.Composition;
using NuPattern.Runtime.IntegrationTests.SampleVsix;

namespace NuPattern.Runtime.IntegrationTests
{
    [TestClass]
    public class EventsIntegrationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenExportedSourcePublisherAndSubscriber
        {
            private IComponentModel globalContainer;
            private INuPatternCompositionService compositionService;

            [TestInitialize]
            public void Initialize()
            {
                this.globalContainer = VsIdeTestHostContext.ServiceProvider.GetService<SComponentModel, IComponentModel>();
                this.compositionService = VsIdeTestHostContext.ServiceProvider.GetService<INuPatternCompositionService>();
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenSourceRaised_ThenSubscriberNotified()
            {
                //// MefLogger.Log();

                var source = this.globalContainer.GetService<EventSource>();
                var subscriber = this.globalContainer.GetService<EventSubscriber>();

                Assert.Equal(0, subscriber.ChangedProperties.Count);

                source.RaisePropertyChanged("Foo");

                Assert.Contains("Foo", subscriber.ChangedProperties);

                source.RaisePropertyChanged("Bar");

                Assert.Contains("Bar", subscriber.ChangedProperties);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenRetrievingEventExportMetadata_ThenContainsDescription()
            {
                var info = this.compositionService
                    .GetExports<IObservableEvent, IComponentMetadata>()
                    .Where(x => x.Metadata.ExportingType == typeof(EventPublisher))
                    .First();

                Assert.Equal("Raised when the source property changes", info.Metadata.Description);
            }
        }
    }
}
