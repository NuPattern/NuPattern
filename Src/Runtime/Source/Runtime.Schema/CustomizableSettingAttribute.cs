using System;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Defines an attribute that is used to designate the element as being a customizable setting.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    internal abstract class CustomizableSettingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the CustomizableSettingAttribute class.
        /// </summary>
        public CustomizableSettingAttribute()
        {
            this.DefaultValue = true;
        }

        /// <summary>
        /// Gets a value indicating whether the default value of the target item is true or false.
        /// </summary>
        public bool DefaultValue { get; private set; }
    }
}
