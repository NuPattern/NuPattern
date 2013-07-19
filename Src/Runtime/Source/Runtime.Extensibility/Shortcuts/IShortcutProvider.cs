
namespace NuPattern.Runtime.Shortcuts
{
    /// <summary>
    /// A provider for a shortcut.
    /// </summary>
    public interface IShortcutProvider
    {
        /// <summary>
        /// Gets the type of the shortcut.
        /// </summary>
        string Type { get; }
    }

    /// <summary>
    /// A provider for executing and updating a shortcut.
    /// </summary>
    public interface IShortcutProvider<T> : IShortcutProvider where T : IShortcut
    {
        /// <summary>
        /// Resolves the shortcut to the given type.
        /// </summary>
        /// <param name="shortcut"></param>
        /// <returns></returns>
        T ResolveShortcut(IShortcut shortcut);

        /// <summary>
        /// Executes the shortcut.
        /// </summary>
        void Execute(T instance);

        /// <summary>
        /// Creates a new shortcut
        /// </summary>
        IShortcut CreateShortcut(T instance);
    }
}
