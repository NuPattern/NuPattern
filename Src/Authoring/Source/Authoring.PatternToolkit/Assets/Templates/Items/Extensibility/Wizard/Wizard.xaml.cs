using System;
using System.ComponentModel;
using NuPattern.Common.Presentation;

namespace $rootnamespace$
{
	/// <summary>
	/// A wizard for displaying pages to configure the current element.
	/// </summary>
	[DisplayName("$safeitemname$")]
	[Description("A custom wizard for displaying pages to configure the current element.")]
	[Category("General")]
	[CLSCompliant(false)]
	public partial class $safeitemname$ : WizardWindow
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="$safeitemname$"/> class.
		/// </summary>
		/// <param name="serviceProvider">A service provider.</param>
		public $safeitemname$(IServiceProvider serviceProvider) :
			base(serviceProvider)
		{
			this.InitializeComponent();
		}
	}
}