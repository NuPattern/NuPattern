using System;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Represents a content provided by a guidance extension
    /// </summary>
    public class GuidanceContent
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GuidanceContent"/>
        /// </summary>
        /// <param name="guidanceExtensionId"></param>
        /// <param name="path"></param>
        public GuidanceContent(string guidanceExtensionId, string path)
        {
            this.GuidanceExtensionId = guidanceExtensionId;
            this.Path = path;
        }

        /// <summary>
        /// Converts the <see cref="GuidanceContent"/> to the <see cref="Uri"/>.
        /// </summary>
        /// <param name="content">The guidance content to convert.</param>
        /// <returns>The URI representing the guidance content.</returns>
        public static explicit operator Uri(GuidanceContent content)
        {
            return content == null ? null : new Uri(content.Path);
        }

        /// <summary>
        /// Gets the full path to the content
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the guidance extension id
        /// </summary>
        public string GuidanceExtensionId { get; private set; }
    }
}