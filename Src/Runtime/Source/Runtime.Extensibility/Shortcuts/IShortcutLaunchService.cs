
namespace NuPattern.Runtime.Shortcuts
{
    /// <summary>
    /// A service for launching shortcuts.
    /// </summary>
    public interface IShortcutLaunchService
    {
        /// <summary>
        /// Checks if the type of shortcut is registered as a valid provider.
        /// </summary>
        /// <param name="type">The type of the shortcut to be checked.</param>
        /// <returns><see langword="true"/>, if a provider exists for the type; otherwise <see langword="false"/></returns>
        bool IsTypeRegistered(string type);

        /// <summary>
        /// Resolves an instance of T based on the type of the shortcut 
        /// </summary>
        /// <param name="shortcut">The reference</param>
        /// <returns>The resolved reference</returns>
        IShortcut ResolveShortcut(IShortcut shortcut);

        /// <summary>
        /// Resolves an instance of T based on the type of the shortcut 
        /// </summary>
        /// <typeparam name="T">The type of the instance to be resolved</typeparam>
        /// <param name="shortcut">The reference</param>
        /// <returns>The resolved reference</returns>
        T ResolveShortcut<T>(IShortcut shortcut) where T : class;

        /// <summary>
        /// Creates a reference for the instance of T. 
        /// If the type is not provided the type of T should be used to determine how the reference should be created. 
        /// </summary>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="type">The type will be used to determine how the reference should be created</param>
        /// <returns>The reference to the instance</returns>
        IShortcut CreateShortcut<T>(T instance, string type = null) where T : class;

        /// <summary>
        /// Executes the instance, and returns an updated shortcut (if any)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="type">The type of the instance</param>
        IShortcut Execute<T>(T instance, string type = null) where T : class;

        /// <summary>
        /// Determines if the reference can be created for the instance of T
        /// </summary>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="type">The type will be used to determine how the reference should be created</param>
        /// <returns>True if the reference can be created</returns>
        bool CanCreateShortcut<T>(T instance, string type = null) where T : class;
    }
}
