
namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Implements a query status behavior that a launchpoint can use.
    /// </summary>
    internal interface IQueryStatusStrategy
    {
        /// <summary>
        /// Determines the status for the given <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">The feature to check the status for, or <see langword="null"/>.</param>
        QueryStatus QueryStatus(IGuidanceExtension feature);
    }
}
