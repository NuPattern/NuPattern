using System;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// ElementViewModel public interface
    /// </summary>
    public interface IElementViewModel
    {
        /// <summary>
        /// Model property
        /// </summary>
        NuPattern.Runtime.IAbstractElement Model { get; }
    }
}
