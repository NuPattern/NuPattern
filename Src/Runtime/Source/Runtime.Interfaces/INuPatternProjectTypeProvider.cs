using System;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime
{
    /// <summary>
    /// Same as IProjectTypeProvider from Feature Builder, but allow as to reexport.
    /// Export does not need additional attributes.
    /// </summary>
    [CLSCompliant(false)]
    public interface INuPatternProjectTypeProvider : IProjectTypeProvider
    {
    }
}
