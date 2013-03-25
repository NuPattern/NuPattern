using System;
using NuPattern.Runtime.Schema.Properties;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Represents one of the types of a <see cref="PropertySchema"/>.Type.
    /// </summary>
    internal class PropertyValueType
    {
        private string displayName;
        private string category;

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyValueType"/> class.
        /// </summary>
        public PropertyValueType()
        {
            this.DataType = typeof(object);
        }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets the displayed name of the type.
        /// </summary>
        public string DisplayName
        {
            get
            {
                if ((string.IsNullOrEmpty(this.displayName))
                    && (this.DataType != null))
                {
                    return this.DataType.FullName;
                }

                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// Gets or sets the category of this type.
        /// </summary>
        public string Category
        {
            get
            {
                if (string.IsNullOrEmpty(this.category))
                {
                    if (string.IsNullOrEmpty(this.displayName))
                    {
                        return Resources.PropertyValueType_Category_Extended;
                    }
                    else
                    {
                        return Resources.PropertyValueType_Category_Common;
                    }
                }

                return this.category;
            }
            set
            {
                this.category = value;
            }
        }
    }
}
