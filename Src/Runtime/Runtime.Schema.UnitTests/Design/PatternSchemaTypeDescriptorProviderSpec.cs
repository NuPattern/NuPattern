using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;
using NuPattern.VisualStudio.Shell;

namespace NuPattern.Runtime.Schema.UnitTests
{
    public class PatternSchemaTypeDescriptorProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test cleanup")]
        public class GivenNoContext
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private PatternSchema patternSchema;
            private PatternSchemaTypeDescriptorProvider.PatternSchemaTypeDescriptor descriptor;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    this.patternSchema = patternModel.CreatePatternSchema() as PatternSchema;
                });

                this.descriptor = new PatternSchemaTypeDescriptorProvider.PatternSchemaTypeDescriptor(this.patternSchema);

                var info = new Mock<IInstalledToolkitInfo>();
                var patternManager = new Mock<IPatternManager>();
                var componentModel = new Mock<IComponentModel>();
                var serviceProvider = Mock.Get(this.store.ServiceProvider);

                info.Setup(i => i.Schema.FindAll<IExtensionPointSchema>()).Returns(new[] { new Mock<IExtensionPointSchema>().Object });
                patternManager.Setup(m => m.InstalledToolkits).Returns(new[] { info.Object });
                componentModel.Setup(c => c.GetService<IPatternManager>()).Returns(patternManager.Object);
                componentModel.Setup(c => c.GetService<IUserMessageService>()).Returns(new Mock<IUserMessageService>().Object);
                serviceProvider.Setup(sp => sp.GetService(typeof(SComponentModel))).Returns(componentModel.Object);
            }

            [TestCleanup]
            public void Cleanup()
            {
                this.store.Dispose();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingExtensionPoints_ThenReturnsExtensionPoints()
            {
                Assert.NotNull(this.descriptor.ExtensionPoints);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingMessageService_ThenReturnsMessageService()
            {
                Assert.NotNull(this.descriptor.MessageService);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenGettingProperties_ThenReturnsImplementedExtensionPointsRawProperty()
            {
                Assert.True(
                    this.descriptor.GetProperties().Cast<PropertyDescriptor>()
                        .Any(p => p.Name == "ImplementedExtensionPointsRaw"));
            }
        }
    }
}