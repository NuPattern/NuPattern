using System.Windows;
using NuPattern.Presentation;

namespace NuPattern.Runtime.UI.Views
{
    /// <summary>
    /// Interaction logic for AddNewNodeView.xaml
    /// </summary>
    partial class AddNewNodeView : CommonDialogWindow, IDialogWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewNodeView"/> class.
        /// </summary>
        public AddNewNodeView()
            : base()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.instanceName.SelectAll();
            this.Loaded -= this.OnLoaded;
        }
    }
}