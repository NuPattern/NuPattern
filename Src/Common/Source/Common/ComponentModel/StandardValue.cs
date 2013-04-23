using NuPattern.Properties;

namespace NuPattern.ComponentModel
{
    /// <summary>
    /// A standard value.
    /// </summary>
    public class StandardValue
    {
        /// <summary>
        /// The name of the default group.
        /// </summary>
        public static readonly string DefaultGroupName = Resources.StandardValue_DefaultGroupName;

        /// <summary>
        /// Creates a new instance of the <see cref="StandardValue"/> class.
        /// </summary>
        /// <param name="displayText"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        /// <param name="group"></param>
        public StandardValue(string displayText, object value, string description = "", string group = "")
        {
            this.DisplayText = displayText;
            this.Value = value;
            this.Description = string.IsNullOrEmpty(description) ? displayText : description;
            this.Group = group;

            if (string.IsNullOrEmpty(this.Group))
            {
                this.Group = DefaultGroupName; // Use the default group name
            }
        }

        /// <summary>
        /// Gets or sets the description of the value.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the displayed text for the value.
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the group for the value.
        /// </summary>
        public string Group { get; set; }
    }
}