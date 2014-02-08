using System;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuPattern.Library.Automation;
using NuPattern.Presentation;
using NuPattern.Runtime;
using NuPattern.Runtime.Composition;

namespace NuPattern.Library.UnitTests.Automation.Wizard
{
    public class WizardAutomationSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullOwner_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new WizardAutomation(null, new Mock<IWizardSettings>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullSettings_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new WizardAutomation(new Mock<IProductElement>().Object, null));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeNameFailsToLoad_ThenThrowsTypeLoadException()
            {
                Assert.Throws<ArgumentNullException>(() => new WizardAutomation(
                    new Mock<IProductElement>().Object,
                    Mocks.Of<IWizardSettings>().First(x => x.TypeName == "Foo"))
                    .Execute());
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenTypeNameIsNotAWindow_ThenThrowsInvalidOperationException()
            {
                Assert.Throws<ArgumentNullException>(() => new WizardAutomation(
                    new Mock<IProductElement>().Object,
                    Mocks.Of<IWizardSettings>().First(x => x.TypeName == typeof(string).AssemblyQualifiedName))
                    .Execute());
            }
        }

        [TestClass]
        public class GivenAWizardAutomation
        {
            private WizardAutomation target;

            public Mock<INuPatternCompositionService> CompositionService { get; private set; }

            [TestInitialize]
            public void Initialize()
            {
                this.target = new WizardAutomation(
                    new Mock<IProductElement>().Object,
                    Mocks.Of<IWizardSettings>().First(s => s.Owner.Name == "Foo" && s.TypeName == typeof(MockWizardWindow).AssemblyQualifiedName));

                this.CompositionService = new Mock<INuPatternCompositionService>();
                this.target.CompositionService = this.CompositionService.Object;
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardExecutedAndAccepted_ThenTurnsIsCanceledToFalse()
            {
                MockWizardWindow.ShouldBeCanceled = false;
                this.target.Execute();

                Assert.False(this.target.IsCanceled);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardExecutedAndCanceled_ThenTurnsIsCanceledToTrue()
            {
                MockWizardWindow.ShouldBeCanceled = true;
                this.target.Execute();

                Assert.True(this.target.IsCanceled);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenWizardIsExecuted_ThenCompositionServiceIsUsedToSatisfyImports()
            {
                this.target.Execute();

                this.CompositionService.Verify(x => x.SatisfyImportsOnce(It.IsAny<ComposablePart>()));
            }
        }

        internal class MockWizardWindow : WizardWindow
        {
            public MockWizardWindow()
                : base(new Mock<IServiceProvider>().Object)
            {
            }

            internal static bool ShouldBeCanceled { get; set; }

            protected override void OnActivated(EventArgs e)
            {
                base.OnActivated(e);
                this.DialogResult = !ShouldBeCanceled;
                this.Close();
            }
        }
    }
}