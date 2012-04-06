using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime.Store;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UnitTests
{
    [TestClass]
    public class ReferenceKindProviderSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        Mock<IProductElement> element;

        [TestInitialize]
        public void InitializeContext()
        {
            this.element = new Mock<IProductElement>();
        }

        [TestMethod]
        public void WhenAddReferenceWithNullElement_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.AddReference(null, new Uri("solution://")));
        }

        [TestMethod]
        public void WhenAddReferenceWithNullValue_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.AddReference(this.element.Object, null));
        }

        [TestMethod]
        public void WhenSetReferenceWithNullElement_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.SetReference((IProductElement)null, new Uri("solution://")));
        }

        [TestMethod]
        public void WhenSetReferenceWithNullReference_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.SetReference((IReference)null, new Uri("solution://")));
        }

        [TestMethod]
        public void WhenSetReferenceWithNullValue_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.SetReference(this.element.Object, null));
        }

        [TestMethod]
        public void WhenGetReferencesWithNullElement_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.GetReference(null));
        }

        [TestMethod]
        public void WhenGetReferenceWithNullElement_ThenThrows()
        {
            Assert.Throws<ArgumentNullException>(
                () => TestReferenceWithoutItsTypeConverter.GetReference(null));
        }

        [TestClass]
        public class GivenATestReferenceWithoutTypeConverterAndNonConvertibleValue
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private ProductElement element;

            [TestInitialize]
            public void InitializeContext()
            {
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Element>();
                    tx.Commit();
                }
            }

            [TestMethod]
            public void WhenAddingReference_ThenThrowsNotSupportedException()
            {
                Assert.Throws<NotSupportedException>(() =>
                    TestReferenceWithoutItsTypeConverterAndNonConvertibleValue.AddReference(this.element, new Foo { }));
            }

            [TestMethod]
            public void WhenGettingReference_ThenThrowsNotSupportedException()
            {
                element.AddReference(typeof(TestReferenceWithoutItsTypeConverterAndNonConvertibleValue).FullName, "foo://null");

                Assert.Throws<NotSupportedException>(() =>
                    TestReferenceWithoutItsTypeConverterAndNonConvertibleValue.GetReference(this.element));
            }
        }

        [TestClass]
        public class GivenATestReferenceWithoutTypeConverter
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private IReferenceKindProvider provider;
            private ProductElement element;

            [TestInitialize]
            public void InitializeContext()
            {
                this.provider = new TestReferenceWithoutItsTypeConverter();
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Element>();
                    tx.Commit();
                }
            }

            [TestMethod]
            public void WhenAddReferenceWithUri_ThenReferenceAdded()
            {
                var reference = TestReferenceWithoutItsTypeConverter.AddReference(this.element, new Uri("solution://foo/bar.cs"));

                Assert.True(this.element.References.Count == 1);
                Assert.Equal(typeof(TestReferenceWithoutItsTypeConverter).FullName, reference.Kind);
                Assert.Equal("solution://foo/bar.cs", reference.Value);
            }

            [TestMethod]
            public void WhenSetReferenceWithUri_ThenSameReferenceUpdated()
            {
                var newUri = new Uri("solution://foo/bar.cs");
                var reference = TestReferenceWithoutItsTypeConverter.AddReference(this.element, new Uri("solution://"));
                TestReferenceWithoutItsTypeConverter.SetReference(this.element, newUri);

                Assert.True(this.element.References.Count == 1);
                Assert.Equal(typeof(TestReferenceWithoutItsTypeConverter).FullName, reference.Kind);
                Assert.Equal(newUri.ToString(), reference.Value);
                Assert.Equal(newUri, TestReferenceWithoutItsTypeConverter.GetReference(this.element));
            }

            [TestMethod]
            public void WhenSetReferenceWithReferenceInstance_ThenReferenceValueUpdated()
            {
                var newUri = new Uri("solution://foo/bar.cs");
                var reference = TestReferenceWithoutItsTypeConverter.AddReference(this.element, new Uri("solution://"));
                TestReferenceWithoutItsTypeConverter.SetReference(reference, newUri);

                Assert.True(this.element.References.Count == 1);
                Assert.Equal(typeof(TestReferenceWithoutItsTypeConverter).FullName, reference.Kind);
                Assert.Equal(newUri.ToString(), reference.Value);
                Assert.Equal(newUri, TestReferenceWithoutItsTypeConverter.GetReference(this.element));
            }

            [TestMethod]
            public void WhenGetReferenceWithNoReference_ThenNullReturned()
            {
                var uri = TestReferenceWithoutItsTypeConverter.GetReference(this.element);

                Assert.Null(uri);
            }

            [TestMethod]
            public void WhenGetReferenceAfterAddReference_ThenDefaultsToValueTypeConverter()
            {
                TestReferenceWithoutItsTypeConverter.AddReference(this.element, new Uri("solution://foo/bar.cs"));
                var uri = TestReferenceWithoutItsTypeConverter.GetReference(this.element);

                Assert.NotNull(uri);
            }

            [TestMethod]
            public void WhenConvertFromReference_ThenGetsTypedValue()
            {
                var newUri = new Uri("solution://foo/bar.cs");
                var reference = TestReferenceWithoutItsTypeConverter.AddReference(this.element, newUri);
                var typedUri = TestReferenceWithoutItsTypeConverter.FromReference(reference);

                Assert.Equal(newUri, typedUri);
            }
        }

        [TestClass]
        public class GivenATestReferenceWithTypeConverter
        {
            private DslTestStore<ProductStateStoreDomainModel> store;
            private IReferenceKindProvider provider;
            private ProductElement element;

            [TestInitialize]
            public void InitializeContext()
            {
                this.provider = new TestReferenceWithUriConverter();
                this.store = new DslTestStore<ProductStateStoreDomainModel>();

                using (var tx = this.store.TransactionManager.BeginTransaction())
                {
                    this.element = this.store.ElementFactory.CreateElement<Element>();
                    tx.Commit();
                }
            }

            [TestMethod]
            public void WhenAddReferenceWithUri_ThenReferenceAdded()
            {
                var reference = TestReferenceWithUriConverter.AddReference(this.element, new Uri("solution://"));

                Assert.True(this.element.References.Count == 1);
                Assert.Equal(typeof(TestReferenceWithUriConverter).FullName, reference.Kind);
                Assert.Equal("solution://", reference.Value);
            }

            [TestMethod]
            public void WhenSetReferenceWithUri_ThenSameReferenceUpdated()
            {
                var reference = TestReferenceWithUriConverter.AddReference(this.element, new Uri("solution://"));
                TestReferenceWithUriConverter.SetReference(this.element, new Uri("solution://Bar"));

                Assert.True(this.element.References.Count == 1);
                Assert.Equal(typeof(TestReferenceWithUriConverter).FullName, reference.Kind);
                Assert.Equal("solution://Bar", reference.Value);
            }

            [TestMethod]
            public void WhenGetReferenceWithNoReference_ThenNullReturned()
            {
                var uri = TestReferenceWithUriConverter.GetReference(this.element);

                Assert.Null(uri);
            }

            [TestMethod]
            public void WhenGetReferenceAfterAddReference_ThenReturnsUri()
            {
                TestReferenceWithUriConverter.AddReference(this.element, new Uri("solution://"));
                var uri = TestReferenceWithUriConverter.GetReference(this.element);

                Assert.Equal(new Uri("solution://"), uri);
            }

            [TestMethod]
            public void WhenGetReferencesAfterAddReference_ThenThrowsNotSupported()
            {
                TestReferenceWithUriConverter.AddReference(this.element, new Uri("solution://1"));
                TestReferenceWithUriConverter.AddReference(this.element, new Uri("solution://2"));
                var uris = TestReferenceWithUriConverter.GetReferences(this.element);

                Assert.Equal(new Uri("solution://1/"), uris.ToList()[0]);
                Assert.Equal(new Uri("solution://2/"), uris.ToList()[1]);
                Assert.Equal(2, uris.Count());
            }
        }
    }

    [ReferenceKindProvider]
    public class TestReferenceWithoutItsTypeConverter : ReferenceKindProvider<TestReferenceWithoutItsTypeConverter, Uri>
    {
    }

    [ReferenceKindProvider]
    public class TestReferenceWithoutItsTypeConverterAndNonConvertibleValue : ReferenceKindProvider<TestReferenceWithoutItsTypeConverterAndNonConvertibleValue, Foo>
    {
    }

    public class Foo { }

    [ReferenceKindProvider]
    [TypeConverter(typeof(UriTypeConverter))]
    public class TestReferenceWithUriConverter : ReferenceKindProvider<TestReferenceWithUriConverter, Uri>
    {
    }
}