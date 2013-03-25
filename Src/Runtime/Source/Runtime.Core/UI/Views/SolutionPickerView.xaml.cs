using NuPattern.Presentation;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// The view for the <see cref="SolutionPicker"/>.
    /// </summary>
    partial class SolutionPickerView : CommonDialogWindow, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPickerView"/> class.
        /// </summary>
        public SolutionPickerView()
            : base()
        {
            this.InitializeComponent();
        }
    }
}