using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace NuPattern.Runtime.Composition
{
    /// <summary>
    /// Main interface that allows features to create additional components 
    /// it may need for its operation, that integrate seamlessly with 
    /// the composition infrastructure of the feature extensions runtime and 
    /// Visual Studio.
    /// </summary>
    public interface IFeatureCompositionService : ICompositionService, IDisposable
    {
        /// <summary>
        /// Composes the given parts
        /// </summary>
        void ComposeParts(params object[] attributedParts);

        /// <summary>
        /// Gets the export of the given type.
        /// </summary>
        Lazy<T> GetExport<T>();

        /// <summary>
        /// Gets the export of the given type, that also match the contract name.
        /// </summary>
        Lazy<T> GetExport<T>(string contractName);

        /// <summary>
        /// Gets the export of the given type, that matches the type of the given metadata
        /// </summary>
        Lazy<T, TMetadataView> GetExport<T, TMetadataView>();

        /// <summary>
        /// Gets the export of the given type, that matches the given metadata and contract name.
        /// </summary>
        Lazy<T, TMetadataView> GetExport<T, TMetadataView>(string contractName);

        /// <summary>
        /// Gets the exported value of the given type.
        /// </summary>
        T GetExportedValue<T>();

        /// <summary>
        /// Gets the exported value of the given type, that matches the given contract.
        /// </summary>
        T GetExportedValue<T>(string contractName);

        /// <summary>
        /// Gets the exported value of the given type.
        /// </summary>
        T GetExportedValueOrDefault<T>();

        /// <summary>
        /// Gets the exported value of the given type, that matches the given contract.
        /// </summary>
        T GetExportedValueOrDefault<T>(string contractName);

        /// <summary>
        /// Gets the exported values of the given type.
        /// </summary>
        IEnumerable<T> GetExportedValues<T>();

        /// <summary>
        /// Gets the exported values of the given type, that matches the given contract.
        /// </summary>
        IEnumerable<T> GetExportedValues<T>(string contractName);

        /// <summary>
        /// Gets the exports of the given type.
        /// </summary>
        IEnumerable<Lazy<T>> GetExports<T>();

        /// <summary>
        /// Gets the exports of the given type, that matches the given contract.
        /// </summary>
        IEnumerable<Lazy<T>> GetExports<T>(string contractName);

        /// <summary>
        /// Gets the exports of the given type, that matches the type of the given metadata.
        /// </summary>
        IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>();

        /// <summary>
        /// Gets the exports of the given type, that matches the type of the given metadata and contract name.
        /// </summary>
        IEnumerable<Lazy<T, TMetadataView>> GetExports<T, TMetadataView>(string contractName);
    }
}
