namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Defines the view model for an <see cref="IProduct"/>.
    /// </summary>
    public interface IProductViewModel
    {
        /// <summary>
        /// Gets the <see cref="IProduct"/> data.
        /// </summary>
        IProduct Data { get; }

        /// <summary>
        /// Gets the current <see cref="IView"/> data.
        /// </summary>
        IView CurrentViewData { get; }
    }
}
