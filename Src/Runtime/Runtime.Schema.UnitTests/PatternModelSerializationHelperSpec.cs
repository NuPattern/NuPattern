using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.Patterning.Runtime.Schema.UnitTests
{
    public class PatternModelSerializationHelperSpec
    {
        [TestClass]
        public class GivenANullStore
        {
            internal static readonly IAssertion Assert = new Assertion();

            [TestMethod]
            public void WhenLoadingAModel_ThenNullExceptionIsThrown()
            {
                Assert.Throws<ArgumentNullException>(
                    () => PatternModelSerializationHelper.Instance.LoadModel(null, null, null, null));
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenAStore
        {
            internal static readonly IAssertion Assert = new Assertion();

            private Store store;

            [TestInitialize]
            [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test code")]
            public void InitializeContext()
            {
                this.store = new DslTestStore<PatternModelDomainModel>().Store;
            }

            [TestMethod]
            public void WhenLoadingAModelWithNullStream_ThenNullExceptionIsThrown()
            {
                Assert.Throws<ArgumentNullException>(() =>
                        PatternModelSerializationHelper.Instance.LoadModel(this.store, null, null, null));
            }

            [TestMethod]
            public void WhenLoadingAModel_ThenPatternModelIsReturned()
            {
                var stream = new MemoryStream(
                    UTF8Encoding.UTF8.GetBytes(
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?><patternModel xmlns:dm0=\"http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core\" dslVersion=\"1.0.0.0\" Id=\"fe155518-e417-4f08-94d2-c2678c28fdd8\" xmlns=\"http://schemas.microsoft.com/dsltools/ComponentModel\"></patternModel>"));

                PatternModelSchema patternModel = null;

                this.store.TransactionManager.DoWithinTransaction(() =>
                    patternModel = PatternModelSerializationHelper.Instance.LoadModel(this.store, stream, null, null));

                Assert.NotNull(patternModel);
            }
        }
    }
}
