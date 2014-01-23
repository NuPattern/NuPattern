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
        internal const string ExtensionIdParameterName = "guidanceid";
        internal const string InstanceNameParameterName = "instancename";
        internal const string AlwaysCreateParameterName = "alwayscreate";
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
        /// Gets or sets the ID of the guidance extension.
        /// </summary>
        public string GuidanceExtensionId
        {
            get { return GetParameterValue(ExtensionIdParameterName); }
        }

        /// <summary>
        /// Gets or sets the instance name.
        /// </summary>
        public string InstanceName
        {
            get { return GetParameterValue(InstanceNameParameterName); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to create  anew instance.
        /// </summary>
        public bool AlwaysCreate
        {
            get
            {
                var tryBool = false;
                if (bool.TryParse(GetParameterValue(AlwaysCreateParameterName), out tryBool))
                {
                    return tryBool;
                }
                return false;
            }
        }

        private string GetParameterValue(string parameterName)
        {
            var param = this.Parameters.FirstOrDefault(p => p.Key.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
            return (string.IsNullOrEmpty(param.Key)) ? null : param.Value;
        }
    }
}
