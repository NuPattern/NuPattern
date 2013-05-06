using System;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// ProductViewModel public interface
    /// </summary>
    public interface IProductViewModel
    {
        /// <summary>
        /// Model property
        /// </summary>
        NuPattern.Runtime.IProduct Model { get; }

        /// <summary>
        /// CurrentView property
        /// </summary>
        IView CurrentView { get; }
    }
}
