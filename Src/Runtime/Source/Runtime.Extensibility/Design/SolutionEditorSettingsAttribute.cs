using System;
using System.Globalization;
using System.Resources;
using NuPattern.VisualStudio.Solution;

namespace NuPattern.Runtime.Design
{
    /// <summary>
    /// Represents the settings to send to the SolutionPicker when you are using with the <see cref="SolutionItemEditor"/>.
    /// The use of this attribute is optional.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    [CLSCompliant(false)]
    public sealed class SolutionEditorSettingsAttribute : Attribute
    {
        private string title;
        private string includeFileExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionEditorSettingsAttribute"/> class.
        /// </summary>
        public SolutionEditorSettingsAttribute()
        {
            this.Kind = ItemKind.Folder | ItemKind.Item | ItemKind.Project | ItemKind.Reference |
                ItemKind.ReferencesFolder | ItemKind.Solution | ItemKind.SolutionFolder | ItemKind.Unknown;
        }

        /// <summary>
        /// Gets or sets the kind of items to get.
        /// </summary>
        public ItemKind Kind { get; set; }

        /// <summary>
        /// Gets or sets the list of included file extensions.
        /// </summary>
        /// <value>The list of included file extensions.</value>
        public string IncludeFileExtensions
        {
            get
            {
                return this.includeFileExtensions ?? (this.includeFileExtensions = this.GetLocalizedString(this.IncludeFileExtensionsResourceName));
            }
            set
            {
                this.includeFileExtensions = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the include file extensions resource.
        /// </summary>
        public string IncludeFileExtensionsResourceName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether include empty containers.
        /// </summary>
        /// <value>
        /// Returns <c>true</c> if include empty containers; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeEmptyContainers { get; set; }

        /// <summary>
        /// Gets the type of the resource.
        /// </summary>
        /// <value>The type of the resource.</value>
        public Type ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the title of the editor.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title ?? (this.title = this.GetLocalizedString(this.TitleResourceName));
            }
            set
            {
                this.title = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the title resource.
        /// </summary>
        public string TitleResourceName { get; set; }

        /// <summary>
        /// Returns the localized string from the resource file.
        /// </summary>
        private string GetLocalizedString(string resourceName)
        {
            if (resourceName != null && this.ResourceType != null)
            {
                var resourceManager = new ResourceManager(this.ResourceType);
                if (resourceManager != null)
                {
                    try
                    {
                        return resourceManager.GetString(resourceName, CultureInfo.CurrentUICulture);
                    }
                    catch (MissingManifestResourceException)
                    {
                        // Ignore invalid resources
                    }
                }
            }

            return resourceName;
        }
    }
}