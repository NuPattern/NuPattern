using System;
using Microsoft.VisualStudio.Patterning.Common.Presentation;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
    /// <summary>
    /// Interaction logic for AddNewProductView.xaml.
    /// </summary>
    [CLSCompliant(false)]
    public partial class AddNewProductView : CommonDialogWindow, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewProductView"/> class.
        /// </summary>
        public AddNewProductView()
            : base()
        {
            this.InitializeComponent();
        }
    }
}