using System;
using System.Collections.Generic;
using System.Linq;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.Guidance.ShortcutProviders
{
    /// <summary>
    /// A shortcut to a pattern
    /// </summary>
    internal class GuidanceShortcut : IShortcut
    {
        internal const string CommandTypeParameterName = "command";
        internal const string DefaultNameParameterName = "defaultname";
        internal const string InstanceNameParameterName = "instancename";
        internal const string ExtensionIdParameterName = "guidanceid";
        internal const string ShortcutType = "guidance";
        private IShortcut shortcut;

        /// <summary>
        /// Creates a new instance of the <see cref="GuidanceShortcut"/> class.
        /// </summary>
        public GuidanceShortcut(IShortcut shortcut)
        {
            Guard.NotNull(() => shortcut, shortcut);

            this.shortcut = shortcut;
            this.Parameters = shortcut.Parameters ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the type of the shortcut.
        /// </summary>
        public string Type
        {
            get { return ShortcutType; }
        }

        /// <summary>
        /// Gtes or sets the description of the shortcut.
        /// </summary>
        public string Description
        {
            get { return this.shortcut.Description; }
        }

        /// <summary>
        /// Gets the parameters of the shortcut.
        /// </summary>
        public IDictionary<string, string> Parameters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the shortcut type
        /// </summary>
        public GuidanceShortcutCommandType CommandType
        {
            get
            {
                var enumVal = GetParameterValue(CommandTypeParameterName);
                GuidanceShortcutCommandType commandType;
                if (Enum.TryParse(enumVal, true, out commandType))
                {
                    return commandType;
                }

                return GuidanceShortcutCommandType.Undefined;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the guidance extension.
        /// </summary>
        public string GuidanceExtensionId
        {
            get { return GetParameterValue(ExtensionIdParameterName); }
        }

        /// <summary>
        /// Gets or sets the name of the instance to activate or instantiate
        /// </summary>
        public string InstanceName
        {
            get { return GetParameterValue(InstanceNameParameterName); }
        }

        /// <summary>
        /// Gets or sets the default name for instantiating a new instance of a workflow
        /// </summary>
        public string DefaultName
        {
            get { return GetParameterValue(DefaultNameParameterName); }
        }

        private string GetParameterValue(string parameterName)
        {
            var param = this.Parameters.FirstOrDefault(p => p.Key.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
            return (string.IsNullOrEmpty(param.Key)) ? null : param.Value;
        }
    }
}
