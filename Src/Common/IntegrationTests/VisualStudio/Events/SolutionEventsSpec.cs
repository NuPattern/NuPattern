using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VSSDK.Tools.VsIdeTesting;
using NuPattern.VisualStudio.Solution;
using SolutionEvents = NuPattern.VisualStudio.Events.SolutionEvents;

namespace NuPattern.IntegrationTests.VisualStudio.Events
{
    public class SolutionEventsSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
        public class GivenAnEnviromentWithoutSolutionOpened
        {
            private SolutionEvents solutionEvents;
            private ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                this.solutionEvents = new SolutionEvents(VsIdeTestHostContext.ServiceProvider);
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenThereIsNoSolutionOpened_ThenIsSolutionOpenedIsFalse()
            {
                Assert.False(this.solutionEvents.IsSolutionOpened);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenCreatingANewSolution_ThenIsSolutionOpenedTurnsTrue()
            {
                this.solution.CreateInstance(Path.GetTempPath(), Path.GetFileName(Path.GetRandomFileName()));

                Assert.True(this.solutionEvents.IsSolutionOpened);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenCreatingANewSolution_ThenRaisesSolutionOpened()
            {
                var eventRaised = false;

                this.solutionEvents.SolutionOpened += (s, e) => eventRaised = true;
                this.solution.CreateInstance(Path.GetTempPath(), Path.GetFileName(Path.GetRandomFileName()));

                Assert.True(eventRaised);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenCreatingANewSolution_ThenIsSolutionOpenedTurnsTrueAndThenRaisesSolutionOpened()
            {
                var sequenceAchived = false;

                this.solutionEvents.SolutionOpened += (s, e) => sequenceAchived = this.solutionEvents.IsSolutionOpened;
                this.solution.CreateInstance(Path.GetTempPath(), Path.GetFileName(Path.GetRandomFileName()));

                Assert.True(sequenceAchived);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
        public class GivenAnEnviromentWithANewSolutionCreated
        {
            private SolutionEvents solutionEvents;
            private ISolution solution;

            [TestInitialize]
            public void Initialize()
            {
                this.solutionEvents = new SolutionEvents(VsIdeTestHostContext.ServiceProvider);

                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();
                this.solution.CreateInstance(Path.GetTempPath(), Path.GetFileName(Path.GetRandomFileName()));
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenClosingTheSolution_ThenIsSolutionOpenedTurnsFalse()
            {
                this.solution.As<EnvDTE.Solution>().Close();

                Assert.False(this.solutionEvents.IsSolutionOpened);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenClosingTheSolution_ThenRaisesSolutionClosing()
            {
                var eventRaised = false;

                this.solutionEvents.SolutionClosing += (s, e) => eventRaised = true;
                this.solution.As<EnvDTE.Solution>().Close();

                Assert.True(eventRaised);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenClosingTheSolution_ThenRaisesSolutionClosed()
            {
                var eventRaised = false;

                this.solutionEvents.SolutionClosed += (s, e) => eventRaised = true;
                this.solution.As<EnvDTE.Solution>().Close();

                Assert.True(eventRaised);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenClosingTheSolution_ThenRaisesSolutionClosingAndThenIsSolutionOpenedTurnsFalseAndThenRaisesSolutionClosed()
            {
                var index = 0;

                this.solutionEvents.SolutionClosing += (s, e) =>
                {
                    if (this.solutionEvents.IsSolutionOpened && index == 0)
                    {
                        index++;
                    }
                };
                this.solutionEvents.SolutionClosed += (s, e) =>
                {
                    if (!this.solutionEvents.IsSolutionOpened && index == 1)
                    {
                        index++;
                    }
                };
                this.solution.As<EnvDTE.Solution>().Close();

                Assert.Equal(2, index);
            }
        }

        [TestClass]
        [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code")]
        public class GivenAnExistingSolution
        {
            private SolutionEvents solutionEvents;
            private ISolution solution;
            private string solutionPath;

            [TestInitialize]
            public void Initialize()
            {
                this.solutionEvents = new SolutionEvents(VsIdeTestHostContext.ServiceProvider);
                this.solution = VsIdeTestHostContext.ServiceProvider.GetService<ISolution>();

                // Create a new solution and close it
                var fileName = Path.GetFileName(Path.GetRandomFileName());
                this.solution.CreateInstance(Path.GetTempPath(), fileName);
                this.solution.As<EnvDTE.Solution>().SaveAs(fileName);
                this.solutionPath = this.solution.PhysicalPath;
                this.solution.As<EnvDTE.Solution>().Close();
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpeningSolution_ThenIsSolutionOpenedTurnsTrue()
            {
                this.solution.As<EnvDTE.Solution>().Open(this.solutionPath);

                Assert.True(this.solutionEvents.IsSolutionOpened);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpeningSolution_ThenRaisesSolutionOpened()
            {
                var eventRaised = false;

                this.solutionEvents.SolutionOpened += (s, e) => eventRaised = true;
                this.solution.As<EnvDTE.Solution>().Open(this.solutionPath);

                Assert.True(eventRaised);
            }

            [HostType("VS IDE")]
            [TestMethod, TestCategory("Integration")]
            public void WhenOpeningSolution_ThenIsSolutionOpenedTurnsTrueAndThenRaisesSolutionOpened()
            {
                var sequenceAchived = false;

                this.solutionEvents.SolutionOpened += (s, e) => sequenceAchived = this.solutionEvents.IsSolutionOpened;
                this.solution.As<EnvDTE.Solution>().Open(this.solutionPath);

                Assert.True(sequenceAchived);
            }
        }
    }
}