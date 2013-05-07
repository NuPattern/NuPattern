using NuPattern.Runtime.Composition;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// The service that provides MEF composition and exports resolution for bound components.
    /// </summary>
    public interface IBindingCompositionService : INuPatternCompositionService
    {
        /// <summary>
        /// Creates the context for providing dynamic values for binding evaluation.
        /// </summary>
        IDynamicBindingContext CreateDynamicContext();
    }
}
