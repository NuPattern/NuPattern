using System;
using Dsl = Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Store
{
    /// <summary>
    /// Reads a serialized product state model in memory.
    /// </summary>
    public static class ProductStateModelReader
    {
        /// <summary>
        /// Root type of the domain model, for modeling transformation initialization purposes.
        /// </summary>
        public static Type DomainModelType
        {
            get { return typeof(ProductStateStoreDomainModel); }
        }

        /// <summary>
        /// Reads the model into the given store and returns it.
        /// </summary>
        public static IProductState Read(Dsl.SerializationResult result, Dsl.Store store, string modelFile)
        {
            return ProductStateStoreSerializationHelper.Instance.LoadModel(result, store, modelFile, null, null, null);
        }
    }
}