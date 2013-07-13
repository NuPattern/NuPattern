
using System;
using NuPattern.Runtime.Shell.Properties;
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

            // Read the file content
            try
            {
                var shortcut = fileHandler.ReadShortcut();
                if (shortcut != null)
                {
                    // Execute the shortcut
                    shortcut.Execute();

                    // Update the shortcut
                    if (shortcut.Upated)
                    {
                        fileHandler.WriteShortcut(shortcut);
                    }
                }
            }
            catch (ShortcutFormatException)
            {
                var messageService = serviceProvider.GetService<IUserMessageService>();
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorShortcutFormat);
                }

                return false;
            }
            catch (ShortcutIOException)
            {
                var messageService = serviceProvider.GetService<IUserMessageService>();
                if (messageService != null)
                {
                    messageService.ShowError(Resources.ShortcutEditorFactory_ErrorShortcutIO);
                }

                return false;
            }
            catch (Exception)
            {
                // Ignore exception and just return
                return false;
            }

            return true;
        }
    }
}
