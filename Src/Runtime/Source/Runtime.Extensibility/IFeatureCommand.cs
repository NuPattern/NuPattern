namespace NuPattern.Runtime
{
    /// <summary>
    /// Feature command
    /// </summary>
    public interface IFeatureCommand
    {
        /// <summary>
        /// Executes the logic of the command
        /// </summary>
        void Execute();
    }
}