using NuPattern.Presentation;

namespace NuPattern.Runtime.UI.Views
{
    /// <summary>
    /// The view for the <see cref="SolutionSelector"/>.
    /// </summary>
    partial class SolutionSelectorView : CommonDialogWindow, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionSelectorView"/> class.
        /// </summary>
        public SolutionSelectorView()
            : base()
        {
            this.InitializeComponent();
        }
    }
}