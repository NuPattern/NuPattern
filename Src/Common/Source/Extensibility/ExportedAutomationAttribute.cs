using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using NuPattern.Runtime;

namespace NuPattern.Extensibility
{
    /// <summary>
    /// Attribute to export automation extensions to MEF
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [MetadataAttribute]
    public sealed class ExportedAutomationAttribute : InheritedExportAttribute, IExportedAutomationMetadata
    {
        private string displayName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportedAutomationAttribute"/> class.
        /// </summary>
        public ExportedAutomationAttribute()
            : base(typeof(IAutomationSettings))
        {
        }

        /// <summary>
        /// Gets the name of the icon resource.
        /// </summary>
        /// <value>The name of the icon resource.</value>
        public string IconPath { get; set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get
            {
                if (this.displayName == null)
                {
                    var name = this.ExportingType.GetCustomAttributes(true)
                        .OfType<DisplayNameAttribute>()
                        .Select(att => att.DisplayName).FirstOrDefault();
                    this.displayName = string.IsNullOrEmpty(name) ? string.Empty : name;
                }

                return this.displayName;
            }

            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// Gets the type of the exporting.
        /// </summary>
        /// <value>The type of the exporting.</value>
        public Type ExportingType { get; set; }
    }
}