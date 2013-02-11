
namespace NuPattern.Runtime
{
    /// <summary>
    /// The classification of a toolkit
    /// </summary>
    internal class ToolkitClassification : IToolkitClassification
    {
        private string category;
        private ToolkitVisibility customizeVisibility;
        private ToolkitVisibility createVisibility;

        /// <summary>
        /// Creates a new instance of the <see cref="ToolkitClassification"/> class.
        /// </summary>
        public ToolkitClassification(string category = "",
            ToolkitVisibility createVisibility = ToolkitVisibility.Expanded,
            ToolkitVisibility customizeVisibility = ToolkitVisibility.Expanded)
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
        public ToolkitVisibility CustomizeVisibility
        {
            get { return this.customizeVisibility; }
        }

        /// <summary>
        /// Gets the visibility of the toolkit for creation.
        /// </summary>
        public ToolkitVisibility CreateVisibility
        {
            get { return this.createVisibility; }
        }
    }
}
