using System;

namespace NuPattern
{
    /// <summary>
    /// Creates and resolves uri references.
    /// </summary>
    public interface IUriReferenceService
    {
        /// <summary>
        /// Checks if the URI scheme is registered as a valid provider.
        /// </summary>
        /// <param name="uri">The URI for the scheme to be checked.</param>
        /// <returns><see langword="true"/>, if a provider exists for the scheme; otherwise <see langword="false"/></returns>
        bool IsSchemeRegistered(Uri uri);

        /// <summary>
        /// Resolves an instance of T based on the scheme of the URI 
        /// </summary>
        /// <typeparam name="T">The type of the instance to be resolved</typeparam>
        /// <param name="uri">The reference</param>
        /// <returns>The resolved reference</returns>
        T ResolveUri<T>(Uri uri) where T : class;

        /// <summary>
        /// Creates a reference for the instance of T. 
        /// If the scheme is not provided the type of T should be used to determine how the reference should be created. 
        /// </summary>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="uriScheme">The scheme will be used to determine how the reference should be created</param>
        /// <returns>The reference to the instance</returns>
        Uri CreateUri<T>(T instance, string uriScheme = null) where T : class;

        /// <summary>
        /// Opens the instance in the appropiate view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="uriScheme"></param>
        void Open<T>(T instance, string uriScheme = null) where T : class;

        /// <summary>
        /// Determines if the reference can be created for the instance of T
        /// </summary>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <param name="instance">The instance</param>
        /// <param name="uriScheme">The scheme will be used to determine how the reference should be created</param>
        /// <returns>True if the reference can be created</returns>
        bool CanCreateUri<T>(T instance, string uriScheme = null) where T : class;
    }
}
