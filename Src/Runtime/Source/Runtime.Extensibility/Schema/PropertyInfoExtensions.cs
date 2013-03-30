
namespace NuPattern.Runtime
{
    /// <summary>
    /// Provides helper methods for property schemas.
    /// </summary>
    public static class IPropertyInfoExtensions
    {
        /// <summary>
        /// Determines whether the property definition has a value provider for a calculated value.
        /// </summary>
        public static bool HasValueProvider(this IPropertyInfo propertyInfo)
        {
            Guard.NotNull(() => propertyInfo, propertyInfo);

            return !string.IsNullOrEmpty(propertyInfo.ValueProvider.TypeId);
        }
    }
}
