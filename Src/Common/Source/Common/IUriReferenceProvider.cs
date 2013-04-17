using System;

namespace NuPattern
{
    /// <summary>
    /// Provides the basic non-generic protocol for the reference provider.
    /// Providers implementation should inherit from <see cref="IUriReferenceProvider{T}"/>
    /// </summary>
    public interface IUriReferenceProvider
    {
        /// <summary>
        /// Gets the scheme of the provider.
        /// For example: file, type, modelbus, etc.
        /// </summary>
        string UriScheme { get; }
    }

    /// <summary>
    /// Allows to create and resolve references for the target type T.
    /// </summary>
    /// <typeparam name="T">The type of instance to be resolved or referenced</typeparam>
    public interface IUriReferenceProvider<T> : IUriReferenceProvider
    {
        /// <summary>
        /// Creates a reference for the instance.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>A reference to the instance.</returns>
        Uri CreateUri(T instance);

        /// <summary>
        /// Resolves a reference to an instance of type T
        /// </summary>
        /// <param name="uri">The reference</param>
        /// <returns>The resolved instance</returns>
        T ResolveUri(Uri uri);

        /// <summary>
        /// Opens the instance in the appropiate view.
        /// </summary>
        /// <param name="instance"></param>
        void Open(T instance);
    }
}
