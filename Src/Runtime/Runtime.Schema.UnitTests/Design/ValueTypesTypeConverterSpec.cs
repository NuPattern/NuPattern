using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Runtime.Schema.Design;

namespace NuPattern.Runtime.Schema.UnitTests.Design
{
    [TestClass]
    public class ValueTypesTypeConverterSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test code")]
        public class GivenNoContext
        {
            private ValueTypesTypeConverter converter;

            [TestInitialize]
            public void InitializeContext()
            {
                this.converter = new ValueTypesTypeConverter();
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenExclusiveStandardValues()
            {
                Assert.True(this.converter.GetStandardValuesSupported());
                Assert.True(this.converter.GetStandardValuesExclusive());
            }
        }
    }
}