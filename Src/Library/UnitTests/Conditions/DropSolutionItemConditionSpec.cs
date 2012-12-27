using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuPattern.Library.Conditions;

namespace NuPattern.Library.UnitTests.Conditions
{
    [TestClass]
    public class DropSolutionItemConditionSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private DropSolutionItemCondition condition;

            [TestInitialize]
            public void InitializeContext()
            {
                this.condition = new DropSolutionItemCondition();
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSingleExtensionDoesNotMatch_ThenGetPathsEndingWithExtensionsReturnsNoPaths()
            {
                var extensions = "cs";
                var paths = new List<string>(new[] { "Foo.xml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.False(results.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSingleExtensionWithNoPaths_ThenGetPathsEndingWithExtensionsReturnsNoPaths()
            {
                var extensions = "cs";
                var paths = new List<string>();
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.False(results.Any());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSingleExtensionContainsNoPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = "cs";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSingleExtensionContainsOnlyPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = ".cs";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenSingleExtensionContainsWildcardAndPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = "*.cs";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultiExtensionContainsNoPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = "cs;xml;tt";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultiExtensionContainsOnlyPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = ".cs;.xml;.tt";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt", "Foo.xaml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultiExtensionContainsWildcardAndPeriod_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = "*.cs;*.xml;*.tt";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt", "Foo.xaml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenMultiExtensionContainsMixture_ThenGetPathsEndingWithExtensionsReturnsPaths()
            {
                var extensions = "*.cs;.xml;tt";
                var paths = new List<string>(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt", "Foo.xaml" });
                var results = paths.GetPathsEndingWithExtensions(extensions);

                Assert.True(results.Match(new[] { "Foo.cs", "Bar.cs", "Foo.xml", "Bar.tt" }));
            }
        }
    }

    internal static class SetExtensions
    {
        public static bool Match(this IEnumerable<string> a, IEnumerable<string> b)
        {
            return a.Intersect<string>(b).Any()
                && !a.Except<string>(b).Any()
                && b.Intersect<string>(a).Any()
                && !b.Except<string>(a).Any();
        }
    }
}
