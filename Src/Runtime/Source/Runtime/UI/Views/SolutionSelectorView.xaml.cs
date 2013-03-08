using System;
using NuPattern.Common.Presentation;

namespace NuPattern.Runtime.UI
{
    /// <summary>
    /// The view for the <see cref="SolutionSelector"/>.
    /// </summary>
    [CLSCompliant(false)]
    public partial class SolutionSelectorView : CommonDialogWindow, IDialogWindow
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