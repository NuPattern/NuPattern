
namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// Defines the shortcut command types
    /// </summary>
    internal enum GuidanceShortcutCommandType
    {
        /// <summary>
        /// The shortcut command is undefined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The shortcut activates an existing instance of guidance.
        /// </summary>
        Activation = 1,

        /// <summary>
        /// The shortcut instantiates a new instance of guidance.
        /// </summary>
        Instantiation = 2,
    }
}
