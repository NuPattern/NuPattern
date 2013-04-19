
namespace NuPattern.Runtime
{
    /// <summary>
    /// Extends <see cref="IProperty"/> behavior.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// If the property has a <see cref="IValueProvider"/> configured, this method 
        /// will re-evaluate the value provider and save this value so that the property 
        /// last-known provided value reflects the current one.
        /// </summary>
        public static void SaveProvidedValue(this IProperty property)
        {
            Guard.NotNull(() => property, property);

            if (property.Info != null && property.Info.HasValueProvider())
            {
                var evaluatedValue = property.Value;
                property.Value = evaluatedValue;
            }
        }
    }
}
