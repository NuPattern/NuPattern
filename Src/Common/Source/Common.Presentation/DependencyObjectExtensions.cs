using System.Windows;
using System.Windows.Media;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Helper methods for <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Travese the visual tree by parent until the type indicated by <typeparamref name="T"/> is found.
        /// </summary>
        /// <typeparam name="T">The type to found.</typeparam>
        /// <param name="reference">The reference <see cref="DependencyObject"/>.</param>
        /// <returns>The parent in the hierarchy for <paramref name="reference"/> or <c>null</c>.</returns>
        public static T FindAncestor<T>(this DependencyObject reference) where T : DependencyObject
        {
            while (reference != null)
            {
                reference = VisualTreeHelper.GetParent(reference);
                var item = reference as T;
                if (item != null)
                {
                    return item;
                }
            }

            return null;
        }
    }
}