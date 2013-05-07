namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines a view model for an <see cref="IAbstractElement"/>.
    /// </summary>
    public interface IElementViewModel
    {
        /// <summary>
        /// Gets the element data.
        /// </summary>
        IAbstractElement Data { get; }
    }
}
