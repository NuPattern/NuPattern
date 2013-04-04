
using NuPattern.VisualStudio.Extensions;

namespace NuPattern.Runtime
{
    /// <summary>
    /// The classification of a toolkit
    /// </summary>
    internal class ToolkitClassification : IToolkitClassification
    {
        private string category;
        private ExtensionVisibility customizeVisibility;
        private ExtensionVisibility createVisibility;

        /// <summary>
        /// Creates a new instance of the <see cref="ToolkitClassification"/> class.
        /// </summary>
        public ToolkitClassification(string category = "",
            ExtensionVisibility createVisibility = ExtensionVisibility.Expanded,
            ExtensionVisibility customizeVisibility = ExtensionVisibility.Expanded)
        {
            Guard.NotNull(() => category, category);

            this.category = category;
            this.createVisibility = createVisibility;
            this.customizeVisibility = customizeVisibility;
        }

        /// <summary>
        /// Gets the category of the toolkit.
        /// </summary>
        public string Category
        {
            get { return this.category; }
        }

        /// <summary>
        /// Gets the visibility of the toolkit for customization.
        /// </summary>
        public ExtensionVisibility CustomizeVisibility
        {
            get { return this.customizeVisibility; }
        }

        /// <summary>
        /// Gets the visibility of the toolkit for creation.
        /// </summary>
        public ExtensionVisibility CreateVisibility
        {
            get { return this.createVisibility; }
        }
    }
}
