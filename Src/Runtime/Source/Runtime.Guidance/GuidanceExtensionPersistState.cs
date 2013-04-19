using System;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Represents the persisted information about guidance extensions.
    /// </summary>
    internal class GuidanceExtensionPersistState : IGuidanceExtensionPersistState
    {
        public string ExtensionId { get; set; }
        public string InstanceName { get; set; }
        public Version Version { get; set; }
    }
}
