namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Defines the instantiation modes for features
    /// </summary>
    public enum FeatureInstantiation
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