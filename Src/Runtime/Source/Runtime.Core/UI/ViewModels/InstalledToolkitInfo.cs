
using System;

namespace NuPattern.Runtime.UI.ViewModels
{
    /// <summary>
    /// Represents a wrapper class for <see cref="IInstalledToolkitInfo"/> that exposes the necessary properties to data bind to.
    /// </summary>
    /// <remarks>
    /// We need this class to wrap <see cref="IInstalledToolkitInfo"/> because XAML cannot bind to the internal properties of the underlying
    /// objects of the properties of <see cref="IInstalledToolkitInfo"/> such as PatternModelSchema. 
    /// In order to data bind, XAML requires that properties must be public even though the type itself can be internal.
    /// This type simply exposes the necessary properties that are needed to data bind.
    /// </remarks>
    internal class InstalledToolkitInfo
    {
        private IInstalledToolkitInfo info;

        /// <summary>
        /// Creates a new instance 
        /// </summary>
        public InstalledToolkitInfo(IInstalledToolkitInfo info)
        {
            Guard.NotNull(() => info, info);
            this.info = info;
        }

        /// <summary>
        /// Gets the underlying <see cref="IInstalledToolkitInfo"/>.
        /// </summary>
        public IInstalledToolkitInfo ToolkitInfo
        {
            get { return this.info; }
        }

        /// <summary>
        /// Gets the display name of the pattern
        /// </summary>
        public string PatternDisplayName
        {
            get { return this.info.Schema.Pattern.DisplayName; }
        }

        /// <summary>
        /// Gets the description of the pattern
        /// </summary>
        public string PatternDescription
        {
            get { return this.info.Schema.Pattern.Description; }
        }

        /// <summary>
        /// Gets the name of the pattern
        /// </summary>
        public string PatternName
        {
            get { return this.info.Schema.Pattern.Name; }
        }

        /// <summary>
        /// Gets the icon path of the pattern
        /// </summary>
        public string PatternIconPath
        {
            get { return this.info.Schema.Pattern.Icon; }
        }

        /// <summary>
        /// Gets the icon path of the pattern
        /// </summary>
        public string ToolkitIconPath
        {
            get { return this.info.ToolkitIconPath; }
        }

        /// <summary>
        /// Gets the name of the toolkit
        /// </summary>
        public string ToolkitName
        {
            get { return this.info.Name; }
        }

        /// <summary>
        /// Gets the description of the toolkit
        /// </summary>
        public string ToolkitDescription
        {
            get { return this.info.Description; }
        }

        /// <summary>
        /// Gets the author of the toolkit
        /// </summary>
        public string ToolkitAuthor
        {
            get { return this.info.Author; }
        }

        /// <summary>
        /// Gets the version of the toolkit
        /// </summary>
        public Version ToolkitVersion
        {
            get { return this.info.Version; }
        }

        /// <summary>
        /// Gets the classification of the toolkit.
        /// </summary>
        public IToolkitClassification ToolkitClassification
        {
            get { return this.info.Classification; }
        }
    }
}
