using Microsoft.Win32;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// A class that reads values form the registry.
    /// </summary>
    public interface IRegistryReader
    {
        /// <summary>
        /// Gets the registry hive to open.
        /// </summary>
        RegistryKey Hive { get; }

        /// <summary>
        /// Gets the name of the key to open.
        /// </summary>
        string KeyName { get; }

        /// <summary>
        /// Gets the name of the value of the open key.
        /// </summary>
        string ValueName { get; }

        /// <summary>
        /// Reads the value of the key.
        /// </summary>
        /// <returns></returns>
        object ReadValue();
    }
}
