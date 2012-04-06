using System;
using System.ComponentModel;
using Microsoft.VisualStudio.Patterning.Common.Presentation;

namespace Microsoft.VisualStudio.Patterning.Authoring.Authoring.Assets.Wizards
{
	/// <summary>
	/// A wizard for displaying pages to configure the current element.
	/// </summary>
	[DisplayName("ToolkitCreationWizard")]
	[Description("A custom wizard for displaying pages to configure the current element.")]
	[Category("General")]
	[CLSCompliant(false)]
	public partial class ToolkitCreationWizard : WizardWindow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ToolkitCreationWizard"/> class.
		/// </summary>
		/// <param name="serviceProvider">A service provider.</param>
		public ToolkitCreationWizard(IServiceProvider serviceProvider) :
			base(serviceProvider)
		{
			this.InitializeComponent();
		}
	}
}