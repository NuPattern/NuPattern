namespace NuPattern.Runtime
{
    public partial interface IProduct
    {
        /// <summary>
        /// Provides toolkit information for the pattern
        /// </summary>
        IProductToolkitInfo ToolkitInfo { get; }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        IView CurrentView { get; set; }
    }
}