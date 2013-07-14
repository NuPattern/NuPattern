
using System;
using NuPattern.Runtime.Shell.Properties;
using NuPattern.Runtime.Shortcuts;
using NuPattern.VisualStudio;

namespace NuPattern.Runtime.Shell.Shortcuts
{
    /// <summary>
    /// Coordinates the launching of shortcuts from a persisteance store
    /// </summary>
    internal static class ShortcutLaunchCoordinator
    {
        /// <summary>
        /// Launches a shortcut from file.
        /// </summary>
        public static bool LaunchShortcut(IServiceProvider serviceProvider, IShortcutPersistenceHandler fileHandler)
        {
            Guard.NotNull(() => serviceProvider, serviceProvider);
            Guard.NotNull(() => fileHandler, fileHandler);

            var messageService = serviceProvider.GetService<IUserMessageService>();
            var launchService = serviceProvider.GetService<IShortcutLaunchService>();

            try
            {
                // Read the file content
                var shortcut = fileHandler.ReadShortcut();
                if (shortcut != null)
                {
                    // Check if type is registered
                    if (!launchService.IsTypeRegistered(shortcut.Type))
                    {
                        throw new ShortcutProviderNotExistException();
                    }

                    // Get an actual instance of the provided shortcut
                    var specificShortcut = launchService.ResolveShortcut(shortcut);
                    if (specificShortcut == null)
                    {
                        throw new ShortcutProviderNotExistException();
                    }

                    // Execute the shortcut
                    var newShortcut = launchService.Execute(specificShortcut);

                    // Update the shortcut (if updated at all)
                    if (newShortcut != null)
                    {
                        fileHandler.WriteShortcut(newShortcut);
                    }

                    return true;
                }
            }
            catch (ShortcutProviderNotExistException)
            {
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorShortcutProviderNotExist);
                }
            }
            catch (ShortcutFileFormatException)
            {
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorShortcutFormat);
                }
            }
            catch (ShortcutFileAccessException)
            {
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorShortcutIO);
                }
            }
            catch (Exception)
            {
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorUnknown);
                }
            }

            return false;
        }
    }
}
