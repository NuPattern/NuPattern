using System.Collections.Generic;

namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// Provides visual studio known launch point locations
    /// </summary>
    internal interface IVsLaunchPointLocationProvider
    {
        IEnumerable<VsLaunchPointLocation> GetKnownLocations();
    }
}
