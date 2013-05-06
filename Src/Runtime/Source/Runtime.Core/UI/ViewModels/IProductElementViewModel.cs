using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Public interface ProduceElementViewModel
    /// </summary>
    [CLSCompliant(false)]
    public interface IProductElementViewModel
    {
        /// <summary>
        /// ContextViewModel property
        /// </summary>
        ISolutionBuilderContext ContextViewModel { get; }
        
        /// <summary>
        /// IconPath property
        /// </summary>
        string IconPath { get; set; }
        
        /// <summary>
        /// Model property
        /// </summary>
        IProductElement Model { get; }

        /// <summary>
        /// IsEditing property
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// IsSelected property
        /// </summary>
        bool IsSelected { get; set; }
        
        /// <summary>
        /// NodesViewModel property
        /// </summary>
        ObservableCollection<IProductElementViewModel> NodesViewModel { get; }

        /// <summary>
        /// MenuOptions property
        /// </summary>
        ObservableCollection<MenuOptionViewModel> MenuOptions { get; }

        /// <summary>
        /// PropertyChanged event
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;
    }
}
