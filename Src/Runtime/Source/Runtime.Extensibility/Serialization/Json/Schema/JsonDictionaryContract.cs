using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonDictionaryContract : JsonContract
    {
        /// <summary>
        /// Gets or sets the property name resolver.
        /// </summary>
        /// <value>The property name resolver.</value>
        public Func<string, string> PropertyNameResolver { get; set; }

        internal Type DictionaryKeyType { get; private set; }
        internal Type DictionaryValueType { get; private set; }

        internal JsonContract DictionaryKeyContract { get; set; }
        internal JsonContract DictionaryValueContract { get; set; }

        private readonly bool _isDictionaryValueTypeNullableType;
        private readonly Type _genericCollectionDefinitionType;
        private Type _genericWrapperType;
        private MethodCall<object, object> _genericWrapperCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDictionaryContract"/> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonDictionaryContract(Type underlyingType)
            : base(underlyingType)
        {
            Type keyType;
            Type valueType;
            if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out _genericCollectionDefinitionType))
            {
                keyType = _genericCollectionDefinitionType.GetGenericArguments()[0];
                valueType = _genericCollectionDefinitionType.GetGenericArguments()[1];
            }
            else
            {
                ReflectionUtils.GetDictionaryKeyValueTypes(UnderlyingType, out keyType, out valueType);
            }

            DictionaryKeyType = keyType;
            DictionaryValueType = valueType;

            if (DictionaryValueType != null)
                _isDictionaryValueTypeNullableType = ReflectionUtils.IsNullableType(DictionaryValueType);

            if (IsTypeGenericDictionaryInterface(UnderlyingType))
            {
                CreatedType = ReflectionUtils.MakeGenericType(typeof(Dictionary<,>), keyType, valueType);
            }
            else if (UnderlyingType == typeof(IDictionary))
            {
                CreatedType = typeof(Dictionary<object, object>);
            }
        }

        internal IWrappedDictionary CreateWrapper(object dictionary)
        {
            if (dictionary is IDictionary && (DictionaryValueType == null || !_isDictionaryValueTypeNullableType))
                return new DictionaryWrapper<object, object>((IDictionary)dictionary);

            if (_genericWrapperCreator == null)
            {
                _genericWrapperType = ReflectionUtils.MakeGenericType(typeof(DictionaryWrapper<,>), DictionaryKeyType, DictionaryValueType);

                ConstructorInfo genericWrapperConstructor = _genericWrapperType.GetConstructor(new[] { _genericCollectionDefinitionType });
                _genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(genericWrapperConstructor);
            }

            return (IWrappedDictionary)_genericWrapperCreator(null, dictionary);
        }

        private bool IsTypeGenericDictionaryInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericDefinition = type.GetGenericTypeDefinition();

            return (genericDefinition == typeof(IDictionary<,>));
        }
    }
}