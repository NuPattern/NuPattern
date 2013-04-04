using Microsoft.Win32;

namespace NuPattern.Win32
{
    /// <summary>
    /// Reads values from the Windows registry.
    /// </summary>
    public class RegistryReader : IRegistryReader
    {
        private RegistryKey hive;
        private string keyName;
        private string valueName;

        /// <summary>
        /// Creates a new instance of the <see cref="RegistryReader"/> class;
        /// </summary>
        public RegistryReader(RegistryKey hive, string keyName, string valueName)
        {
            Guard.NotNull(() => hive, hive);
            Guard.NotNullOrEmpty(() => keyName, keyName);
            Guard.NotNullOrEmpty(() => valueName, valueName);

            this.hive = hive;
            this.keyName = keyName;
            this.valueName = valueName;
        }

        /// <summary>
        /// Gets the registry hive to open.
        /// </summary>
        /// <remarks>This value is typically either <see cref="Registry.LocalMachine"/> or <see cref="Registry.CurrentUser"/></remarks>
        public RegistryKey Hive
        {
            get { return this.hive; }
        }

        /// <summary>
        /// Get the name of the Key to open.
        /// </summary>
        public string KeyName
        {
            get { return this.keyName; }
        }

        /// <summary>
        /// Gets the name of the value of the open key.
        /// </summary>
        public string ValueName
        {
            get { return this.valueName; }
        }

        /// <summary>
        /// Reads the value of the key value.
        /// </summary>
        /// <returns></returns>
        public object ReadValue()
        {
            using (RegistryKey key = this.hive.OpenSubKey(this.keyName, false))
            {
                return key.GetValue(this.valueName);
            }
        }
    }
}
