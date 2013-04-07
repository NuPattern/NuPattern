using NuPattern.Library.Commands;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Extensions to be used in elements to add automation elements at runtime
    /// </summary>
    internal static class ProductElementExtensions
    {
        /// <summary>
        /// Adds a new Command by specifying the command type.
        /// </summary>
        /// <returns>Returns an <see cref="ICommandSettings{T}"/> for the added command. May be chaineable.</returns>
        public static ICommandSettings<T> CreateCommand<T>(this IProductElement product, string name)
        {
            Guard.NotNull(() => product, product);

            var patternSchema = product.Info as IPatternElementSchema;
            var commandSettings = patternSchema.CreateAutomationSettings<ICommandSettings>(name);
            commandSettings.TypeId = typeof(T).ToString();
            product.AddAutomationExtension(new CommandAutomation(product, commandSettings));
            return new CommandSettingsDelegator<T>(commandSettings);
        }

        /// <summary>
        /// Adds a new Event Launch Point to the pattern.
        /// </summary>
        /// <returns>Returns an <see cref="IEventSettings"/> for the added event.</returns>
        public static IEventSettings CreateEvent<T>(this IProductElement product, string name)
        {
            Guard.NotNull(() => product, product);

            var patternSchema = product.Info as IPatternElementSchema;
            var eventSettings = patternSchema.CreateAutomationSettings<IEventSettings>(name);
            eventSettings.EventId = typeof(T).ToString();
            product.AddAutomationExtension(new EventAutomation(product, eventSettings));
            return eventSettings;
        }
    }
}
