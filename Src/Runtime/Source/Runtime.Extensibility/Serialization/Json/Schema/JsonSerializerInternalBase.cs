using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NuPattern.Extensibility.Serialization.Json
{
    internal abstract class JsonSerializerInternalBase
    {
        private ErrorContext _currentErrorContext;
        private BidirectionalDictionary<string, object> _mappings;

        internal readonly JsonSerializer Serializer;

        protected JsonSerializerInternalBase(JsonSerializer serializer)
        {
            Guard.NotNull(() => serializer, serializer);

            Serializer = serializer;
        }

        private class ReferenceEqualsEqualityComparer : IEqualityComparer<object>
        {
            bool IEqualityComparer<object>.Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj)
            {
#if !PocketPC
                // put objects in a bucket based on their reference
                return RuntimeHelpers.GetHashCode(obj);
#else
                // put all objects in the same bucket so ReferenceEquals is called on all
                return -1;
#endif
            }
        }

        internal BidirectionalDictionary<string, object> DefaultReferenceMappings
        {
            get
            {
                // override equality comparer for object key dictionary
                // object will be modified as it deserializes and might have mutable hashcode
                if (_mappings == null)
                    _mappings = new BidirectionalDictionary<string, object>(
                      EqualityComparer<string>.Default,
                      new ReferenceEqualsEqualityComparer());

                return _mappings;
            }
        }

        protected ErrorContext GetErrorContext(object currentObject, object member, Exception error)
        {
            if (_currentErrorContext == null)
                _currentErrorContext = new ErrorContext(currentObject, member, error);

            if (_currentErrorContext.Error != error)
                throw new InvalidOperationException("Current error context error is different to requested error.");

            return _currentErrorContext;
        }

        protected void ClearErrorContext()
        {
            if (_currentErrorContext == null)
                throw new InvalidOperationException("Could not clear error context. Error context is already null.");

            _currentErrorContext = null;
        }

        protected bool IsErrorHandled(object currentObject, JsonContract contract, object keyValue, Exception ex)
        {
            ErrorContext errorContext = GetErrorContext(currentObject, keyValue, ex);
            contract.InvokeOnError(currentObject, Serializer.Context, errorContext);

            if (!errorContext.Handled)
                Serializer.OnError(new ErrorEventArgs(currentObject, errorContext));

            return errorContext.Handled;
        }
    }
}