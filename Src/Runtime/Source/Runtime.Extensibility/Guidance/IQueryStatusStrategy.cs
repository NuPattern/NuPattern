
namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines the query status behavior that a launchpoint can use.
    /// </summary>
    public interface IQueryStatusStrategy
    {
        /// <summary>
		/// Determines the status for the given <paramref name="extension"/>.
        /// </summary>
		/// <param name="extension">The extension to check the status for, or <see langword="null"/>.</param>
        QueryStatus QueryStatus(IGuidanceExtension extension);
    }
}
