using System;
using System.Linq;
using Microsoft.VisualStudio.Modeling.Validation;
using Microsoft.VisualStudio.Patterning.Extensibility.Binding;
using Microsoft.VisualStudio.Patterning.Runtime;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace Microsoft.VisualStudio.Patterning.Extensibility
{
    /// <summary>
    /// Extensions for dealing with <see cref="IBindingSettings"/>, such as finding the 
    /// <see cref="Type"/> for a given bound <c>typeId</c>.
    /// </summary>
    [CLSCompliant(false)]
    public static class BindingSettingsExtensions
    {
        /// <summary>
        /// Finds the <see cref="Type"/> of the binding <paramref name="settings"/>.
        /// </summary>
        /// <typeparam name="TInterface">The type of the target component interface, such as <see cref="IValueProvider"/>.</typeparam>
        /// <param name="settings">The settings containing the type to find.</param>
        /// <param name="allComponents">All compatible components registered in MEF.</param>
        /// <param name="context">The validation context where the binding is being resolved.</param>
        /// <returns>A valid <see cref="Type"/> if the type was found either in the MEF components or the current solution; <see langword="null"/> otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "By Design")]
        public static Type FindBoundType<TInterface>(
            this IBindingSettings settings,
            ILookup<string, Type> allComponents,
            ValidationContext context)
        {
            Guard.NotNull(() => settings, settings);
            Guard.NotNull(() => allComponents, allComponents);
            Guard.NotNull(() => context, context);

            if (string.IsNullOrEmpty(settings.TypeId))
                return null;

            // Feature components live in the feature runtime decorating catalog that exposes the 
            // ExportingType, so that we don't have to instantiate the target type in order to be able
            // to know its type.
            var boundType = allComponents[settings.TypeId].FirstOrDefault();

            if (boundType == null)
            {
                boundType = context.TryGetProjectComponentType<TInterface>(settings.TypeId);
            }

            return boundType;
        }

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
