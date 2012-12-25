using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Store.UnitTests
{
    public class ProductElementAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAContainerWithNoAutomationSettings
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private ProductElement element;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Product>();
                    tx.Commit();
                }
            }

            [TestCleanup]
            public void CleanUp()
            {
                if (!this.store.Store.Disposed)
                {
                    this.store.Dispose();
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenHasEmptyAutomations()
            {
                Assert.Equal(0, this.element.AutomationExtensions.Count);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "TestCleanup")]
        [TestClass]
        public class GivenAContainerWithConfiguredAutomationSettings
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private Mock<IAutomationSettings> extensionSettings;
            private Mock<IAutomationExtension> automationExtension;
            private ProductElement element;

            [TestInitialize]
            public void Initialize()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();
                Mock.Get(this.store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(SComponentModel)))
                    .Returns(new Mock<IComponentModel>
                    {
                        DefaultValue = DefaultValue.Mock
                    }
                    .Object);
                Mock.Get(this.store.ServiceProvider)
                    .Setup(x => x.GetService(typeof(IBindingCompositionService)))
                    .Returns(new Mock<IBindingCompositionService>
                    {
                        DefaultValue = DefaultValue.Mock
                    }
                    .Object);

                this.automationExtension = new Mock<IAutomationExtension>();
                this.automationExtension.As<IDisposable>();
                this.automationExtension.As<ISupportInitialize>();
                this.extensionSettings = new Mock<IAutomationSettings>();
                this.extensionSettings
                    .Setup(x => x.CreateAutomation(It.IsAny<IProductElement>()))
                    .Returns(this.automationExtension.Object);

                var settingsContainer = new Mock<IPatternElementInfo>();
                var settingsInfo = new Mock<IAutomationSettingsInfo>();

                settingsContainer
                    .Setup(x => x.AutomationSettings)
                    .Returns(new[] { settingsInfo.Object });

                settingsInfo
                    .Setup(info => info.As<IAutomationSettings>())
                    .Returns(this.extensionSettings.Object);

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Product>();
                    this.element.Info = settingsContainer.Object;
                    tx.Commit();
                }
            }

            [TestCleanup]
            public void CleanUp()
            {
                if (!this.store.Store.Disposed)
                {
                    this.store.Dispose();
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenElementIsAdded_ThenCreatesRuntimeAutomation()
            {
                this.extensionSettings.Verify(x => x.CreateAutomation(It.IsAny<IProductElement>()));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAutomationSupportsInitialize_ThenInvokesBeginEndInit()
            {
                this.automationExtension.As<ISupportInitialize>()
                    .Setup(x => x.BeginInit());

                this.automationExtension.As<ISupportInitialize>()
                    .Setup(x => x.EndInit());
            }
        }
    }
}