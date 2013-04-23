using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using NuPattern.Runtime.ToolkitInterface;

namespace NuPattern.Runtime
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IToolkitInterfaceService))]
    internal class ToolkitInterfaceService : IToolkitInterfaceService
    {
        [ImportMany]
        public IEnumerable<Lazy<IToolkitInterface, IToolkitInterfaceMetadata>> AllInterfaces { get; set; }
    }
}
