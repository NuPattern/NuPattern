using System;
using NuPattern.Library.Commands;
using NuPattern.Runtime;

namespace NuPattern.Library.Automation
{
    /// <summary>
    /// Extensions to be used in elements to add automation elements at runtime
    /// </summary>
    public static class ProductElementExtensions
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
        public static IEventSettings CreateEvent<T>(this IProductElement product, string name, bool filterForCurrentElement)
        {
            Guard.NotNull(() => product, product);

            var patternSchema = product.Info as IPatternElementSchema;
            var eventSettings = patternSchema.CreateAutomationSettings<IEventSettings>(name);
            eventSettings.EventId = typeof(T).ToString();
            eventSettings.FilterForCurrentElement = filterForCurrentElement;
            product.AddAutomationExtension(new EventAutomation(product, eventSettings));
            return eventSettings;
        }

        /// <summary>
        /// Used to add SyncName capabilities to a pattern element.
        /// </summary>
        public static void EnsureSyncArtifactNameExtensionAutomation(this IProductElement product, string tagId, IServiceProvider serviceProvider)
        {
            Guard.NotNull(() => product, product);
            Guard.NotNull(() => tagId, tagId);

            //using (var transaction = pattern.BeginTransaction())
            //{
            //    string commandName = string.Format("SyncName_{0}", tagId.Replace('-', '_'));
            //    if (!pattern.AutomationExtensions.Any(t => t.Name == commandName))
            //    {
            //        var commandSettings = pattern.CreateCommand<SynchArtifactNameCommand>(commandName)
            //            .SetProperty(t => t.ReferenceTag, tagId);

            //        var eventSettings = pattern.CreateEvent<IOnElementPropertyChangedEvent>(string.Format("SyncEvent_{0}", tagId.Replace('-', '_')), true);
            //        eventSettings.CommandId = commandSettings.Id;
            //    }

            //    transaction.Commit();
            //}
        }
    }
}
