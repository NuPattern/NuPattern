using System;
using Microsoft.VisualStudio.Modeling;

namespace NuPattern.Runtime.Schema
{
    /// <summary>
    /// Reads a serialized product state model in memory.
    /// </summary>
    public static class PatternModelReader
    {
        /// <summary>
        /// Root type of the domain model, for modeling transformation initialization purposes.
        /// </summary>
        public static Type DomainModelType
        {
            get { return typeof(PatternModelDomainModel); }
        }

        /// <summary>
        /// Reads the model into the given store and returns it.
        /// </summary>
        public static IPatternModelSchema Read(SerializationResult result, Store store, string modelFile)
        {
            return PatternModelSerializationHelper.Instance.LoadModel(result, store, modelFile, null, null, null);
        }
    }
}