using System;
using System.Reflection;
using Microsoft.VisualStudio.Modeling;
using Microsoft.VisualStudio.TeamArchitect.PowerTools.Features;

namespace NuPattern.Runtime.Bindings
{
    /// <summary>
    /// Helper methods for dynamic bindings.
    /// </summary>
    [CLSCompliant(false)]
    public static class DynamicBindingExtensions
    {
        private static readonly MethodInfo AddExportMethod = Reflect<IDynamicBindingContext>.GetMethod(x => x.AddExport<string>(null)).GetGenericMethodDefinition();

        /// <summary>
        /// Adds all types of the instance and any of its base classes except for <see cref="object"/>.
        /// </summary>
        public static void AddExportsFromInheritance(this IDynamicBindingContext context, object instance)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => instance, instance);

            var type = instance.GetType();

            while (type != typeof(object))
            {
                var addExport = AddExportMethod.MakeGenericMethod(type);
                addExport.Invoke(context, new object[] { instance });

                type = type.BaseType;
            }
        }

        /// <summary>
        /// Adds all interfaces implemented or inherited by the <paramref name="instance"/> as 
        /// dynamic exports.
        /// </summary>
        public static void AddExportsFromInterfaces(this IDynamicBindingContext context, object instance)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => instance, instance);

            foreach (var iface in instance.GetType().GetInterfaces())
            {
                var addExport = AddExportMethod.MakeGenericMethod(iface);
                addExport.Invoke(context, new object[] { instance });
            }
        }

        /// <summary>
        /// Creates the dynamic context and adds the extension and its owner as dynamic context values.
        /// </summary>
        public static IDynamicBindingContext CreateDynamicContext<T>(this IDynamicBinding<T> binding, IAutomationExtension extension)
            where T : class
        {
            Guard.NotNull(() => binding, binding);
            Guard.NotNull(() => extension, extension);

            var context = binding.CreateDynamicContext();

            context.AddAutomation(extension);

            return context;
        }

        /// <summary>
        /// Adds the automation extension to the context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="extension"></param>
        public static void AddAutomation(this IDynamicBindingContext context, IAutomationExtension extension)
        {
            Guard.NotNull(() => context, context);
            Guard.NotNull(() => extension, extension);

            context.AddExport(extension);
            context.AddExportsFromInterfaces(extension);
            context.AddExportsFromInterfaces(extension.Owner);

            var mel = extension.Owner as ModelElement;
            if (mel != null)
            {
                context.AddExport(mel);
            }
        }
    }
}
