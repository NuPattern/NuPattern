using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Modeling;
using NuPattern.Runtime.Store;

namespace NuPattern.Runtime.UnitTests.Rules
{
    [TestClass]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test CleanUp")]
    public class PropertyAddRuleSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        private DslTestStore<ProductStateStoreDomainModel> store;
        private Element element;
        private Mock<IPropertyInfo> propertyInfo;

        [TestInitialize]
        public void Initialize()
        {
            this.store = new DslTestStore<ProductStateStoreDomainModel>();

            var elementInfo = new Mock<IElementInfo>();

            using (var tx = this.store.TransactionManager.BeginTransaction())
            {
                this.element = this.store.ElementFactory.CreateElement<Element>();
                this.element.Info = elementInfo.Object;
                tx.Commit();
            }

            this.propertyInfo = new Mock<IPropertyInfo>();
            this.propertyInfo.Setup(v => v.Id).Returns(Guid.NewGuid());
            this.propertyInfo.Setup(v => v.Parent).Returns(this.element.Info);

            elementInfo.Setup(e => e.Properties).Returns(new[] { this.propertyInfo.Object });
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.store.Dispose();
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCreatingNewPropertyWithoutDefinitionId_ThenInfoTurnsNull()
        {
            using (var tx = this.store.TransactionManager.BeginTransaction())
            {
                var target = this.store.ElementFactory.CreateElement<Property>();
                tx.Commit();

                Assert.Null(target.Info);
            }
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCreatingNewPropertyWithDefinitionId_ThenSetsSchemaInfo()
        {
            var target = this.CreateProperty();

            Assert.Equal(this.propertyInfo.Object, target.Info);
        }

        [TestMethod, TestCategory("Unit")]
        public void WhenCreatingNewProperty_ThenSetProperties()
        {
            this.propertyInfo.Setup(p => p.Name).Returns("bar");
            this.propertyInfo.Setup(p => p.DisplayName).Returns("bar display");

            var target = this.CreateProperty();

            Assert.Equal(this.propertyInfo.Object.Name, target.Info.Name);
        }

        private Property CreateProperty()
        {
            using (var tx = this.store.TransactionManager.BeginTransaction())
            {
                var target = this.store.ElementFactory.CreateElement<Property>();
                target.DefinitionId = this.propertyInfo.Object.Id;
                target.Owner = this.element;
                tx.Commit();
                return target;
            }
        }
    }
}