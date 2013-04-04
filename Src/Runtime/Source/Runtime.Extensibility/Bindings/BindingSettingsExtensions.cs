using System;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Extensions for dealing with <see cref="IBindingSettings"/>, such as finding the 
    /// <see cref="Type"/> for a given bound <c>typeId</c>.
    /// </summary>
    [CLSCompliant(false)]
    public static class BindingSettingsExtensions
    {
        /// <summary>
        /// Determines whether the binding is configured.
        /// </summary>
        public static bool IsConfigured(this IBindingSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            return !string.IsNullOrEmpty(settings.TypeId);
        }

        /// <summary>
        /// Resets binding.
        /// </summary>
        public static void Reset(this IBindingSettings settings)
        {
            Guard.NotNull(() => settings, settings);

            settings.TypeId = BindingSettings.Empty;
            settings.Properties.Clear();
        }
    }
}
