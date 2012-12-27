using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NuPattern.Extensibility.Serialization.Json
{
    /// <summary>
    /// Contract details for a <see cref="Type"/> used by the <see cref="JsonSerializer"/>.
    /// </summary>
    public class JsonArrayContract : JsonContract
    {
        internal Type CollectionItemType { get; private set; }
        //internal JsonContract CollectionItemContract { get; set; }

        private readonly bool _isCollectionItemTypeNullableType;
        private readonly Type _genericCollectionDefinitionType;
        private Type _genericWrapperType;
        private MethodCall<object, object> _genericWrapperCreator;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArrayContract"/> class.
        /// </summary>
        /// <param name="underlyingType">The underlying type for the contract.</param>
        public JsonArrayContract(Type underlyingType)
            : base(underlyingType)
        {
            ContractType = JsonContractType.Array;

            if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(ICollection<>), out _genericCollectionDefinitionType))
            {
                CollectionItemType = _genericCollectionDefinitionType.GetGenericArguments()[0];
            }
            else if (underlyingType.IsGenericType && underlyingType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                _genericCollectionDefinitionType = typeof(IEnumerable<>);
                CollectionItemType = underlyingType.GetGenericArguments()[0];
            }
            else
            {
                CollectionItemType = ReflectionUtils.GetCollectionItemType(UnderlyingType);
            }

            if (CollectionItemType != null)
                _isCollectionItemTypeNullableType = ReflectionUtils.IsNullableType(CollectionItemType);

            if (IsTypeGenericCollectionInterface(UnderlyingType))
            {
                CreatedType = ReflectionUtils.MakeGenericType(typeof(List<>), CollectionItemType);
            }
        }

        internal IWrappedCollection CreateWrapper(object list)
        {
            if ((list is IList && (CollectionItemType == null || !_isCollectionItemTypeNullableType))
              || UnderlyingType.IsArray)
                return new CollectionWrapper<object>((IList)list);

            if (_genericCollectionDefinitionType != null)
            {
                EnsureGenericWrapperCreator();
                return (IWrappedCollection)_genericWrapperCreator(null, list);
            }
            else
            {
                IList values = ((IEnumerable)list).Cast<object>().ToList();

                if (CollectionItemType != null)
                {
                    Array array = Array.CreateInstance(CollectionItemType, values.Count);
                    for (int i = 0; i < values.Count; i++)
                    {
                        array.SetValue(values[i], i);
                    }

                    values = array;
                }

                return new CollectionWrapper<object>(values);
            }
        }

        private void EnsureGenericWrapperCreator()
        {
            if (_genericWrapperCreator == null)
            {
                _genericWrapperType = ReflectionUtils.MakeGenericType(typeof(CollectionWrapper<>), CollectionItemType);

                Type constructorArgument;

                if (ReflectionUtils.InheritsGenericDefinition(_genericCollectionDefinitionType, typeof(List<>))
                  || _genericCollectionDefinitionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    constructorArgument = ReflectionUtils.MakeGenericType(typeof(ICollection<>), CollectionItemType);
                else
                    constructorArgument = _genericCollectionDefinitionType;

                ConstructorInfo genericWrapperConstructor = _genericWrapperType.GetConstructor(new[] { constructorArgument });
                _genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(genericWrapperConstructor);
            }
        }

        private bool IsTypeGenericCollectionInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type genericDefinition = type.GetGenericTypeDefinition();

            return (genericDefinition == typeof(IList<>)
                    || genericDefinition == typeof(ICollection<>)
                    || genericDefinition == typeof(IEnumerable<>));
        }
    }
}