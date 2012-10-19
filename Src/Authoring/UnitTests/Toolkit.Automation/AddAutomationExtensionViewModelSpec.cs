using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Patterning.Extensibility;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.VisualStudio.Patterning.Runtime.Schema;

namespace Microsoft.VisualStudio.Patterning.Authoring.UnitTests
{
	public class AddAutomationExtensionViewModelSpec
	{
		internal static readonly IAssertion Assert = new Assertion();

		[TestClass]
		public class GivenNoContext
		{
			[TestMethod]
			public void WhenInitializingAndLibraryIsNull_ThenThrowsArgumentNullException()
			{
				Assert.Throws<ArgumentNullException>(() => new AddAutomationExtensionViewModel(null));
			}
		}

		[TestClass]
		public class GivenAnExportedAutomations
		{
			private AddAutomationExtensionViewModel target;

			[TestInitialize]
			public void Initialize()
			{
				var automationExtensions = new List<IExportedAutomationMetadata> { new Mock<IExportedAutomationMetadata>().Object };

				this.target = new AddAutomationExtensionViewModel(automationExtensions);
			}

			[TestMethod]
			public void WhenInitializing_ThenLibraryIsExposed()
			{
				Assert.Equal(1, this.target.ExportedAutomations.Count());
			}

			[TestMethod]
			public void WhenCurrentAutomationExtensionIsNull_ThenSelectAutomationExtensionCommandCanNotExecute()
			{
				this.target.CurrentExportedAutomation = null;

				Assert.False(this.target.SelectAutomationExtensionCommand.CanExecute(new Mock<IDialogWindow>().Object));
			}

			[TestMethod]
			public void WhenCurrentAutomationExtensionIsNotNull_ThenSelectAutomationExtensionCommandCanExecute()
			{
				this.target.CurrentExportedAutomation = new Mock<IExportedAutomationMetadata>().Object;

				Assert.True(this.target.SelectAutomationExtensionCommand.CanExecute(new Mock<IDialogWindow>().Object));
			}

			[TestMethod]
			public void WhenDataIsValidAndSelectAutomationExtensionCommandExecuting_ThenDialogIsClosed()
			{
				var dialog = new Mock<IDialogWindow>();

				this.target.CurrentExportedAutomation = new Mock<IExportedAutomationMetadata>().Object;

				this.target.SelectAutomationExtensionCommand.Execute(dialog.Object);

				dialog.VerifySet(d => d.DialogResult = true);
				dialog.Verify(d => d.Close());
			}
		}
	}
}