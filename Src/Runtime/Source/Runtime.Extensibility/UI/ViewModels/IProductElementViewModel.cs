using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines a view model for an <see cref="IProductElement"/>.
    /// </summary>
    public interface IProductElementViewModel
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        ISolutionBuilderContext Context { get; }

        /// <summary>
        /// Gets the path of the icon.
        /// </summary>
        string IconPath { get; set; }

        /// <summary>
        /// Gets the element data.
        /// </summary>
        IProductElement Data { get; }

        /// <summary>
        /// Gets a value indicating whether view is being edited.
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// Gets a vlaue indicating whether the view is currently selected.
        /// </summary>
        bool IsSelected { get; set; }

        /// <summary>
        /// Gets the models of all elements.
        /// </summary>
        ObservableCollection<IProductElementViewModel> ChildNodes { get; }

        /// <summary>
        /// Gets the models of all context menus for this element.
        /// </summary>
        ObservableCollection<IMenuOptionViewModel> MenuOptions { get; }

        /// <summary>
        /// Gets the handler for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        IProductElementViewModel ParentNode { get; }

        /// <summary>
        /// Gets the elements container
        /// </summary>
        IElementContainer ElementContainerData { get; }

        /// <summary>
        /// Reorders the child nodes
        /// </summary>
        void Reorder();

        /// <summary>
        /// Add child nodes to the node.
        /// </summary>
        /// <param name="elements"></param>
        void AddChildNodes(IEnumerable<IProductElement> elements);

        /// <summary>
        /// Gets a value indicating whether the node is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Selects the specified element.
        /// </summary>
        void Select(IProductElement element);

        /// <summary>
        /// Displays the AddNew dialog to retrieve a new instance name
        /// </summary>
        /// <returns></returns>
        string AddNewElement(IPatternElementInfo info);

        /// <summary>
        /// Ends label editing.
        /// </summary>
        void EndEdit();

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        System.Windows.Input.ICommand DeleteCommand { get; }
    }
}
