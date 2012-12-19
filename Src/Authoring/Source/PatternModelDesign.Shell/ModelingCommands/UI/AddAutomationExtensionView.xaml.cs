using System;
using NuPattern.Common.Presentation;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Interaction logic for AddAutomationExtensionView.xaml.
    /// </summary>
    [CLSCompliant(false)]
    public partial class AddAutomationExtensionView : CommonDialogWindow, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddAutomationExtensionView"/> class.
        /// </summary>
        public AddAutomationExtensionView()
            : base()
        {
            this.InitializeComponent();
        }
    }
}