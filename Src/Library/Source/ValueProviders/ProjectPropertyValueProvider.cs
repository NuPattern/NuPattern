using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NuPattern.ComponentModel.Design;
using NuPattern.Diagnostics;
using NuPattern.Library.Properties;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Library.ValueProviders
{
    /// <summary>
    /// A <see cref="NuPattern.Runtime.ValueProvider"/> that provides the value of an arbitrary 
    /// project property.
    /// </summary>
    [DisplayNameResource(@"ProjectPropertyValueProvider_DisplayName", typeof(Resources))]
    [DescriptionResource(@"ProjectPropertyValueProvider_Description", typeof(Resources))]
    [CategoryResource(@"AutomationCategory_VisualStudio", typeof(Resources))]
    [CLSCompliant(false)]
    public class ProjectPropertyValueProvider : VsProjectPropertyValueProvider
    {
        /// <summary>
        /// Equals the string "|".
        /// </summary>
        private const string BuildConfigurationSeparator = "|";
        private static readonly ITracer tracer = Tracer.Get<ProjectPropertyValueProvider>();

        /// <summary>
        /// Gets or sets the expression to evaluate.
        /// </summary>
        [DisplayNameResource(@"ProjectPropertyValueProvider_Configuration_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ProjectPropertyValueProvider_Configuration_Description", typeof(Resources))]
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets the expression to evaluate.
        /// </summary>
        [DisplayNameResource(@"ProjectPropertyValueProvider_Platform_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ProjectPropertyValueProvider_Platform_Description", typeof(Resources))]
        public string Platform { get; set; }

        /// <summary>
        /// Gets or sets the expression to evaluate.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayNameResource(@"ProjectPropertyValueProvider_Property_DisplayName", typeof(Resources))]
        [DescriptionResource(@"ProjectPropertyValueProvider_Property_Description", typeof(Resources))]
        public string Property { get; set; }

        /// <summary>
        /// Retrieves the value of the property.
        /// </summary>
        protected override object GetPropertyValue(IProject project)
        {
            var configPlatform = this.Configuration;
            if (!string.IsNullOrEmpty(this.Platform))
            {
                configPlatform = string.IsNullOrEmpty(configPlatform) ?
                    this.Platform :
                    this.Configuration + BuildConfigurationSeparator + this.Platform;
            }

            object value = null;

            if (string.IsNullOrEmpty(configPlatform))
            {
                value = project.Data[this.Property];
                if (value == null)
                    value = project.UserData[this.Property];
            }
            else
            {
                value = project.Data[configPlatform][this.Property];
                if (value == null)
                    value = project.UserData[configPlatform][this.Property];
            }

            return value;
        }
    }
}