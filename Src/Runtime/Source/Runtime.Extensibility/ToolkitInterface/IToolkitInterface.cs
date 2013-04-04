
namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Marker interface implemented by all toolkit interface layer proxies and interfaces.
    /// </summary>
    public interface IToolkitInterface
    {
        /// <summary>
        /// Gets the generic underlying runtime element converted to the given type if possible (null otherwise).
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As")]
        TRuntimeInterface As<TRuntimeInterface>() where TRuntimeInterface : class;
    }
}
