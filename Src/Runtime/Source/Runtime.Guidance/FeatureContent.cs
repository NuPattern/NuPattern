using System;

namespace NuPattern.Runtime.Guidance
{
    /// <summary>
    /// Represents a content provided by a feature
    /// </summary>
    internal class FeatureContent
    {
        /// <summary>
        /// Initializes a new instance of <see cref="FeatureContent"/>
        /// </summary>
        /// <param name="featureId"></param>
        /// <param name="path"></param>
        public FeatureContent(string featureId, string path)
        {
            this.FeatureId = featureId;
            this.Path = path;
        }

        /// <summary>
        /// Converts the <see cref="FeatureContent"/> to the <see cref="Uri"/>.
        /// </summary>
        /// <param name="content">The feature content to convert.</param>
        /// <returns>The URI representing the feature content.</returns>
        public static explicit operator Uri(FeatureContent content)
        {
            return content == null ? null : new Uri(content.Path);
        }

        /// <summary>
        /// Gets the full path to the content
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Gets the feature id
        /// </summary>
        public string FeatureId { get; private set; }
    }
}