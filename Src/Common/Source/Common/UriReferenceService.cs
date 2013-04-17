using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;

namespace NuPattern
{
    [Export(typeof(IUriReferenceService))]
    internal class UriReferenceService : IUriReferenceService
    {
        public UriReferenceService()
        {
            this.Providers = Enumerable.Empty<IUriReferenceProvider>();
        }

        [ImportingConstructor]
        public UriReferenceService([ImportMany]IEnumerable<IUriReferenceProvider> providers)
        {
            this.Providers = providers;
        }

        public IEnumerable<IUriReferenceProvider> Providers { get; protected internal set; }

        public bool IsSchemeRegistered(Uri uri)
        {
            return this.Providers.Any(p => p.UriScheme == uri.Scheme);
        }

        public T ResolveUri<T>(Uri uri) where T : class
        {
            try
            {
                dynamic provider = GetProvidersAssignableTo(uri.Scheme, typeof(T)).Single();
                // it is not ok to cast provider to IUriReferenceProvider<T> this would only be valid of IUriReferenceProvider<T> is covariant.
                return provider.ResolveUri(uri);
            }
            catch (Exception ex)
            {
                throw new NotSupportedException(
                    string.Format("Failed to resolve provider for scheme {0} and type {1}", uri.Scheme, typeof(T)),
                    ex);
            }
        }

        public bool CanCreateUri<T>(T instance, string uriScheme = null)
            where T : class
        {
            Guard.NotNull(() => instance, instance);

            return GetReferenceProvider(instance, uriScheme) != null;
        }

        public void Open<T>(T instance, string uriScheme = null) where T : class
        {
            var provider = GetReferenceProvider(instance, uriScheme);

            if (provider == null)
                throw new NotSupportedException();

            provider.GetType().InvokeMember("Open",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, provider, new object[] { instance });
        }

        public Uri CreateUri<T>(T instance, string uriScheme = null)
            where T : class
        {
            Guard.NotNull(() => instance, instance);

            IUriReferenceProvider selectedProvider = GetReferenceProvider<T>(instance, uriScheme);

            if (selectedProvider == null)
                throw new NotSupportedException();

            return (Uri)selectedProvider.GetType().InvokeMember("CreateUri",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, selectedProvider, new object[] { instance });
        }

        private IUriReferenceProvider GetReferenceProvider<T>(T instance, string uriScheme) where T : class
        {
            IEnumerable<IUriReferenceProvider> compatibleProviders;

            var instanceType = instance.GetType();
            compatibleProviders = GetProvidersAssignableFrom(uriScheme, instanceType);

            return compatibleProviders.FirstOrDefault();
        }

        private IEnumerable<IUriReferenceProvider> GetProvidersAssignableFrom(string uriScheme, Type targetType)
        {
            var compatibleProviders = Providers.Where(provider =>
             (uriScheme == null || provider.UriScheme == uriScheme) &&
             GetImplementedProviderInterfaces(provider.GetType())
                 .Any(@interface => @interface.GetGenericArguments()[0].IsAssignableFrom(targetType)));

            return compatibleProviders;
        }

        private IEnumerable<IUriReferenceProvider> GetProvidersAssignableTo(string uriScheme, Type targetType)
        {
            var compatibleProviders = Providers.Where(provider =>
                 (uriScheme == null || provider.UriScheme == uriScheme) &&
                 GetImplementedProviderInterfaces(provider.GetType())
                         .Any(@interface => targetType.IsAssignableFrom(@interface.GetGenericArguments()[0])));

            return compatibleProviders;
        }

        private static IEnumerable<Type> GetImplementedProviderInterfaces(Type type)
        {
            return type.GetInterfaces()
                .Where(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IUriReferenceProvider<>));
        }
    }
}