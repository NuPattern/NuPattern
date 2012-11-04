using System;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI.UnitTests
{
    public class AddNewNodeViewModelSpec
    {
        internal static readonly IAssertion Assert = new Assertion();

        [TestClass]
        public class GuivenNoContext
        {
            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullSiblings_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AddNewNodeViewModel(
                    null,
                    new Mock<IPatternElementInfo>().Object,
                    new Mock<IUserMessageService>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullInfo_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AddNewNodeViewModel(
                    Enumerable.Empty<IProductElement>(),
                    null,
                    new Mock<IUserMessageService>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenCreatingNewWithNullUserMessageService_ThenThrowsArgumentNullException()
            {
                Assert.Throws<ArgumentNullException>(() => new AddNewNodeViewModel(
                    Enumerable.Empty<IProductElement>(),
                    new Mock<IPatternElementInfo>().Object,
                    null));
            }
        }

        [TestClass]
        public class GivenAnEmptySiblings
        {
            private AddNewNodeViewModel target;

            [TestInitialize]
            public void Initialize()
            {
                this.target = new AddNewNodeViewModel(
                    Enumerable.Empty<IProductElement>(),
                    Mocks.Of<IPatternElementInfo>().First(x => x.DisplayName == "Foo"),
                    new Mock<IUserMessageService>().Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializing_ThenInstanceNameTurnedToDefaultName()
            {
                Assert.Equal("Foo1", this.target.InstanceName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameChanging_ThenPropertyChangedIsRaised()
            {
                var eventRaised = false;

                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewNodeViewModel>.GetProperty(x => x.InstanceName).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.InstanceName = "Foo";

                Assert.True(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameChangingToTheSameValue_ThenPropertyChangedIsNotRaised()
            {
                var eventRaised = false;

                this.target.InstanceName = "Foo";
                this.target.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == Reflector<AddNewNodeViewModel>.GetProperty(x => x.InstanceName).Name)
                    {
                        eventRaised = true;
                    }
                };
                this.target.InstanceName = "Foo";

                Assert.False(eventRaised);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameIsNull_ThenErrorMessageIsAdded()
            {
                this.target.InstanceName = null;

                Assert.True(
                    this.target[Reflector<AddNewNodeViewModel>.GetProperty(x => x.InstanceName).Name].Length > 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameIsEmpty_ThenErrorMessageIsAdded()
            {
                this.target.InstanceName = string.Empty;

                Assert.True(
                    this.target[Reflector<AddNewNodeViewModel>.GetProperty(x => x.InstanceName).Name].Length > 1);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInstanceNameIsNullOrEmpty_ThenAcceptCommandCanNotExecute()
            {
                this.target.InstanceName = null;

                Assert.False(this.target.AcceptCommand.CanExecute(new Mock<IDialogWindow>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDataAreValid_ThenAcceptCommandCanExecute()
            {
                this.target.InstanceName = "foo";

                Assert.True(this.target.AcceptCommand.CanExecute(new Mock<IDialogWindow>().Object));
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenDataAreValidAndAcceptCommandExecuting_ThenDialogIsClosed()
            {
                var dialog = new Mock<IDialogWindow>();

                this.target.InstanceName = "foo";

                this.target.AcceptCommand.Execute(dialog.Object);

                dialog.VerifySet(d => d.DialogResult = true);
                dialog.Verify(d => d.Close());
            }
        }

        [TestClass]
        public class GivenSomeSiblings
        {
            private AddNewNodeViewModel target;
            private Mock<IUserMessageService> userMessageService;

            [TestInitialize]
            public void Initialize()
            {
                this.userMessageService = new Mock<IUserMessageService>();

                this.target = new AddNewNodeViewModel(
                    new[] { Mocks.Of<IProductElement>().First(x => x.InstanceName == "Foo1") },
                    Mocks.Of<IPatternElementInfo>().First(x => x.DisplayName == "Foo"),
                    this.userMessageService.Object);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenInitializingAndThereIsANodeWithTheSameName_ThenIncrementorIsUsedWithDefaultName()
            {
                Assert.Equal("Foo2", this.target.InstanceName);
            }

            [TestMethod, TestCategory("Unit")]
            public void WhenAcceptingDialogAndInstanceNameExistsInSiblings_ThenErrorMessageIsShownAndDialogIsNotClosed()
            {
                var dialog = new Mock<IDialogWindow>();

                this.target.InstanceName = "Foo1";

                this.target.AcceptCommand.Execute(dialog.Object);

                dialog.Verify(x => x.Close(), Times.Never());
                this.userMessageService.Verify(x => x.ShowError(It.IsAny<string>()), Times.Once());
            }
        }
    }
}