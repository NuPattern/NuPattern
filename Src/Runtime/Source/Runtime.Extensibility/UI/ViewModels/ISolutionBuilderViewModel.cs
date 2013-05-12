using System;
using System.Collections.ObjectModel;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines a view model for the solution builder window
    /// </summary>
    public interface ISolutionBuilderViewModel
    {
        /// <summary>
        /// Gets the models for the top level product elements
        /// </summary>
        ObservableCollection<IProductElementViewModel> TopLevelNodes { get; }

        /// <summary>
        /// Gets the handler for the CurrentNodeChanged event
        /// </summary>
        event EventHandler CurrentNodeChanged;

        /// <summary>
        /// Gets the model for the current product element.
        /// </summary>
        IProductElementViewModel CurrentElement { get; }

        /// <summary>
        /// Begins edit mode.
        /// </summary>
        void BeginEditNode();

        /// <summary>
        /// Whether edit mode can begin.
        /// </summary>
        /// <returns></returns>
        bool CanBeginEditNode();

        /// <summary>
        /// Reorders the top level elements.
        /// </summary>
        void Reorder();

        /// <summary>
        /// Selects the specified element.
        /// </summary>
        /// <param name="element"></param>
        void Select(IProductElement element);
    }
}
