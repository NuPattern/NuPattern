using System;
using System.Collections.ObjectModel;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Public interface of SolutionBuilderViewModel
    /// </summary>
    [CLSCompliant(false)]
    public interface ISolutionBuilderViewModel
    {
        /// <summary>
        /// NodesViewModel property
        /// </summary>
        ObservableCollection<IProductElementViewModel> NodesViewModel { get; }
        
        /// <summary>
        /// CurrentNodeChanged event
        /// </summary>
        event EventHandler CurrentNodeChanged;
        
        /// <summary>
        /// CurrentNodeViewModel property
        /// </summary>
        IProductElementViewModel CurrentNodeViewModel { get; }
    }
}
