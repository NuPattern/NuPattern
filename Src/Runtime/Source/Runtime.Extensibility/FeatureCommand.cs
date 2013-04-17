namespace Microsoft.VisualStudio.TeamArchitect.PowerTools.Features
{
    /// <summary>
    /// A command used in automation
    /// </summary>
    [FeatureCommand]
    public abstract class FeatureCommand : IFeatureCommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        public abstract void Execute();
    }
}