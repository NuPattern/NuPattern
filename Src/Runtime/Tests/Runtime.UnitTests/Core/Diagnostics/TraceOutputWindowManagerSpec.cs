using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Runtime.Diagnostics;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UnitTests.Core.Diagnostics
{
    [TestClass]
    public class TraceOutputWindowManagerSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            private TraceOutputWindowManager manager;

            [TestInitialize]
            public void InitializeContext()
            {
                this.manager = new TraceOutputWindowManager(Mock.Of<IServiceProvider>(), Mock.Of<IShellEvents>());
            }

            [TestMethod, TestCategory("Unit")]
            public void ThenNoTracePanes()
            {
                Assert.Equal(0, this.manager.TracePanes.Count());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatePaneWithNullTitle_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    this.manager.CreateTracePane(Guid.NewGuid(), null, new[] { "Foo" }));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatePaneWithNullSources_ThenThrows()
            {
                Assert.Throws<ArgumentNullException>(() =>
                    this.manager.CreateTracePane(Guid.NewGuid(), "Foo", null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatePaneWithASource_ThenTracePaneCreated()
            {
                var traceId = Guid.NewGuid();
                this.manager.CreateTracePane(traceId, "Foo", new[] { "Source1" });

                Assert.Equal(1, this.manager.TracePanes.Count());
                Assert.Equal(traceId, this.manager.TracePanes.First().TracePaneId);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatePaneWithDuplicateTraceId_ThenThrows()
            {
                var traceId = Guid.NewGuid();
                this.manager.CreateTracePane(traceId, "Foo", new[] { "Source1" });

                Assert.Throws<InvalidOperationException>(() =>
                    this.manager.CreateTracePane(traceId, "Foo", new[] { "Source1" }));
            }
        }
    }
}
