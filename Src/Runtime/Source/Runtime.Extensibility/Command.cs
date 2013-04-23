namespace NuPattern.Runtime
{
    /// <summary>
    /// A command used in automation
    /// </summary>
    [Command]
    public abstract class Command : ICommand
    {
        /// <summary>
        /// Executes the command
        /// </summary>
        public abstract void Execute();
    }
}