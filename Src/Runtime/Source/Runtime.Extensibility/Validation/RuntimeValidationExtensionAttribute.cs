using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Modeling.Validation;

namespace NuPattern.Runtime.Validation
{
    /// <summary>
    /// Runtime Validation Extension MEF attribute.
    /// </summary>
    [MetadataAttribute, AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RuntimeValidationExtensionAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeValidationExtensionAttribute"/> class.
        /// </summary>
        public RuntimeValidationExtensionAttribute()
            : base(typeof(Action<ValidationContext, object>))
        {
        }

        /// <summary>
        /// Gets the metadata filter.
        /// </summary>
        /// <value>The metadata filter.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "MEF")]
        public object MetadataFilter
        {
            get { return ValidationConstants.MetadataFilter; }
        }
    }
}