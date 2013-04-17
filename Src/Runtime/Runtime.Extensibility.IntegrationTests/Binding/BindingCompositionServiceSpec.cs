using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using Moq;
using NuPattern.Runtime.Bindings;
using NuPattern.Runtime.Composition;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.IntegrationTests.Binding
{
    [TestClass]
    public class BindingCompositionServiceSpec : IntegrationTest
    {
        internal static readonly IAssertion Assert = new Assertion();
        private IServiceProvider serviceProvider;

        [TestInitialize]
        public void InitializeContext()
        {
            this.serviceProvider = VsIdeTestHostContext.ServiceProvider;
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingISolution_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var solution = composition.GetExportedValue<ISolution>();

            Assert.NotNull(solution);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingIBindingFactory_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var result = composition.GetExportedValue<IBindingFactory>();

            Assert.NotNull(result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingEvents_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var result = composition.GetExportedValues<IObservableEvent>();

            Assert.NotNull(result);
            Assert.True(result.Count() > 0);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingOtherVSExports_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var result = composition.GetExportedValues<ISmartTagSourceProvider>();

            Assert.NotNull(result);
            Assert.True(result.Count() > 0);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingSVsServiceProvider_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var result = (IServiceProvider)composition.GetExportedValue<SVsServiceProvider>();

            Assert.NotNull(result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingCompositionService_ThenReturnsSelf()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var service = composition.GetExportedValue<IFeatureCompositionService>();

            Assert.Same(composition, service);
        }

        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingSVsServiceProviderImport_ThenSucceeds()
        {
            var components = new Mock<IComponentModel> { DefaultValue = DefaultValue.Mock };
            components.As<SComponentModel>();
            var services = new Mock<IServiceProvider> { DefaultValue = DefaultValue.Mock };
            services.As<SVsServiceProvider>();

            services.Setup(x => x.GetService(typeof(SComponentModel))).Returns(components.Object);

            var composition = new BindingCompositionService(services.Object);

            var result = new ServicedClass();

            composition.SatisfyImportsOnce(result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingSVsServiceProviderImportInVS_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();

            var result = new ServicedClass();

            composition.SatisfyImportsOnce(result);
        }

        [HostType("VS IDE")]
        [TestMethod, TestCategory("Integration")]
        public void WhenResolvingDynamicBindingContxtImport_ThenSucceeds()
        {
            var composition = this.serviceProvider.GetService<IBindingCompositionService>();
            var context = composition.CreateDynamicContext();

            var result = new ServicedClass();

            context.CompositionService.SatisfyImportsOnce(result);

            Assert.NotNull(result.Context);
        }

        public class ServicedClass
        {
            [Import(typeof(SVsServiceProvider))]
            public IServiceProvider Services { get; set; }

            [Import(AllowDefault = true)]
            public IDynamicBindingContext Context { get; set; }
        }
    }
}
