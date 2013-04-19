namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines the instantiation modes for guidance extensions
    /// </summary>
    public enum GuidanceInstantiation
    {
        /// <summary>
        /// Cannot be instantiated
        /// </summary>
        None,

        /// <summary>
        /// Singleton instance 
        /// </summary>
        SingleInstance,

        /// <summary>
        /// Multi-instance
        /// </summary>
        MultipleInstance
    }
}