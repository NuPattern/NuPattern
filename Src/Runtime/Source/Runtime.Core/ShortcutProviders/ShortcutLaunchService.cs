using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.Shell;
using NuPattern.Runtime.Properties;
using NuPattern.Runtime.Shortcuts;

namespace NuPattern.Runtime.ShortcutProviders
{
    /// <summary>
    /// Servie for lauching shortcuts.
    /// </summary>
    [Export(typeof(IShortcutLaunchService))]
    internal class ShortcutLaunchService : IShortcutLaunchService
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ShortcutLaunchService"/> class.
        /// </summary>
        [ImportingConstructor]
        public ShortcutLaunchService(
            [ImportMany]IEnumerable<IShortcutProvider> providers)
        {
            this.Providers = providers;
        }

        /// <summary>
        /// Gets the registered providers.
        /// </summary>
        public IEnumerable<IShortcutProvider> Providers { get; protected internal set; }

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        [Import(typeof(SVsServiceProvider))]
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Whether the type of the shortcut is registered.
        /// </summary>
        /// <param name="type">The type of the shortcut</param>
        public bool IsTypeRegistered(string type)
        {
            return this.Providers.Any(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Resolves the shortcut to an instance of the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut</param>
        public T ResolveShortcut<T>(IShortcut shortcut) where T : class
        {
            if (!this.Providers.Any())
            {
                return null;
            }

            try
            {
                // it is not ok to cast provider to IShortcutProvider<T> this would only be valid of IShortcutProvider<T> is covariant.
                dynamic provider = GetProvidersAssignableTo(shortcut.Type, typeof(T)).Single();
                return provider.GetType().InvokeMember(@"ResolveShortcut",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, provider, new object[] { shortcut });
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format(Resources.ShortcutLaunchService_ErrorFailedResolveShortcut, shortcut.Type, typeof(T)),
                    ex);
            }
        }

        /// <summary>
        /// Resolves the shortcut to an instance of the shortcut.
        /// </summary>
        /// <param name="shortcut">The shortcut</param>
        public IShortcut ResolveShortcut(IShortcut shortcut)
        {
            if (!this.Providers.Any())
            {
                return null;
            }

            try
            {
                // it is not ok to cast provider to IShortcutProvider<T> this would only be valid of IShortcutProvider<T> is covariant.
                dynamic provider = GetProvidersAssignableTo(shortcut.Type).Single();
                return (IShortcut)provider.GetType().InvokeMember(@"ResolveShortcut",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, provider, new object[] { shortcut });
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format(Resources.ShortcutLaunchService_ErrorFailedResolveShortcutUntyped, shortcut.Type),
                    ex);
            }
        }

        /// <summary>
        /// Executes the shortcut
        /// </summary>
        /// <param name="instance">The instance to execute</param>
        /// <param name="type">The type of the shortcut</param>
        public IShortcut Execute<T>(T instance, string type = null) where T : class
        {
            var provider = GetReferenceProvider(instance, type);
            if (provider == null)
            {
                throw new NotSupportedException();
            }

            return (IShortcut)provider.GetType().InvokeMember(@"Execute",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, provider, new object[] { instance });
        }

        /// <summary>
        /// Creates a new shortcut 
        /// </summary>
        /// <param name="instance">The instance to create</param>
        /// <param name="type">The type of the shortcut</param>
        /// <returns></returns>
        public IShortcut CreateShortcut<T>(T instance, string type = null) where T : class
        {
            var provider = GetReferenceProvider(instance, type);
            if (provider == null)
            {
                throw new NotSupportedException();
            }

            return (IShortcut)provider.GetType().InvokeMember(@"CreateShortcut",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, provider, new object[] { instance });
        }

        /// <summary>
        /// Whether the shortcut can be created.
        /// </summary>
        /// <param name="instance">The instance to create</param>
        /// <param name="type">The type of the shortcut</param>
        public bool CanCreateShortcut<T>(T instance, string type = null) where T : class
        {
            if (!this.Providers.Any())
            {
                return false;
            }

            return GetReferenceProvider(instance, type) != null;
        }

        private IShortcutProvider GetReferenceProvider<T>(T instance, string type) where T : class
        {
            var instanceType = instance.GetType();
            var compatibleProviders = GetProvidersAssignableFrom(type, instanceType);

            return compatibleProviders.FirstOrDefault();
        }

        private IEnumerable<IShortcutProvider> GetProvidersAssignableFrom(string type, Type targetType)
        {
            var compatibleProviders = Providers.Where(provider =>
             (type == null || provider.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
             GetImplementedProviderInterfaces(provider.GetType())
                 .Any(@interface => @interface.GetGenericArguments()[0].IsAssignableFrom(targetType)));

            return compatibleProviders;
        }

        private IEnumerable<IShortcutProvider> GetProvidersAssignableTo(string type, Type targetType)
        {
            var compatibleProviders = Providers.Where(provider =>
                 (type == null || provider.Type.Equals(type, StringComparison.OrdinalIgnoreCase)) &&
                 GetImplementedProviderInterfaces(provider.GetType())
                         .Any(@interface => targetType.IsAssignableFrom(@interface.GetGenericArguments()[0])));

            return compatibleProviders;
        }

        private IEnumerable<IShortcutProvider> GetProvidersAssignableTo(string type)
        {
            var compatibleProviders = Providers.Where(provider =>
                 (type == null || provider.Type.Equals(type, StringComparison.OrdinalIgnoreCase)));

            return compatibleProviders;
        }

        private static IEnumerable<Type> GetImplementedProviderInterfaces(Type type)
        {
            return type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IShortcutProvider<>));
        }
    }
}
