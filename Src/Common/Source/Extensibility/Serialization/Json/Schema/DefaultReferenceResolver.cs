using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Patterning.Extensibility.Serialization.Json
{
    internal class DefaultReferenceResolver : IReferenceResolver
    {
        private int _referenceCount;

        private BidirectionalDictionary<string, object> GetMappings(object context)
        {
            JsonSerializerInternalBase internalSerializer;

            if (context is JsonSerializerInternalBase)
                internalSerializer = (JsonSerializerInternalBase)context;
            else if (context is JsonSerializerProxy)
                internalSerializer = ((JsonSerializerProxy)context).GetInternalSerializer();
            else
                throw new Exception("The DefaultReferenceResolver can only be used internally.");

            return internalSerializer.DefaultReferenceMappings;
        }

        public object ResolveReference(object context, string reference)
        {
            object value;
            GetMappings(context).TryGetByFirst(reference, out value);
            return value;
        }

        public string GetReference(object context, object value)
        {
            var mappings = GetMappings(context);

            string reference;
            if (!mappings.TryGetBySecond(value, out reference))
            {
                _referenceCount++;
                reference = _referenceCount.ToString(CultureInfo.InvariantCulture);
                mappings.Add(reference, value);
            }

            return reference;
        }

        public void AddReference(object context, string reference, object value)
        {
            GetMappings(context).Add(reference, value);
        }

        public bool IsReferenced(object context, object value)
        {
            string reference;
            return GetMappings(context).TryGetBySecond(value, out reference);
        }
    }
}