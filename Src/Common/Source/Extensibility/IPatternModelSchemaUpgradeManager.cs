

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Defines an automated schema upgrade manager
    /// </summary>
    public interface IPatternModelSchemaUpgradeManager
    {
        /// <summary>
        /// Executes the automated upgrade with the given context.
        /// </summary>
        /// <param name="context">The context of the schema upgrade</param>
        void Execute(ISchemaUpgradeContext context);
    }
}
