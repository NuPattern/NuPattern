using System;
using System.Globalization;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace Microsoft.VisualStudio.Patterning.Build
{
    /// <summary>
    /// MsBuild task to write registry settings.
    /// </summary>
    public class RegistrySetValue : Task
    {
        private RegistryHive hive;

        /// <summary>
        /// Creates a new instance of the <see cref="RegistrySetValue"/> class.
        /// </summary>
        public RegistrySetValue()
            : base(Resources.ResourceManager)
        {
        }

        /// <summary>
        /// Sets the type of the data.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Gets the data.
        /// </summary>
        [Output]
        public string Data { get; set; }

        /// <summary>
        /// Sets the value. If Value is not provided, an attempt will be made to read the Default Value.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Sets the key.
        /// </summary>
        [Required]
        public string Key { get; set; }

        /// <summary>
        /// Sets the Registry Hive. Supports ClassesRoot, CurrentUser, LocalMachine, Users, PerformanceData, CurrentConfig, DynData
        /// </summary>
        [Required]
        public string RegistryHive
        {
            get
            {
                return this.hive.ToString();
            }

            set
            {
                this.hive = (RegistryHive)Enum.Parse(typeof(RegistryHive), value);
            }
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            RegistryKey registryKey = null;
            try
            {
                try
                {
                    registryKey = RegistryKey.OpenRemoteBaseKey(this.hive, Environment.MachineName);
                }
                catch (Exception)
                {
                    this.Log.LogErrorFromResources(Resources.RegistrySetValue_RegistryHiveNotExist, this.RegistryHive, Environment.MachineName);
                    throw;
                }

                if (registryKey != null)
                {
                    using (var subkey = registryKey.OpenSubKey(this.Key, true))
                    {
                        if (subkey != null)
                        {
                            string oldData = GetRegistryKeyValue(subkey, this.Value);
                            if (oldData == null || oldData != this.Data)
                            {
                                if (string.IsNullOrEmpty(this.DataType))
                                {
                                    subkey.SetValue(this.Value, this.Data);
                                }
                                else
                                {
                                    // assumption that ',' is separator for binary and multistring value types.
                                    char[] separator = { ',' };
                                    object registryValue;

                                    RegistryValueKind valueKind = (Microsoft.Win32.RegistryValueKind)Enum.Parse(typeof(RegistryValueKind), this.DataType, true);
                                    switch (valueKind)
                                    {
                                        case RegistryValueKind.Binary:
                                            string[] parts = this.Data.Split(separator);
                                            byte[] val = new byte[parts.Length];
                                            for (int i = 0; i < parts.Length; i++)
                                            {
                                                val[i] = byte.Parse(parts[i], CultureInfo.CurrentCulture);
                                            }

                                            registryValue = val;
                                            break;
                                        case RegistryValueKind.DWord:
                                            registryValue = uint.Parse(this.Data, CultureInfo.CurrentCulture);
                                            break;
                                        case RegistryValueKind.MultiString:
                                            parts = this.Data.Split(separator);
                                            registryValue = parts;
                                            break;
                                        case RegistryValueKind.QWord:
                                            registryValue = ulong.Parse(this.Data, CultureInfo.CurrentCulture);
                                            break;
                                        default:
                                            registryValue = this.Data;
                                            break;
                                    }

                                    subkey.SetValue(this.Value, registryValue, valueKind);
                                }
                            }
                        }
                        else
                        {
                            this.Log.LogErrorFromResources(Resources.RegistrySetValue_RegistryKeyNotFound, this.Key, this.RegistryHive, Environment.MachineName);
                        }
                    }
                }

                return true;
            }
            finally
            {
                if (registryKey != null)
                {
                    registryKey.Close();
                }
            }
        }

        private static string GetRegistryKeyValue(RegistryKey subkey, string value)
        {
            var v = subkey.GetValue(value);
            if (v == null)
            {
                return null;
            }

            RegistryValueKind valueKind = subkey.GetValueKind(value);
            if (valueKind == RegistryValueKind.Binary && v is byte[])
            {
                byte[] valueBytes = (byte[])v;
                StringBuilder bytes = new StringBuilder(valueBytes.Length * 2);
                foreach (byte b in valueBytes)
                {
                    bytes.Append(b.ToString(CultureInfo.InvariantCulture));
                    bytes.Append(',');
                }

                return bytes.ToString(0, bytes.Length - 1);
            }

            if (valueKind == RegistryValueKind.MultiString && v is string[])
            {
                var itemList = new StringBuilder();
                foreach (string item in (string[])v)
                {
                    itemList.Append(item);
                    itemList.Append(',');
                }

                return itemList.ToString(0, itemList.Length - 1);
            }

            return v.ToString();
        }

    }
}
