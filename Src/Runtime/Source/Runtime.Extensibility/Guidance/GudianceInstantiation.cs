namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines the instantiation modes for guidance extensions
    /// </summary>
    public enum GudianceInstantiation
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