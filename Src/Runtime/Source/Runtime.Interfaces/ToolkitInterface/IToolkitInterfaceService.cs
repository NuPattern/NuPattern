using System;
using System.Collections.Generic;

namespace NuPattern.Runtime.ToolkitInterface
{
    /// <summary>
    /// Exposes the published interface layers in the system and their metadata.
    /// </summary>
    public interface IToolkitInterfaceService
    {
        /// <summary>
        /// Gets all registered interfaces.
        /// </summary>
        IEnumerable<Lazy<IToolkitInterface, IToolkitInterfaceMetadata>> AllInterfaces { get; }
    }
}
