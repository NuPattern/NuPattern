using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuPattern.Presentation;
using NuPattern.Runtime.UI.ViewModels;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.UI.Views
{
    /// <summary>
    /// Interaction logic for SolutionBuilderView.xaml.
    /// </summary>
    partial class SolutionBuilderView : CommonUserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionBuilderView"/> class.
        /// </summary>
        public SolutionBuilderView()
            : base()
        {
            this.InitializeComponent();
            var componentModel = ServiceProvider.GlobalProvider.GetService<SComponentModel, IComponentModel>();
            var container = componentModel.DefaultExportProvider as CompositionContainer;
            if (container != null)
            {
                container.ComposeExportedValue(this);
            }
        }

        private static IEditableCollectionView GetEditableView(DependencyObject reference)
        {
            if (reference != null)
            {
                var itemsControl = reference.FindAncestor<TreeViewItem>().FindAncestor<ItemsControl>();
                if (itemsControl != null)
                {
                    return itemsControl.Items as IEditableCollectionView;
                }
            }

            return null;
        }

        #region Drag

        private Point startPoint;

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var mousePos = e.GetPosition(null);
                var diff = startPoint - mousePos;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var treeView = sender as TreeView;
                    var treeViewItem =
                        FindAncestor<TreeViewItem>((DependencyObject)e.OriginalSource);

                    if (treeView == null || treeViewItem == null)
                        return;

                    var viewModel = treeView.SelectedItem as ProductElementViewModel;
                    if (viewModel == null)
                        return;

                    var dragData = new DataObject("VSPAT", viewModel.Model);
                    DragDrop.DoDragDrop(treeViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        #endregion

        #region Drop

        internal event DragEventHandler ElementDragEnter;
        internal event DragEventHandler ElementDragLeave;
        internal event DragEventHandler ElementDrop;

        internal DragDropEffects effects = DragDropEffects.None;

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (ElementDragEnter != null)
            {
                ElementDragEnter(((ProductElementViewModel)(((TreeViewItem)sender).DataContext)).Model, e);
            }

            effects = e.Effects;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = effects;
            e.Handled = true;

            if (ElementDragEnter != null)
            {
                ElementDragEnter(((ProductElementViewModel)(((TreeViewItem)sender).DataContext)).Model, e);
            }
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            if (ElementDragLeave != null)
            {
                ElementDragLeave(((ProductElementViewModel)(((TreeViewItem)sender).DataContext)).Model, e);
            }
        }

        private void OnDragDrop(object sender, DragEventArgs e)
        {
            if (ElementDrop != null)
            {
                ElementDrop(((ProductElementViewModel)(((TreeViewItem)sender).DataContext)).Model, e);
            }
            e.Handled = true;
        }

        #endregion

        private void OnItemMouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var item = sender as TreeViewItem;
            if (item != null)
            {
                item.Focus();
                e.Handled = true;
            }
        }

        private void OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var item = sender as TreeViewItem;

            if (item != null)
            {
                item.ContextMenu.Items.OfType<AutomationMenuOptionViewModel>().ForEach(
                    m => m.ReEvaluateCommand());
            }
        }

        private void OnItemEditBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var itemEditBox = sender as TextBox;
            if (itemEditBox != null)
            {
                itemEditBox.SelectAll();
                e.Handled = true;
            }
        }

        private void OnItemEditBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var view = GetEditableView(sender as DependencyObject);
            if (view != null && view.IsEditingItem)
            {
                view.CommitEdit();
                e.Handled = true;
            }
        }

        private static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }
    }
}