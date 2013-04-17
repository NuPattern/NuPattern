namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
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