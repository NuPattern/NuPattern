using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Patterning.Common.Presentation;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Runtime.UI
{
    /// <summary>
    /// Interaction logic for ProductPicker.xaml.
    /// </summary>
    [CLSCompliant(false)]
    public partial class ProductPicker : CommonDialogWindow, IDialogWindow, IProductPicker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductPicker"/> class.
        /// </summary>
        public ProductPicker()
            : base()
        {
            this.InitializeComponent();

            this.Products = new List<IProduct>();
            this.ok.Command = new RelayCommand(this.SelectItem);
        }

        /// <summary>
        /// Gets the selected item picked from the items tree.
        /// </summary>
        public IInstanceBase SelectedItem
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parent item to traversing the items.
        /// </summary>
        public ICollection<IProduct> Products
        {
            get;
            private set;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Window.Activated"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnActivated(EventArgs e)
        {
            this.solutionItems.ItemsSource = this.Products;
            base.OnActivated(e);
        }

        private void SelectItem()
        {
            this.SelectedItem = this.solutionItems.SelectedItem as IInstanceBase;
            this.DialogResult = true;
            this.Close();
        }
    }
}