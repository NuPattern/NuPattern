using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Extensibility;

namespace NuPattern.Runtime.Schema.UnitTests
{
    [TestClass]
    public partial class NamedElementSchemaSpec
    {
        private const string TestName = "TestName";
        private const string TestName2 = "TestName2";
        private const string TestBaseId = "TestBaseId";
        private const string TestDisplayName = "TestDisplayName";

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANamedElementWithAName : GivenANamedElementSchema
        {
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();
            private NamedElementSchema element;

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    var patternModel = this.store.ElementFactory.CreateElement<PatternModelSchema>();
                    var pattern = patternModel.Create<PatternSchema>();
                    var view = pattern.Create<ViewSchema>();
                    this.element = view.Create<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.Element.Name = TestName;
                });
            }

            protected override NamedElementSchema Element
            {
                get
                {
                    return this.element;
                }
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenNameIsNotEmpty()
            {
                Assert.False(String.IsNullOrEmpty(this.Element.Name));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenBaseIdIsEmpty()
            {
                Assert.True(String.IsNullOrEmpty(this.Element.BaseId));
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenIsIheritedFromBaseIsFalse()
            {
                Assert.False(this.Element.IsInheritedFromBase);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameChanged_ThenDisplayNameTracksValue()
            {
                this.Element.WithTransaction<NamedElementSchema>(element => element.Name = TestName2);

                Assert.Equal(NamedElementSchema.MakePascalIntoWords(TestName2), this.Element.DisplayName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenNameChanged_ThenDescriptionTracksValue()
            {
                this.Element.WithTransaction<NamedElementSchema>(element => element.Name = TestName2);

                Assert.Equal(
                    string.Format(CultureInfo.CurrentCulture, Properties.Resources.NamedElementSchema_DescriptionFormat, this.Element.SchemaPath),
                    this.Element.Description);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenANamedElementWithABaseId
        {
            private NamedElementSchema element = null;
            private DslTestStore<PatternModelDomainModel> store = new DslTestStore<PatternModelDomainModel>();

            [TestInitialize]
            public void Initialize()
            {
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element = this.store.ElementFactory.CreateElement<ElementSchema>();
                });
                this.store.TransactionManager.DoWithinTransaction(() =>
                {
                    this.element.BaseId = TestBaseId;
                });
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenBaseIdIsNotEmpty()
            {
                Assert.Equal(TestBaseId, this.element.BaseId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenConstructed_ThenIsInheritedFromBaseIsTrue()
            {
                Assert.True(this.element.IsInheritedFromBase);
            }
        }
    }
}