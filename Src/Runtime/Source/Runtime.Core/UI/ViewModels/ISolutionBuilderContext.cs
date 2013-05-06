using System;
namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Public interface of SolutionBuilderContext
    /// </summary>
    public interface ISolutionBuilderContext
    {
        /// <summary>
        /// PatternManager property
        /// </summary>
        NuPattern.Runtime.IPatternManager PatternManager { get; }
    }
}
